import functools
import logging
import time

def log_request(func):
    @functools.wraps(func)
    def logging_wrapper(self, request, context):
        logging.info(f"Received request {request}")
        start_time = time.perf_counter_ns()
        value = func(self, request, context)
        end_time = time.perf_counter_ns()
        logging.debug(f"Elapsed time: {end_time - start_time} ns")
        return value
    return logging_wrapper