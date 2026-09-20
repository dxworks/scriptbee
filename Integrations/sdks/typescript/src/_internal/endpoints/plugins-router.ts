import { Hono } from "hono";
import type { GetInstalledPluginsUseCase } from "../../abstractions/get-installed-plugins-use-case.js";
import type { InstallPluginUseCase } from "../../abstractions/install-plugin-use-case.js";
import type { UninstallPluginUseCase } from "../../abstractions/uninstall-plugin-use-case.js";
import {
  WebInstallPluginCommandSchema,
  webInstalledPluginFromDomain,
} from "./contracts/web-models.js";

export function makePluginsRouter(options: {
  getInstalledPlugins: () => GetInstalledPluginsUseCase;
  installPlugin: () => InstallPluginUseCase;
  uninstallPlugin: () => UninstallPluginUseCase;
}): Hono {
  const router = new Hono();

  router.get("/api/plugins", (c) => {
    const plugins = options.getInstalledPlugins().get();
    return c.json({ data: plugins.map(webInstalledPluginFromDomain) });
  });

  router.post("/api/plugins", (c) => {
    return c.req.json().then((body) => {
      const parsed = WebInstallPluginCommandSchema.safeParse(body);

      if (!parsed.success) {
        return c.json({ title: "Validation Failed", detail: parsed.error.message }, 400);
      }

      const pluginId = { name: parsed.data.pluginId, version: parsed.data.version };
      const result = options.installPlugin().installPlugin(pluginId);

      if (result._tag === "Success") {
        return new Response(null, { status: 204 });
      }

      if (result._tag === "InvalidPluginError") {
        return c.json(
          {
            title: "Plugin Installation Failed",
            detail: `Invalid plugin version: ${result.id.version} for ${result.id.name}`,
          },
          400,
        );
      }

      return c.json(
        {
          title: "Plugin Installation Failed",
          detail: `An error occurred while installing plugin ${result.id.name} version ${result.id.version}`,
        },
        500,
      );
    });
  });

  router.delete("/api/plugins/:pluginId", (c) => {
    const pluginId = c.req.param("pluginId");
    const version = c.req.query("version") ?? "";
    options.uninstallPlugin().uninstallPlugin({ name: pluginId, version });
    return new Response(null, { status: 204 });
  });

  return router;
}
