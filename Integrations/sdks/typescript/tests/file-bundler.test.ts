import { describe, expect, it } from "vitest";
import { FileBundler } from "../src/file-bundler.js";

describe("FileBundler", () => {
  it("produces a terminal uint32 zero for an empty file list", () => {
    const bundler = new FileBundler();
    const buf = bundler.writeToBuffer([]);

    expect(buf.byteLength).toBe(4);
    const view = new DataView(buf.buffer);
    expect(view.getUint32(0, false)).toBe(0);
  });

  it("encodes a single file with correct big-endian framing", () => {
    const bundler = new FileBundler();
    const buf = bundler.writeToBuffer([{ name: "Model.ts", content: "export {}" }]);

    const encoder = new TextEncoder();
    const pathBytes = encoder.encode("Model.ts");
    const contentBytes = encoder.encode("export {}");

    const view = new DataView(buf.buffer);
    let offset = 0;

    const pathLen = view.getUint32(offset, false);
    expect(pathLen).toBe(pathBytes.byteLength);
    offset += 4;

    const decodedPath = new TextDecoder().decode(buf.slice(offset, offset + pathLen));
    expect(decodedPath).toBe("Model.ts");
    offset += pathLen;

    const contentLen = Number(view.getBigUint64(offset, false));
    expect(contentLen).toBe(contentBytes.byteLength);
    offset += 8;

    const decodedContent = new TextDecoder().decode(buf.slice(offset, offset + contentLen));
    expect(decodedContent).toBe("export {}");
    offset += contentLen;

    const terminal = view.getUint32(offset, false);
    expect(terminal).toBe(0);
  });

  it("encodes multiple files sequentially", () => {
    const bundler = new FileBundler();
    const files = [
      { name: "A.ts", content: "a" },
      { name: "B.ts", content: "bb" },
    ];
    const buf = bundler.writeToBuffer(files);

    const view = new DataView(buf.buffer);
    let offset = 0;

    for (const file of files) {
      const encoder = new TextEncoder();
      const pathBytes = encoder.encode(file.name);
      const contentBytes = encoder.encode(file.content);

      expect(view.getUint32(offset, false)).toBe(pathBytes.byteLength);
      offset += 4 + pathBytes.byteLength;

      expect(Number(view.getBigUint64(offset, false))).toBe(contentBytes.byteLength);
      offset += 8 + contentBytes.byteLength;
    }

    expect(view.getUint32(offset, false)).toBe(0);
  });
});
