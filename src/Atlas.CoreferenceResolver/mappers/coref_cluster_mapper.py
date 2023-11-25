from generated.coreference_resolver_pb2 import CorefMention


def map_to_cluster(spacy_pred):
    for mention in spacy_pred:
        pass