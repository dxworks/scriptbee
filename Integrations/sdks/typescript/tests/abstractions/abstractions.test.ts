import { describe, expect, it } from "vitest";
import type { ClearContextUseCase } from "../../src/abstractions/clear-context-use-case.js";
import type { GenerateClassesUseCase } from "../../src/abstractions/generate-classes-use-case.js";
import type { GetContextGraphUseCase } from "../../src/abstractions/get-context-graph-use-case.js";
import type { GetContextUseCase } from "../../src/abstractions/get-context-use-case.js";
import type { GetInstalledPluginsUseCase } from "../../src/abstractions/get-installed-plugins-use-case.js";
import {
  type InstallPluginResult,
  type InstallPluginUseCase,
  success,
} from "../../src/abstractions/install-plugin-use-case.js";
import type { LinkContextUseCase } from "../../src/abstractions/link-context-use-case.js";
import type { LoadContextUseCase } from "../../src/abstractions/load-context-use-case.js";
import type { RunAnalysisCommand, RunAnalysisUseCase } from "../../src/abstractions/run-analysis-use-case.js";
import type { UninstallPluginUseCase } from "../../src/abstractions/uninstall-plugin-use-case.js";
import { invalidPluginError, pluginInstallationError } from "../../src/abstractions/errors.js";
import type { AnalysisInfo } from "../../src/domain/analysis.js";
import type { SampleCodeFile } from "../../src/domain/code-generation.js";
import type { ContextGraphResult, ContextSlice } from "../../src/domain/context.js";
import type { Plugin, PluginId } from "../../src/domain/plugins.js";
import type { FileId } from "../../src/domain/project.js";

const stubAnalysisInfo: AnalysisInfo = {
  id: { value: "an-1" },
  projectId: { value: "proj-1" },
  instanceId: { value: "inst-1" },
  scriptId: { value: "scr-1" },
  status: { value: "Started" },
  creationDate: new Date("2024-01-01T00:00:00Z"),
  results: [],
  errors: [],
};

class StubRunAnalysis implements RunAnalysisUseCase {
  async run(_command: RunAnalysisCommand): Promise<AnalysisInfo> {
    return stubAnalysisInfo;
  }
}

class StubGetContext implements GetContextUseCase {
  get(): ContextSlice[] {
    return [];
  }
}

class StubLoadContext implements LoadContextUseCase {
  async load(_filesToLoad: Record<string, FileId[]>): Promise<void> {}
}

class StubLinkContext implements LinkContextUseCase {
  async link(_linkerIds: string[]): Promise<void> {}
}

class StubClearContext implements ClearContextUseCase {
  clear(): void {}
}

class StubGetContextGraph implements GetContextGraphUseCase {
  searchNodes(_query: string, _offset: number, _limit: number): ContextGraphResult {
    return { nodes: [], edges: [] };
  }

  getNeighbors(_nodeId: string): ContextGraphResult {
    return { nodes: [], edges: [] };
  }
}

class StubGenerateClasses implements GenerateClassesUseCase {
  async generateClasses(_languages: string[]): Promise<SampleCodeFile[]> {
    return [];
  }
}

class StubGetInstalledPlugins implements GetInstalledPluginsUseCase {
  get(): Plugin[] {
    return [];
  }
}

class StubInstallPlugin implements InstallPluginUseCase {
  constructor(private readonly result: InstallPluginResult) {}

  installPlugin(_pluginId: PluginId): InstallPluginResult {
    return this.result;
  }
}

class StubUninstallPlugin implements UninstallPluginUseCase {
  uninstallPlugin(_pluginId: PluginId): void {}
}

describe("RunAnalysisUseCase", () => {
  it("returns analysis info from stub implementation", async () => {
    const useCase = new StubRunAnalysis();
    const result = await useCase.run({ projectId: { value: "p" }, scriptId: { value: "s" } });
    expect(result.id.value).toBe("an-1");
    expect(result.status.value).toBe("Started");
  });
});

describe("GetContextUseCase", () => {
  it("returns an empty list from stub", () => {
    expect(new StubGetContext().get()).toEqual([]);
  });
});

describe("LoadContextUseCase", () => {
  it("resolves without error from stub", async () => {
    await expect(new StubLoadContext().load({ "loader-1": [{ value: "f1" }] })).resolves.toBeUndefined();
  });
});

describe("LinkContextUseCase", () => {
  it("resolves without error from stub", async () => {
    await expect(new StubLinkContext().link(["linker-1"])).resolves.toBeUndefined();
  });
});

describe("ClearContextUseCase", () => {
  it("clears without error from stub", () => {
    expect(() => new StubClearContext().clear()).not.toThrow();
  });
});

describe("GetContextGraphUseCase", () => {
  it("searchNodes returns empty graph from stub", () => {
    const result = new StubGetContextGraph().searchNodes("q", 0, 10);
    expect(result.nodes).toEqual([]);
    expect(result.edges).toEqual([]);
  });

  it("getNeighbors returns empty graph from stub", () => {
    const result = new StubGetContextGraph().getNeighbors("node-1");
    expect(result.nodes).toEqual([]);
  });
});

describe("GenerateClassesUseCase", () => {
  it("returns empty list from stub", async () => {
    const result = await new StubGenerateClasses().generateClasses(["typescript"]);
    expect(result).toEqual([]);
  });
});

describe("GetInstalledPluginsUseCase", () => {
  it("returns empty list from stub", () => {
    expect(new StubGetInstalledPlugins().get()).toEqual([]);
  });
});

describe("InstallPluginUseCase", () => {
  it("returns Success result", () => {
    const result = new StubInstallPlugin(success()).installPlugin({ name: "p", version: "1.0" });
    expect(result._tag).toBe("Success");
  });

  it("returns InvalidPluginError result", () => {
    const pluginId = { name: "p", version: "bad" };
    const result = new StubInstallPlugin(invalidPluginError(pluginId)).installPlugin(pluginId);
    expect(result._tag).toBe("InvalidPluginError");
    if (result._tag === "InvalidPluginError") {
      expect(result.id).toEqual(pluginId);
    }
  });

  it("returns PluginInstallationError result", () => {
    const pluginId = { name: "p", version: "1.0" };
    const result = new StubInstallPlugin(pluginInstallationError(pluginId)).installPlugin(pluginId);
    expect(result._tag).toBe("PluginInstallationError");
  });
});

describe("UninstallPluginUseCase", () => {
  it("uninstalls without error from stub", () => {
    expect(() => new StubUninstallPlugin().uninstallPlugin({ name: "p", version: "1.0" })).not.toThrow();
  });
});

describe("success helper", () => {
  it("is distinct from error types", () => {
    const s = success();
    expect(s._tag).toBe("Success");
    expect(s._tag).not.toBe("InvalidPluginError");
    expect(s._tag).not.toBe("PluginInstallationError");
  });
});
