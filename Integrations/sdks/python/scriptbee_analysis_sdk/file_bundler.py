import struct
from io import RawIOBase

from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile


class FileBundler:
    def write_to_stream(self, files: list[SampleCodeFile], stream: RawIOBase) -> None:
        for file in files:
            path_bytes = file.name.encode("utf-8")
            content_bytes = file.content.encode("utf-8")
            stream.write(struct.pack(">I", len(path_bytes)))
            stream.write(path_bytes)
            stream.write(struct.pack(">Q", len(content_bytes)))
            stream.write(content_bytes)
        stream.write(struct.pack(">I", 0))
