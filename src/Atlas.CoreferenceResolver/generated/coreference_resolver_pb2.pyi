from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class CorefCluster(_message.Message):
    __slots__ = ["antecedent", "mention"]
    ANTECEDENT_FIELD_NUMBER: _ClassVar[int]
    MENTION_FIELD_NUMBER: _ClassVar[int]
    antecedent: CorefMention
    mention: _containers.RepeatedCompositeFieldContainer[CorefMention]
    def __init__(self, mention: _Optional[_Iterable[_Union[CorefMention, _Mapping]]] = ..., antecedent: _Optional[_Union[CorefMention, _Mapping]] = ...) -> None: ...

class CorefMention(_message.Message):
    __slots__ = ["end_mention", "start_mention", "token_index"]
    END_MENTION_FIELD_NUMBER: _ClassVar[int]
    START_MENTION_FIELD_NUMBER: _ClassVar[int]
    TOKEN_INDEX_FIELD_NUMBER: _ClassVar[int]
    end_mention: int
    start_mention: int
    token_index: int
    def __init__(self, token_index: _Optional[int] = ..., start_mention: _Optional[int] = ..., end_mention: _Optional[int] = ...) -> None: ...

class TokenRequest(_message.Message):
    __slots__ = ["tokens"]
    TOKENS_FIELD_NUMBER: _ClassVar[int]
    tokens: _containers.RepeatedScalarFieldContainer[str]
    def __init__(self, tokens: _Optional[_Iterable[str]] = ...) -> None: ...
