from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile


class GenerateClassesUseCase(ABC):
    @abstractmethod
    async def generate_classes(self, languages: list[str]) -> list[SampleCodeFile]: ...
