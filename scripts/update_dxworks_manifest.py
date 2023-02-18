#!/usr/bin/env python3

import sys
import yaml


def verify_version_exists(version, versions):
    for v in versions:
        if v['version'] == version:
            return True
    return False


def is_semver(version):
    parts = version.split('.')
    if len(parts) != 3:
        return False

    for part in parts:
        try:
            if int(part) < 0:
                return False
        except ValueError:
            return False

    return True


def verify_version(version):
    if version.startswith('v'):
        return False

    if not is_semver(version):
        return False

    return True


def main():
    if len(sys.argv) < 3:
        print("Usage: update_dxworks_manifest.py <version> <file>")
        sys.exit(1)

    version = sys.argv[1]
    manifest_file = sys.argv[2]

    with open(manifest_file, 'r') as stream:
        manifest = yaml.safe_load(stream)

    scriptbee = manifest['scriptBee']
    versions = scriptbee['versions']

    if not verify_version(version):
        print("Version is not valid")
        sys.exit(1)

    if verify_version_exists(version, versions):
        print("Version already exists")
        sys.exit(1)

    versions.append({
        "version": version,
        "name": "dxworks/scriptbee",
        "sourceCode": "https://github.com/dxworks/scriptbee/tree/v" + version + "/Plugins",
        "manifest": "https://raw.githubusercontent.com/dxworks/scriptbee/v" + version + "/Plugins/manifest.yaml",
        "asset": "honeydew-scriptbee.zip"
    })

    with open(manifest_file, 'w') as stream:
        yaml.dump(manifest, stream, sort_keys=False)


if __name__ == "__main__":
    main()
