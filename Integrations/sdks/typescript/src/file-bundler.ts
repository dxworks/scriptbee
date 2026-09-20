import type { SampleCodeFile } from "./domain/code-generation.js";

export class FileBundler {
  writeToBuffer(files: SampleCodeFile[]): Uint8Array {
    const chunks: Uint8Array[] = [];
    const encoder = new TextEncoder();

    for (const file of files) {
      const pathBytes = encoder.encode(file.name);
      const contentBytes = encoder.encode(file.content);

      chunks.push(encodeBigEndianUint32(pathBytes.byteLength));
      chunks.push(pathBytes);
      chunks.push(encodeBigEndianUint64(contentBytes.byteLength));
      chunks.push(contentBytes);
    }

    chunks.push(encodeBigEndianUint32(0));

    return mergeChunks(chunks);
  }
}

function encodeBigEndianUint32(value: number): Uint8Array {
  const buf = new ArrayBuffer(4);
  new DataView(buf).setUint32(0, value, false);
  return new Uint8Array(buf);
}

function encodeBigEndianUint64(value: number): Uint8Array {
  const buf = new ArrayBuffer(8);
  new DataView(buf).setBigUint64(0, BigInt(value), false);
  return new Uint8Array(buf);
}

function mergeChunks(chunks: Uint8Array[]): Uint8Array {
  const totalLength = chunks.reduce((sum, chunk) => sum + chunk.byteLength, 0);
  const result = new Uint8Array(totalLength);
  let offset = 0;
  for (const chunk of chunks) {
    result.set(chunk, offset);
    offset += chunk.byteLength;
  }
  return result;
}
