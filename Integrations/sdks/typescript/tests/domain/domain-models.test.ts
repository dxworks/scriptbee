import { describe, expect, it } from "vitest";
import { ProjectIdSchema, ScriptIdSchema, FileIdSchema, InstanceIdSchema } from "../../src/domain/project.js";
import { SampleCodeFileSchema } from "../../src/domain/code-generation.js";
import { AnalysisInfoSchema, ResultType } from "../../src/domain/analysis.js";
import { PluginIdSchema, PluginKind } from "../../src/domain/plugins.js";
import { ContextSliceSchema, ContextGraphNodeSchema } from "../../src/domain/context.js";

describe("ProjectId", () => {
  it("accepts a non-empty value", () => {
    const result = ProjectIdSchema.safeParse({ value: "proj-1" });
    expect(result.success).toBe(true);
  });

  it("rejects an empty string", () => {
    const result = ProjectIdSchema.safeParse({ value: "" });
    expect(result.success).toBe(false);
  });
});

describe("ScriptId", () => {
  it("accepts a non-empty value", () => {
    const result = ScriptIdSchema.safeParse({ value: "script-1" });
    expect(result.success).toBe(true);
  });

  it("rejects an empty string", () => {
    const result = ScriptIdSchema.safeParse({ value: "" });
    expect(result.success).toBe(false);
  });
});

describe("FileId", () => {
  it("accepts a non-empty value", () => {
    expect(FileIdSchema.safeParse({ value: "file-1" }).success).toBe(true);
  });

  it("rejects an empty string", () => {
    expect(FileIdSchema.safeParse({ value: "" }).success).toBe(false);
  });
});

describe("InstanceId", () => {
  it("accepts any string including empty", () => {
    expect(InstanceIdSchema.safeParse({ value: "" }).success).toBe(true);
    expect(InstanceIdSchema.safeParse({ value: "inst-1" }).success).toBe(true);
  });
});

describe("SampleCodeFile", () => {
  it("accepts a valid file", () => {
    expect(SampleCodeFileSchema.safeParse({ name: "Model.ts", content: "export {}" }).success).toBe(true);
  });

  it("rejects an empty name", () => {
    expect(SampleCodeFileSchema.safeParse({ name: "", content: "export {}" }).success).toBe(false);
  });
});

describe("AnalysisInfo", () => {
  it("parses a minimal valid analysis info", () => {
    const result = AnalysisInfoSchema.safeParse({
      id: { value: "an-1" },
      projectId: { value: "proj-1" },
      instanceId: { value: "inst-1" },
      scriptId: { value: "scr-1" },
      status: { value: "Started" },
      creationDate: new Date().toISOString(),
    });
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.results).toEqual([]);
      expect(result.data.errors).toEqual([]);
    }
  });
});

describe("ResultType", () => {
  it("has correct constant values", () => {
    expect(ResultType.FILE).toBe("File");
    expect(ResultType.CONSOLE).toBe("Console");
    expect(ResultType.RUN_ERROR).toBe("RunError");
  });
});

describe("PluginId", () => {
  it("stores name and version", () => {
    const result = PluginIdSchema.safeParse({ name: "my-plugin", version: "1.0.0" });
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.name).toBe("my-plugin");
      expect(result.data.version).toBe("1.0.0");
    }
  });
});

describe("PluginKind", () => {
  it("has correct constant values", () => {
    expect(PluginKind.LOADER).toBe("Loader");
    expect(PluginKind.LINKER).toBe("Linker");
    expect(PluginKind.UI).toBe("Ui");
  });
});

describe("ContextSlice", () => {
  it("parses model and pluginIds", () => {
    const result = ContextSliceSchema.safeParse({ model: "MyModel", pluginIds: ["p1"] });
    expect(result.success).toBe(true);
  });
});

describe("ContextGraphNode", () => {
  it("defaults properties to empty object", () => {
    const result = ContextGraphNodeSchema.safeParse({ id: "n1", label: "Node", type: "Class" });
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.properties).toEqual({});
    }
  });
});
