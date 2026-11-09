import argparse
import difflib
import json
import re
import subprocess
import sys
import tomllib
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parent.parent
CHANGELOG_RELEASE_TAGS = {
    "Plugins/CHANGELOG.md": "bundle",
    "DxWorks.ScriptBee.Plugin.Api/CHANGELOG.md": "plugin-api",
    "Integrations/MCP/CHANGELOG.md": "mcp",
    "Integrations/VS-Code/CHANGELOG.md": "vs-code",
}
NUGET_PROJECTS_BY_CHANGELOG = {
    "Plugins/CHANGELOG.md": ("Plugins/",),
    "DxWorks.ScriptBee.Plugin.Api/CHANGELOG.md": ("DxWorks.ScriptBee.Plugin.Api/",),
    "Integrations/MCP/CHANGELOG.md": ("Integrations/MCP/",),
    "Integrations/sdks/CHANGELOG.md": ("Integrations/sdks/csharp/",),
}
NPM_PROJECTS_BY_CHANGELOG = {
    "Plugins/CHANGELOG.md": ("Plugins/UI/default-scriptbee-charts",),
    "Integrations/VS-Code/CHANGELOG.md": ("Integrations/VS-Code",),
    "Integrations/sdks/CHANGELOG.md": ("Integrations/sdks/typescript",),
}
NPM_SECTIONS = ("dependencies", "optionalDependencies", "peerDependencies")
VERSION_PATTERN = re.compile(r"\d+(?:\.\d+)+(?:[-+][A-Za-z0-9.-]+)?")


def git_output(arguments):
    return subprocess.run(
        ["git", *arguments],
        cwd=ROOT,
        capture_output=True,
        text=True,
        check=True,
    ).stdout


def git_show(ref, relative_path):
    result = subprocess.run(
        ["git", "show", f"{ref}:{relative_path}"],
        cwd=ROOT,
        capture_output=True,
        text=True,
        check=False,
    )
    return result.stdout if result.returncode == 0 else None


def parse_nuget_versions(content):
    if content is None:
        return {}
    root = ET.fromstring(content)
    return {
        element.get("Include", "").casefold(): (
            element.get("Include", ""),
            element.get("Version", ""),
        )
        for element in root.iter()
        if element.tag.rsplit("}", 1)[-1] == "PackageVersion"
    }


def parse_project_references(content, include_private_assets=False):
    root = ET.fromstring(content)
    references = {}
    for element in root.iter():
        if element.tag.rsplit("}", 1)[-1] != "PackageReference":
            continue
        private_assets = next(
            (
                child.text.strip().casefold()
                for child in element
                if child.tag.rsplit("}", 1)[-1] == "PrivateAssets" and child.text
            ),
            "",
        )
        if private_assets == "all" and not include_private_assets:
            continue
        name = element.get("Include", "")
        references[name.casefold()] = (name, element.get("Version"))
    return references


def project_paths(ref, directory):
    if ref == "WORKTREE":
        return [
            path.relative_to(ROOT).as_posix()
            for path in (ROOT / directory).rglob("*.csproj")
        ]
    return [
        path
        for path in git_output(["ls-tree", "-r", "--name-only", ref, "--", directory])
        .splitlines()
        if path.endswith(".csproj")
    ]


def nuget_versions(ref, changelog):
    central_content = (
        (ROOT / "Directory.Packages.props").read_text(encoding="utf-8")
        if ref == "WORKTREE"
        else git_show(ref, "Directory.Packages.props")
    )
    central_versions = parse_nuget_versions(central_content)
    packages = {}

    for directory in NUGET_PROJECTS_BY_CHANGELOG.get(changelog, ()):
        for relative_path in project_paths(ref, directory):
            if any("test" in part.casefold() for part in relative_path.split("/")):
                continue
            project_content = (
                (ROOT / relative_path).read_text(encoding="utf-8")
                if ref == "WORKTREE"
                else git_show(ref, relative_path)
            )
            if project_content is None:
                continue
            for key, (name, project_version) in parse_project_references(project_content).items():
                central_package = central_versions.get(key)
                version = project_version or (central_package[1] if central_package else None)
                if version:
                    packages[key] = (name, version)

    return packages


def npm_manifest_versions(content):
    if content is None:
        return {}
    manifest = json.loads(content)
    dependencies = {
        name: version
        for section in NPM_SECTIONS
        for name, version in manifest.get(section, {}).items()
    }
    vscode_engine = manifest.get("engines", {}).get("vscode")
    if vscode_engine:
        dependencies["vscode engine"] = vscode_engine
    return dependencies


def npm_manifest_dependency_names(content):
    if content is None:
        return set()
    manifest = json.loads(content)
    names = {
        name.casefold()
        for section in (*NPM_SECTIONS, "devDependencies")
        for name in manifest.get(section, {})
    }
    if manifest.get("engines", {}).get("vscode"):
        names.add("vscode engine")
    return names


def npm_lock_versions(content):
    if content is None:
        return {}
    lock = json.loads(content)
    return {
        path.removeprefix("node_modules/"): package["version"]
        for path, package in lock.get("packages", {}).items()
        if path.startswith("node_modules/") and package.get("version")
    }


