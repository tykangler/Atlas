# -------- BUILD & INSTALLATION STAGE --------------
FROM python:3.11.11-alpine3.20 as build

# install build dependencies
RUN apk add --no-cache make g++ musl-dev

# create a virtual env to install and build dependencies and copy it into the next stage
# https://pythonspeed.com/articles/activate-virtualenv-dockerfile/
# activating a python venv without the activate script
RUN python -m venv /venv
ENV PATH=/venv/bin:$PATH

# copy requirements.txt  into app dir and install dependencies
WORKDIR /app
COPY src/Atlas.NarratorService/Makefile .
COPY src/Atlas.NarratorService/requirements.txt .
RUN make install install_args=--no-cache-dir

# copy proto files and generate grpc server files in app dir
COPY proto/Narrator/ ./proto/Narrator
RUN make proto_location=./proto generate

# ----------- RUN STAGE --------------
# note the base image is the exact same as the build stage!
FROM python:3.11.11-alpine3.20 as run

# copy the venv and activate it. Note the paths must be the same!
COPY --from=build /venv /venv
ENV PATH=/venv/bin:$PATH

# copy app files and protos, and generate grpc files
WORKDIR /app
COPY --from=build /app .
COPY src/Atlas.NarratorService/ .

# entry point
EXPOSE 50051
ENTRYPOINT [ "python", "server.py" ]