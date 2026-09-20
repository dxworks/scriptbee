import io
import struct

from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile
from scriptbee_analysis_sdk.file_bundler import FileBundler


def test_write_to_stream_encodes_files_correctly():
    files = [
        SampleCodeFile(name="file1.cs", content="content1"),
        SampleCodeFile(name="dir/file2.py", content="content2"),
    ]
    stream = io.BytesIO()

    FileBundler().write_to_stream(files, stream)
    stream.seek(0)

    for file in files:
        path_bytes = file.name.encode("utf-8")
        content_bytes = file.content.encode("utf-8")

        (path_len,) = struct.unpack(">I", stream.read(4))
        assert path_len == len(path_bytes)
        assert stream.read(path_len).decode("utf-8") == file.name

        (content_len,) = struct.unpack(">Q", stream.read(8))
        assert content_len == len(content_bytes)
        assert stream.read(content_len).decode("utf-8") == file.content

    (end,) = struct.unpack(">I", stream.read(4))
    assert end == 0
    assert stream.read() == b""


def test_write_to_stream_empty_list_writes_only_end_marker():
    stream = io.BytesIO()

    FileBundler().write_to_stream([], stream)
    stream.seek(0)

    data = stream.read()
    assert len(data) == 4
    (end,) = struct.unpack(">I", data)
    assert end == 0
