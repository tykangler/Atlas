proto_location = ../../proto
install_args = 

run:
	python server.py

freeze:
	pip freeze > requirements.txt

build: install generate

install:
	pip install -r requirements.txt $(install_args)

generate:
	python -m grpc_tools.protoc \
	--proto_path $(proto_location) \
	--python_out=. \
	--grpc_python_out=. \
	--pyi_out=. \
	$(proto_location)/Narrator/Narrator.proto $(proto_location)/Narrator/DocumentRequest.proto $(proto_location)/Narrator/Phrase.proto $(proto_location)/Narrator/RelationshipResponse.proto
	python -m grpc_tools.protoc \
	--proto_path $(proto_location) \
	--python_out=. \
	--grpc_python_out=. \
	--pyi_out=. \
	$(proto_location)/CoreferenceResolver/CoreferenceResolver.proto $(proto_location)/CoreferenceResolver/CoreferenceRequest.proto $(proto_location)/CoreferenceResolver/CoreferenceResponse.proto $(proto_location)/CoreferenceResolver/CorefCluster.proto $(proto_location)/CoreferenceResolver/Mention.proto