from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class Mention(_message.Message):
    __slots__ = ("start_char", "end_char", "text")
    START_CHAR_FIELD_NUMBER: _ClassVar[int]
    END_CHAR_FIELD_NUMBER: _ClassVar[int]
    TEXT_FIELD_NUMBER: _ClassVar[int]
    start_char: int
    end_char: int
    text: str
    def __init__(self, start_char: _Optional[int] = ..., end_char: _Optional[int] = ..., text: _Optional[str] = ...) -> None: ...