def npm_versions(ref, directory):
    manifest_path = f"{directory}/package.json"
    lock_path = f"{directory}/package-lock.json"
    manifest_content = (
        (ROOT / manifest_path).read_text(encoding="utf-8")
        if ref == "WORKTREE"
        else git_show(ref, manifest_path)
    )
    lock_content = (
        (ROOT / lock_path).read_text(encoding="utf-8")
        if ref == "WORKTREE"
        else git_show(ref, lock_path)
    )
    manifest_versions = npm_manifest_versions(manifest_content)
    lock_versions = npm_lock_versions(lock_content)
    return {
        name: (
            name,
            lock_versions.get(name, version.lstrip("^~<>= "))
            if name != "vscode engine"
            else version.lstrip("^~<>= "),
        )
        for name, version in manifest_versions.items()
    }


def normalize_python_name(name):
    return re.sub(r"[-_.]+", "-", name).casefold()


def python_manifest_versions(content):
    if content is None:
        return {}
    project = tomllib.loads(content).get("project", {})
    versions = {}
    for requirement in project.get("dependencies", []):
        name = re.match(r"\s*([A-Za-z0-9_.-]+)", requirement)
        version = VERSION_PATTERN.search(requirement)
        if name:
            versions[normalize_python_name(name.group(1))] = (
                name.group(1),
                version.group() if version else "",
            )
    return versions


def python_manifest_dependency_names(content):
    if content is None:
        return set()
    project = tomllib.loads(content).get("project", {})
    names = {
        normalize_python_name(match.group(1))
        for requirement in project.get("dependencies", [])
        if (match := re.match(r"\s*([A-Za-z0-9_.-]+)", requirement))
    }
    for requirements in project.get("dependency-groups", {}).values():
        names.update(
            normalize_python_name(match.group(1))
            for requirement in requirements
            if isinstance(requirement, str)
            and (match := re.match(r"\s*([A-Za-z0-9_.-]+)", requirement))
        )
    return names


def python_lock_versions(content):
    if content is None:
        return {}
    lock = tomllib.loads(content)
    return {
        normalize_python_name(package["name"]): package["version"]
        for package in lock.get("package", [])
        if package.get("name") and package.get("version")
    }


def python_versions(ref):
    manifest_path = "Integrations/sdks/python/pyproject.toml"
    lock_path = "Integrations/sdks/python/uv.lock"
    manifest_content = (
        (ROOT / manifest_path).read_text(encoding="utf-8")
        if ref == "WORKTREE"
        else git_show(ref, manifest_path)
    )
    lock_content = (
        (ROOT / lock_path).read_text(encoding="utf-8")
        if ref == "WORKTREE"
        else git_show(ref, lock_path)
    )
    manifest_versions = python_manifest_versions(manifest_content)
    lock_versions = python_lock_versions(lock_content)
    return {
        name: (display_name, lock_versions.get(name, requirement_version))
        for name, (display_name, requirement_version) in manifest_versions.items()
    }


def released_sdk_ref(changelog_path):
    changelog = (ROOT / changelog_path).read_text(encoding="utf-8")
    released_heading = next(
        (
            line.strip()
            for line in changelog.splitlines()
            if line.startswith("## [") and line.strip() != "## [Unreleased]"
        ),
        None,
    )
    if released_heading is None:
        raise ValueError(f"{changelog_path} has no released version heading")
    commits = git_output(
        [
            "log",
            "--all",
            "--reverse",
            "--format=%H",
            f"-S{released_heading}",
            "--",
            changelog_path,
        ]
    ).splitlines()
    if not commits:
        raise ValueError(f"Could not find the commit that introduced {released_heading}")
    return commits[0]


def released_ref(changelog_path):
    if changelog_path not in CHANGELOG_RELEASE_TAGS:
        return released_sdk_ref(changelog_path)

    tag_prefix = CHANGELOG_RELEASE_TAGS[changelog_path]
    tags = git_output(
        ["tag", "--list", f"{tag_prefix}@*", "--sort=-version:refname"]
    ).splitlines()
    if not tags:
        raise ValueError(
            f"No {tag_prefix}@ release tag found; fetch tags before updating changelogs"
        )
    return tags[0]


def current_dependencies(changelog_path, ref):
    packages = nuget_versions(ref, changelog_path)
    for directory in NPM_PROJECTS_BY_CHANGELOG.get(changelog_path, ()):
        packages.update(npm_versions(ref, directory))
    if changelog_path == "Integrations/sdks/CHANGELOG.md":
        packages.update(python_versions(ref))
    return packages


