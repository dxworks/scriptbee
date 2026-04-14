# Generate Model Classes Streaming Protocol Specification

> [!NOTE]
> This is used by the VS Code extension to generate the model classes

## Protocol Overview

The protocol follows a strictly sequential structure. Data is transmitted as a contiguous stream of **File Blocks** and
concludes with a specific **End of Stream (EOS)** marker.

To ensure cross-platform compatibility and adherence to standard networking practices, all numeric headers are encoded
in **Big Endian** (Network Byte Order).

## Binary Format

### 1. File Block Structure

Each file in the stream is defined by four distinct segments transmitted in the following order:

| Segment            | Size      | Type       | Description                                      |
| :----------------- | :-------- | :--------- | :----------------------------------------------- |
| **Path Length**    | 4 bytes   | Int32 (BE) | The length ($N$) of the UTF-8 encoded file path. |
| **Path Data**      | $N$ bytes | String     | The relative destination path of the file.       |
| **Content Length** | 8 bytes   | Int64 (BE) | The total size ($M$) of the file payload.        |
| **Content Data**   | $M$ bytes | Binary     | The raw binary data of the file.                 |

### 2. End of Stream (EOS)

The protocol uses a sentinel value to signal the end of the transmission. A stream is considered complete when the
consumer encounters a **Path Length** of `0` (`0x00 00 00 00`).

Since a valid file must have a path, a zero-length path serves as an unambiguous termination signal, allowing the client
to close the connection and finalize operations.

## Design Principles

### Endianness & Data Types

- **Big Endian:** All length prefixes must be written and read as Big Endian. Systems using Little Endian must perform
  byte swapping.
- **Signed Integers:** Lengths are represented as signed integers (Int32 for paths, Int64 for content) to maintain broad
  compatibility with standard library stream readers.
- **UTF-8 Encoding:** All file paths must be UTF-8 encoded without a Byte Order Mark (BOM).

## Processing Logic

A compliant consumer should implement the following logic:

1. **Read Header:** Extract the first 4 bytes.
2. **Check for Termination:** \* If value is `0`: Terminate processing.
   - If value is `> 0`: Continue to Step 3.
3. **Extract Metadata:** Read $N$ bytes for the path, then 8 bytes for the content length ($M$).
4. **Process Payload:** Read exactly $M$ bytes. These bytes represent the file content and can be streamed directly to a
   destination.
5. **Repeat:** Return to Step 1 to look for the next block.
