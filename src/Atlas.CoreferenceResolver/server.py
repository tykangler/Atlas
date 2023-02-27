from concurrent import futures
import services.coreference_resolver
import generated.coreference_resolver_pb2_grpc
import grpc
import logging
from interceptors.logging_interceptor import LoggingInterceptor

ADDRESS = "localhost:50051"

def serve():
    server = grpc.server(
        thread_pool=futures.ThreadPoolExecutor(max_workers=10),
        interceptors=[LoggingInterceptor()])
    generated.coreference_resolver_pb2_grpc.add_CoreferenceResolverServicer_to_server(
        servicer=services.coreference_resolver.CoreferenceResolver(),
        server=server
    )
    server.add_insecure_port(ADDRESS)
    server.start()
    server.wait_for_termination()

if __name__ == "__main__":
    logging.basicConfig(encoding="utf-8", level=logging.DEBUG)
    logging.info(f"Starting server at {ADDRESS}")
    serve()