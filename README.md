# Atlas

## Setup Instructions

### Using Podman/Docker

To build the narrator service image:
```pwsh
podman build -f containers/Narrator.Dockerfile -t narrator-service .
```

> Note the build context is set to `.`, which references the current directory, the project root. And we set the dockerfile to `containers/Narrator.Dockerfile`.
> The Dockerfile is written so that the build context is relative to the project root. (i.e. `COPY src/Atlas.NarratorService/requirements.txt .`).
> Just be sure the build context is set to project root.

To run the narrator service:
```pwsh
podman run -p 50051:50051 narrator-service
```

### Manual

**Prerequisites**:
* Make
* Python 3.11.11

1. Create a python virtual env using the following command and using **python 3.11.11**. Note the `{ENV_DIR}` doesn't matter.:
```pwsh
python -m venv {ENV_DIR}
```

2. Activate the virtual env:
```pwsh
./{ENV_DIR}/scripts/activate
```

3. `cd` into `Atlas.NarratorService`:
```pwsh
cd src/Atlas.NarratorService
```

3. Install requirements with:
```pwsh
make setup
```

4. Generate GRPC files with:
```pwsh
make generate
```

5. Run the GRPC NarratorService server with:
```pwsh
python server.py
```


