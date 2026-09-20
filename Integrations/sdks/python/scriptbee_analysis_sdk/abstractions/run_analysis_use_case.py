from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.abstractions.run_analysis_command import RunAnalysisCommand
from scriptbee_analysis_sdk.domain.analysis import AnalysisInfo


class RunAnalysisUseCase(ABC):
    @abstractmethod
    async def run(self, command: RunAnalysisCommand) -> AnalysisInfo: ...
