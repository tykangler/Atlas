from concurrent import futures
import services.coreference_resolver
import generated.coreference_resolver_pb2_grpc
import grpc
import logging

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    generated.coreference_resolver_pb2_grpc.add_CoreferenceResolverServicer_to_server(
        servicer = services.coreference_resolver.CoreferenceResolver(), 
        server = server
    )
    server.add_insecure_port("localhost:50051")
    server.start()
    server.wait_for_termination()

if __name__ == "__main__":
    logging.basicConfig()
    serve()