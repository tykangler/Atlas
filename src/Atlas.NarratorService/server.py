import sys
import grpc
import logging
from concurrent.futures import ThreadPoolExecutor
import Narrator.Narrator_pb2_grpc as Narrator_pb2_grpc
import CoreferenceResolver.CoreferenceResolver_pb2_grpc as CoreferenceResolver_pb2_grpc
from services.grpc.narrator_service import NarratorService
from services.grpc.coreference_service import CoreferenceService

PORT = 50051
HOST = "[::]"

def serve():
    server = grpc.server(ThreadPoolExecutor(max_workers=10))
    Narrator_pb2_grpc.add_NarratorServicer_to_server(NarratorService(), server)
    CoreferenceResolver_pb2_grpc.add_CoreferenceResolverServicer_to_server(CoreferenceService(), server)
    server.add_insecure_port(f"{HOST}:{PORT}") # set up ssl
    logging.info(f"Now listening on {HOST}:{PORT}")
    server.start()
    server.wait_for_termination()
    
if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    serve()