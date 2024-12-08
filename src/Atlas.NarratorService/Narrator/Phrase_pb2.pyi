from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class Phrase(_message.Message):
    __slots__ = ("startIndex", "endIndex", "text")
    STARTINDEX_FIELD_NUMBER: _ClassVar[int]
    ENDINDEX_FIELD_NUMBER: _ClassVar[int]
    TEXT_FIELD_NUMBER: _ClassVar[int]
    startIndex: int
    endIndex: int
    text: str
    def __init__(self, startIndex: _Optional[int] = ..., endIndex: _Optional[int] = ..., text: _Optional[str] = ...) -> None: ...
