from pydantic import BaseModel, field_validator


class SampleCodeFile(BaseModel):
    name: str
    content: str

    @field_validator("name")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v