def managed_dependency_names(changelog_path, ref):
    names = set()
    for directory in NUGET_PROJECTS_BY_CHANGELOG.get(changelog_path, ()):
        for relative_path in project_paths(ref, directory):
            project_content = (
                (ROOT / relative_path).read_text(encoding="utf-8")
                if ref == "WORKTREE"
                else git_show(ref, relative_path)
            )
            if project_content is not None:
                names.update(
                    parse_project_references(project_content, include_private_assets=True)
                )

    for directory in NPM_PROJECTS_BY_CHANGELOG.get(changelog_path, ()):
        manifest_path = f"{directory}/package.json"
        manifest_content = (
            (ROOT / manifest_path).read_text(encoding="utf-8")
            if ref == "WORKTREE"
            else git_show(ref, manifest_path)
        )
        names.update(npm_manifest_dependency_names(manifest_content))

    if changelog_path == "Integrations/sdks/CHANGELOG.md":
        manifest_path = "Integrations/sdks/python/pyproject.toml"
        manifest_content = (
            (ROOT / manifest_path).read_text(encoding="utf-8")
            if ref == "WORKTREE"
            else git_show(ref, manifest_path)
        )
        names.update(python_manifest_dependency_names(manifest_content))

    return names


def changed_dependencies(changelog_path):
    release_ref = released_ref(changelog_path)
    released = current_dependencies(changelog_path, release_ref)
    current = current_dependencies(changelog_path, "WORKTREE")
    changes = {}
    for key, (name, version) in current.items():
        released_package = released.get(key)
        if released_package is None or released_package[1] != version:
            changes[name] = version
    managed_names = managed_dependency_names(changelog_path, "WORKTREE")
    return changes, managed_names


def updated_changelog_content(changelog_path, dependencies, managed_names):
    original = changelog_path.read_text(encoding="utf-8")
    lines = original.splitlines()
    unreleased_start = next(
        (index for index, line in enumerate(lines) if line.strip() == "## [Unreleased]"),
        None,
    )
    if unreleased_start is None:
        raise ValueError(f"{changelog_path.relative_to(ROOT)} has no [Unreleased] section")

    unreleased_end = next(
        (
            index
            for index in range(unreleased_start + 1, len(lines))
            if lines[index].startswith("## [")
        ),
        len(lines),
    )
    section = lines[unreleased_start + 1 : unreleased_end]
    existing_entry = re.compile(r"^- (?:update|upgrade) `([^`]+)`(?: to .*)?$", re.IGNORECASE)
    section = [
        line
        for line in section
        if not ((match := existing_entry.match(line)) and match.group(1).casefold() in managed_names)
    ]
    entries = [
        f"- update `{name}` to {version}"
        for name, version in sorted(dependencies.items(), key=lambda item: item[0].casefold())
    ]
    changed_heading = next(
        (index for index, line in enumerate(section) if line.strip() == "### Changed"),
        None,
    )

    if changed_heading is None:
        while section and not section[-1].strip():
            section.pop()
        section.extend(["", "### Changed", "", *entries])
    else:
        changed_end = next(
            (
                index
                for index in range(changed_heading + 1, len(section))
                if section[index].startswith("### ")
            ),
            len(section),
        )
        changed_section = section[changed_heading + 1 : changed_end]
        while changed_section and not changed_section[0].strip():
            changed_section.pop(0)
        while changed_section and not changed_section[-1].strip():
            changed_section.pop()
        if changed_section:
            changed_section.append("")
        section[changed_heading + 1 : changed_end] = ["", *changed_section, *entries]

    while section and not section[-1].strip():
        section.pop()
    if unreleased_end < len(lines):
        section.append("")

    updated = lines[: unreleased_start + 1] + section + lines[unreleased_end:]
    return "\n".join(updated) + "\n"


def main():
    parser = argparse.ArgumentParser(
        description="Reconcile unreleased dependency notes with the latest released versions."
    )
    parser.add_argument(
        "--write",
        action="store_true",
        help="Write changelog updates; without this option, only show a preview",
    )
    args = parser.parse_args()

    modified = []
    for relative_path in (
        *CHANGELOG_RELEASE_TAGS.keys(),
        "Integrations/sdks/CHANGELOG.md",
    ):
        dependencies, managed_names = changed_dependencies(relative_path)
        changelog_path = ROOT / relative_path
        if changelog_path.is_symlink():
            raise ValueError(f"Refusing to update symlinked changelog: {relative_path}")
        original = changelog_path.read_text(encoding="utf-8")
        updated = updated_changelog_content(changelog_path, dependencies, managed_names)
        if updated == original:
            continue

        modified.append(relative_path)
        if args.write:
            newline = "\r\n" if b"\r\n" in changelog_path.read_bytes() else "\n"
            changelog_path.write_bytes(updated.replace("\n", newline).encode("utf-8"))
        else:
            print(
                "".join(
                    difflib.unified_diff(
                        original.splitlines(keepends=True),
                        updated.splitlines(keepends=True),
                        fromfile=relative_path,
                        tofile=relative_path,
                    )
                ),
                end="",
            )

    if not modified:
        print("Changelogs already match dependencies changed since their latest release.")
    elif args.write:
        print("Updated changelogs: " + ", ".join(modified))


if __name__ == "__main__":
    try:
        main()
    except (
        OSError,
        ET.ParseError,
        ValueError,
        json.JSONDecodeError,
        subprocess.CalledProcessError,
    ) as error:
        print(error, file=sys.stderr)
        sys.exit(1)
