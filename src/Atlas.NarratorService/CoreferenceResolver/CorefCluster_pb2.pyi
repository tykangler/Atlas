from CoreferenceResolver import Mention_pb2 as _Mention_pb2
from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from collections.abc import Iterable as _Iterable, Mapping as _Mapping
from typing import ClassVar as _ClassVar, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class CorefCluster(_message.Message):
    __slots__ = ("mentions", "antecedent")
    MENTIONS_FIELD_NUMBER: _ClassVar[int]
    ANTECEDENT_FIELD_NUMBER: _ClassVar[int]
    mentions: _containers.RepeatedCompositeFieldContainer[_Mention_pb2.Mention]
    antecedent: _Mention_pb2.Mention
    def __init__(self, mentions: _Optional[_Iterable[_Union[_Mention_pb2.Mention, _Mapping]]] = ..., antecedent: _Optional[_Union[_Mention_pb2.Mention, _Mapping]] = ...) -> None: ...
