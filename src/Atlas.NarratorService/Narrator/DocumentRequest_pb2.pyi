from Narrator import Phrase_pb2 as _Phrase_pb2
from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class DocumentRequest(_message.Message):
    __slots__ = ("text", "targetPhrases")
    TEXT_FIELD_NUMBER: _ClassVar[int]
    TARGETPHRASES_FIELD_NUMBER: _ClassVar[int]
    text: str
    targetPhrases: _containers.RepeatedCompositeFieldContainer[_Phrase_pb2.Phrase]
    def __init__(self, text: _Optional[str] = ..., targetPhrases: _Optional[_Iterable[_Union[_Phrase_pb2.Phrase, _Mapping]]] = ...) -> None: ...
