# ProtoBuf Format

## Structure

The project's pb files are located in this directory. Each subdirectory under the `proto` directory represents a GRPC service and the dependencies for that service. 
Files for each GRPC service should go under the same directory. In a sense, each pb file can be considered of consisting of a controller and several models.

The `proto` directory should contain only proto files, and any projects that need to use these pb files will clone the repo and generate the server and client files
as needed.

## Version Control

The `proto` directory will be a git submodule so that any other projects can clone from the submodule and generate server and client files as needed.

## Imports

Qualify any imports by the service name. i.e. `proto/Narrator/DocumentRequest.proto` has the service name `Narrator`, and imports should be `import "Narrator/Phrase.proto"`.
This way to compile the protobuf files for python (for example) we can run:
```pwsh
python -m grpc_tools.protoc `
    --proto_path ../../proto `
    --python_out=. `
    --grpc_python_out=. `
    --pyi_out=. `
    ../../proto/Narrator/Narrator.proto ../../proto/Narrator/DocumentRequest.proto ../../proto/Narrator/Phrase.proto ../../proto/Narrator/RelationshipResponse.proto
```

`--proto_path, --python_out, --grpc_python_out, --pyi_out` will be the same no matter what. And the only thing that will change are the proto files being compiled.