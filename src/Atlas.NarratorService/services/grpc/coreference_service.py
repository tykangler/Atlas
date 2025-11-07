import spacy
import coreferee
from CoreferenceResolver.CoreferenceResolver_pb2_grpc import CoreferenceResolverServicer
from CoreferenceResolver.CoreferenceRequest_pb2 import CoreferenceRequest
from CoreferenceResolver.CoreferenceResponse_pb2 import CoreferenceResponse
from CoreferenceResolver.CorefCluster_pb2 import CorefCluster
from CoreferenceResolver.Mention_pb2 import Mention

class CoreferenceService(CoreferenceResolverServicer):
    """
    gRPC service for resolving coreferences in text using spaCy and coreferee.

    This service identifies mentions in text that refer to the same entity,
    returning clusters of coreferent mentions with character-based positions.
    """

    def __init__(self):
        """Initialize the spaCy model with coreferee extension."""
        self.nlp = spacy.load("en_core_web_lg")
        self.nlp.add_pipe("coreferee")

    def ResolveCoreferences(self, request: CoreferenceRequest, context):
        """
        Resolve coreferences in the provided text.

        Args:
            request: CoreferenceRequest containing the text to analyze
            context: gRPC context

        Returns:
            CoreferenceResponse containing clusters of coreferent mentions
        """
        # Process the text with spaCy + coreferee
        doc = self.nlp(request.text)

        # Extract coreference clusters
        clusters = []

        if doc._.coref_chains:
            for chain in doc._.coref_chains:
                mentions = []
                antecedent = None

                # Process each mention in the chain
                for i, mention_indices in enumerate(chain):
                    # Get the span for this mention
                    mention_span = doc[mention_indices[0]:mention_indices[-1] + 1]

                    # Create a Mention message with character positions
                    mention = Mention(
                        start_char=mention_span.start_char,
                        end_char=mention_span.end_char,
                        text=mention_span.text
                    )

                    mentions.append(mention)

                    # First mention is typically the antecedent
                    if i == 0:
                        antecedent = mention

                # Create a CorefCluster with all mentions and the antecedent
                if antecedent and mentions:
                    cluster = CorefCluster(
                        mentions=mentions,
                        antecedent=antecedent
                    )
                    clusters.append(cluster)

        return CoreferenceResponse(clusters=clusters)
