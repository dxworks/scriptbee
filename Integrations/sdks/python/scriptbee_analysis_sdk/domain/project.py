from pydantic import BaseModel, field_validator


class ProjectId(BaseModel):
    value: str

    @field_validator("value")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v

    def __str__(self) -> str:
        return self.value


class ScriptId(BaseModel):
    value: str

    @field_validator("value")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v

    def __str__(self) -> str:
        return self.value


class FileId(BaseModel):
    value: str

    @field_validator("value")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v

    def __str__(self) -> str:
        return self.value


class InstanceId(BaseModel):
    value: str

    def __str__(self) -> str:
        return self.value
