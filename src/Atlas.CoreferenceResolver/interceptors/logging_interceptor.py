from grpc import ServerInterceptor
import logging

class LoggingInterceptor(ServerInterceptor):
    def __init__(self):
        pass

    def intercept_service(self, continuation, handler_call_details):
        logging.info(f"request received at {handler_call_details.method}")
        return continuation(handler_call_details)