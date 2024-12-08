from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class RelationshipResponse(_message.Message):
    __slots__ = ("action", "prep", "detailed_action", "target")
    ACTION_FIELD_NUMBER: _ClassVar[int]
    PREP_FIELD_NUMBER: _ClassVar[int]
    DETAILED_ACTION_FIELD_NUMBER: _ClassVar[int]
    TARGET_FIELD_NUMBER: _ClassVar[int]
    action: str
    prep: str
    detailed_action: str
    target: str
    def __init__(self, action: _Optional[str] = ..., prep: _Optional[str] = ..., detailed_action: _Optional[str] = ..., target: _Optional[str] = ...) -> None: ...
