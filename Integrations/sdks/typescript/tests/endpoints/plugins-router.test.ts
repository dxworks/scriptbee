import { describe, expect, it, vi } from "vitest";
import {
  invalidPluginError,
  pluginInstallationError,
} from "../../src/abstractions/errors.js";
import type { GetInstalledPluginsUseCase } from "../../src/abstractions/get-installed-plugins-use-case.js";
import {
  success,
  type InstallPluginUseCase,
} from "../../src/abstractions/install-plugin-use-case.js";
import type { UninstallPluginUseCase } from "../../src/abstractions/uninstall-plugin-use-case.js";
import { PluginKind } from "../../src/domain/plugins.js";
import { makePluginsRouter } from "../../src/_internal/endpoints/plugins-router.js";

function createRouter(overrides?: {
  getInstalledPlugins?: GetInstalledPluginsUseCase;
  installPlugin?: InstallPluginUseCase;
  uninstallPlugin?: UninstallPluginUseCase;
}) {
  const getInstalledPlugins: GetInstalledPluginsUseCase =
    overrides?.getInstalledPlugins ?? {
      get: vi.fn().mockReturnValue([
        {
          id: { name: "test-plugin", version: "1.0.0" },
          folderPath: "/plugins/test-plugin",
          manifest: {
            apiVersion: "1.0.0",
            name: "test-plugin",
            description: "Test plugin",
            author: "ScriptBee",
            extensionPoints: [
              {
                kind: PluginKind.LOADER,
                name: "CustomLoader",
                description: "Loader",
              },
            ],
          },
        },
      ]),
    };
  const installPlugin: InstallPluginUseCase = overrides?.installPlugin ?? {
    installPlugin: vi.fn().mockReturnValue(success()),
  };
  const uninstallPlugin: UninstallPluginUseCase =
    overrides?.uninstallPlugin ?? {
      uninstallPlugin: vi.fn(),
    };

  const router = makePluginsRouter({
    getInstalledPlugins: () => getInstalledPlugins,
    installPlugin: () => installPlugin,
    uninstallPlugin: () => uninstallPlugin,
  });

  return { router, getInstalledPlugins, installPlugin, uninstallPlugin };
}

describe("makePluginsRouter", () => {
  it("handles GET /api/plugins", async () => {
    const { router, getInstalledPlugins } = createRouter();

    const response = await router.request("/api/plugins");
    expect(response.status).toBe(200);

    const json = (await response.json()) as {
      data: Array<{ id: string; version: string }>;
    };
    expect(json.data).toHaveLength(1);
    expect(json.data[0]?.id).toBe("test-plugin");
    expect(json.data[0]?.version).toBe("1.0.0");
    expect(getInstalledPlugins.get).toHaveBeenCalled();
  });

  it("handles POST /api/plugins with success result returning 204", async () => {
    const { router, installPlugin } = createRouter();

    const response = await router.request("/api/plugins", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        pluginId: "my-plugin",
        version: "2.0.0",
      }),
    });

    expect(response.status).toBe(204);
    expect(installPlugin.installPlugin).toHaveBeenCalledWith({
      name: "my-plugin",
      version: "2.0.0",
    });
  });

  it("handles POST /api/plugins with InvalidPluginError returning 400", async () => {
    const installPlugin: InstallPluginUseCase = {
      installPlugin: vi
        .fn()
        .mockReturnValue(
          invalidPluginError({ name: "bad-plugin", version: "0.0.0" }),
        ),
    };
    const { router } = createRouter({ installPlugin });

    const response = await router.request("/api/plugins", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        pluginId: "bad-plugin",
        version: "0.0.0",
      }),
    });

    expect(response.status).toBe(400);
    const json = (await response.json()) as { title: string; detail: string };
    expect(json.title).toBe("Plugin Installation Failed");
    expect(json.detail).toContain("Invalid plugin version");
  });

  it("handles POST /api/plugins with PluginInstallationError returning 500", async () => {
    const installPlugin: InstallPluginUseCase = {
      installPlugin: vi
        .fn()
        .mockReturnValue(
          pluginInstallationError({ name: "err-plugin", version: "1.0.0" }),
        ),
    };
    const { router } = createRouter({ installPlugin });

    const response = await router.request("/api/plugins", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        pluginId: "err-plugin",
        version: "1.0.0",
      }),
    });

    expect(response.status).toBe(500);
    const json = (await response.json()) as { title: string; detail: string };
    expect(json.title).toBe("Plugin Installation Failed");
    expect(json.detail).toContain("An error occurred");
  });

  it("handles POST /api/plugins with invalid payload returning 400", async () => {
    const { router, installPlugin } = createRouter();

    const response = await router.request("/api/plugins", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        pluginId: "",
      }),
    });

    expect(response.status).toBe(400);
    expect(installPlugin.installPlugin).not.toHaveBeenCalled();
  });

  it("handles DELETE /api/plugins/:pluginId returning 204", async () => {
    const { router, uninstallPlugin } = createRouter();

    const response = await router.request("/api/plugins/plug-1?version=1.2.3", {
      method: "DELETE",
    });

    expect(response.status).toBe(204);
    expect(uninstallPlugin.uninstallPlugin).toHaveBeenCalledWith({
      name: "plug-1",
      version: "1.2.3",
    });
  });
});
