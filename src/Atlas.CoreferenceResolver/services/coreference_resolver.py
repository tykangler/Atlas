from generated.coreference_resolver_pb2_grpc import CoreferenceResolverServicer
import generated.coreference_resolver_pb2 as models
from decorators.logging_decorator import log_request
from services.spacy_coreferee_service import SpacyCorefereeService

# setup spacy + coreferee pipeline as singleton shared reference.
# pass into service then?
# in here, run pipeline(request)

# this was a nice experiment. But I don't think we need AI, let's just a simple scoring system.

class CoreferenceResolver(CoreferenceResolverServicer):
    def __init__(self):
        self.spacy_coreferee_service = SpacyCorefereeService()
    
    @log_request
    def ResolveAntecedents(self, request, context):
        yield models.CorefCluster(
            mentions=self.spacy_coreferee_service.process(' '.join(request.tokens)),
            antecedent=None
        )