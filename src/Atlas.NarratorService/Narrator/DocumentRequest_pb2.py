# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# NO CHECKED-IN PROTOBUF GENCODE
# source: Narrator/DocumentRequest.proto
# Protobuf Python Version: 5.28.1
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import runtime_version as _runtime_version
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
_runtime_version.ValidateProtobufRuntimeVersion(
    _runtime_version.Domain.PUBLIC,
    5,
    28,
    1,
    '',
    'Narrator/DocumentRequest.proto'
)
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from Narrator import Phrase_pb2 as Narrator_dot_Phrase__pb2


DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x1eNarrator/DocumentRequest.proto\x12\x0e\x41tlas.Narrator\x1a\x15Narrator/Phrase.proto\"N\n\x0f\x44ocumentRequest\x12\x0c\n\x04text\x18\x01 \x01(\t\x12-\n\rtargetPhrases\x18\x02 \x03(\x0b\x32\x16.Atlas.Narrator.PhraseB\x1f\xaa\x02\x1c\x41tlas.Core.Clients.Generatedb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'Narrator.DocumentRequest_pb2', _globals)
if not _descriptor._USE_C_DESCRIPTORS:
  _globals['DESCRIPTOR']._loaded_options = None
  _globals['DESCRIPTOR']._serialized_options = b'\252\002\034Atlas.Core.Clients.Generated'
  _globals['_DOCUMENTREQUEST']._serialized_start=73
  _globals['_DOCUMENTREQUEST']._serialized_end=151
# @@protoc_insertion_point(module_scope)