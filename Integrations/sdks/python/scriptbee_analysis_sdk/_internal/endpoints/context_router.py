import io

from fastapi import APIRouter, Depends
from fastapi.responses import StreamingResponse

from scriptbee_analysis_sdk._internal.endpoints.contracts.web_models import (
    WebContextGraphEdge,
    WebContextGraphNode,
    WebContextGraphResponse,
    WebContextSlice,
    WebGenerateClassesRequest,
    WebGetContextResponse,
    WebLinkContextCommand,
    WebLoadContextCommand,
)
from scriptbee_analysis_sdk.abstractions.clear_context_use_case import ClearContextUseCase
from scriptbee_analysis_sdk.abstractions.generate_classes_use_case import GenerateClassesUseCase
from scriptbee_analysis_sdk.abstractions.get_context_graph_use_case import GetContextGraphUseCase
from scriptbee_analysis_sdk.abstractions.get_context_use_case import GetContextUseCase
from scriptbee_analysis_sdk.abstractions.link_context_use_case import LinkContextUseCase
from scriptbee_analysis_sdk.abstractions.load_context_use_case import LoadContextUseCase
from scriptbee_analysis_sdk.domain.project import FileId
from scriptbee_analysis_sdk.file_bundler import FileBundler


def make_context_router(
    get_context: callable,
    load_context: callable,
    link_context: callable,
    clear_context: callable,
    get_context_graph: callable,
    generate_classes: callable,
) -> APIRouter:
    router = APIRouter(tags=["Context"])

    @router.get(
        "/api/context",
        name="Context",
        summary="Get analysis context",
        description="Retrieves the current data context from the analysis service.",
    )
    def get_context_handler(
        use_case: GetContextUseCase = Depends(get_context),
    ) -> WebGetContextResponse:
        slices = use_case.get()
        return WebGetContextResponse(data=[WebContextSlice.from_context_slice(s) for s in slices])

    @router.post(
        "/api/context/load",
        status_code=204,
        name="Load",
        summary="Load data into analysis context",
        description="Loads data from files into the current analysis context using the provided loaders.",
    )
    async def load_context_handler(
        command: WebLoadContextCommand,
        use_case: LoadContextUseCase = Depends(load_context),
    ) -> None:
        files = {k: [FileId(value=f) for f in v] for k, v in command.files_to_load.items()}
        await use_case.load(files)

    @router.post(
        "/api/context/link",
        status_code=204,
        name="Link",
        summary="Link analysis context",
        description="Links the current analysis context using the provided linkers.",
    )
    async def link_context_handler(
        command: WebLinkContextCommand,
        use_case: LinkContextUseCase = Depends(link_context),
    ) -> None:
        await use_case.link(command.linker_ids)

    @router.post(
        "/api/context/clear",
        status_code=204,
        name="Clear",
        summary="Clear analysis context",
        description="Clears all data from the current analysis context.",
    )
    def clear_context_handler(
        use_case: ClearContextUseCase = Depends(clear_context),
    ) -> None:
        use_case.clear()

    @router.get(
        "/api/context/graph-nodes",
        name="GraphNodes",
        summary="Search context nodes",
        description="Searches for context nodes based on a query string.",
        tags=["Context", "Graph"],
    )
    def search_nodes(
        use_case: GetContextGraphUseCase = Depends(get_context_graph),
        query: str = "",
        offset: int = 0,
        limit: int = 10,
    ) -> WebContextGraphResponse:
        result = use_case.search_nodes(query, offset, limit)
        return WebContextGraphResponse(
            nodes=[WebContextGraphNode.from_node(n) for n in result.nodes],
            edges=[WebContextGraphEdge.from_edge(e) for e in result.edges],
        )

    @router.get(
        "/api/context/graph-nodes/{node_id}/neighbors",
        name="Neighbors",
        summary="Get node neighbors",
        description="Retrieves the immediate neighbors and edges for a specific context node.",
        tags=["Context", "Graph"],
    )
    def get_neighbors(
        node_id: str,
        use_case: GetContextGraphUseCase = Depends(get_context_graph),
    ) -> WebContextGraphResponse:
        result = use_case.get_neighbors(node_id)
        return WebContextGraphResponse(
            nodes=[WebContextGraphNode.from_node(n) for n in result.nodes],
            edges=[WebContextGraphEdge.from_edge(e) for e in result.edges],
        )

    @router.post(
        "/api/context/generate-classes",
        name="GenerateClasses",
        summary="Generate classes for analysis context",
        description="Generates script classes based on the current data context and returns them as a stream.",
    )
    async def generate_classes_handler(
        request: WebGenerateClassesRequest,
        use_case: GenerateClassesUseCase = Depends(generate_classes),
    ) -> StreamingResponse:
        languages = request.languages or []
        files = await use_case.generate_classes(languages)
        stream = io.BytesIO()
        FileBundler().write_to_stream(files, stream)
        stream.seek(0)
        return StreamingResponse(
            stream,
            media_type="application/octet-stream",
            headers={"Content-Disposition": 'attachment; filename="classes.bin"'},
        )

    return router
