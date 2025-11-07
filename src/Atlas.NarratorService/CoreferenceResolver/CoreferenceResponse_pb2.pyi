from CoreferenceResolver import CorefCluster_pb2 as _CorefCluster_pb2
from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from collections.abc import Iterable as _Iterable, Mapping as _Mapping
from typing import ClassVar as _ClassVar, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class CoreferenceResponse(_message.Message):
    __slots__ = ("clusters",)
    CLUSTERS_FIELD_NUMBER: _ClassVar[int]
    clusters: _containers.RepeatedCompositeFieldContainer[_CorefCluster_pb2.CorefCluster]
    def __init__(self, clusters: _Optional[_Iterable[_Union[_CorefCluster_pb2.CorefCluster, _Mapping]]] = ...) -> None: ...
