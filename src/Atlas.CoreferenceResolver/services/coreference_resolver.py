from generated.coreference_resolver_pb2_grpc import CoreferenceResolverServicer
import generated.coreference_resolver_pb2 as models

class CoreferenceResolver(CoreferenceResolverServicer):
    def __init__(self):
        pass

    def ResolveAntecedents(self, request, context):
        for i, token in enumerate(request.tokens):
            yield models.CorefCluster(
                mention = [models.CorefMention(token_index = i, start_mention = i + 1, end_mention = i + 2)],
                antecedent = models.CorefMention(token_index = 1, start_mention = 5, end_mention = 10))