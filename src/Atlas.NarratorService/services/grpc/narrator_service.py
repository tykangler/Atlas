from typing import Iterator
import spacy
from spacy.tokens import Doc, Span, Token
from Narrator.Narrator_pb2_grpc import NarratorServicer
from Narrator.DocumentRequest_pb2 import DocumentRequest
from Narrator.RelationshipResponse_pb2 import RelationshipResponse

class NarratorService(NarratorServicer):
    def __init__(self):
        self.nlp = spacy.load("en_core_web_lg")

    def GetRelationships(self, request: DocumentRequest, context):
        for response in self.process_extracts([request]):
            yield response

    def process_extracts(self, requests: Iterator[DocumentRequest]):
        docs = self.nlp.pipe(ext.text for ext in requests)
        for (request, doc) in zip(requests, docs):
            for phrase in request.targetPhrases:
                target_span = self.get_phrase_span(doc, phrase.text)
                yield self.evaluate_relationships(target_span)

    def get_phrase_span(self, doc: Doc, phrase: str):
        index_found = doc.text.index(phrase)
        return doc.char_span(index_found, len(phrase) + index_found)

    def evaluate_relationships(self, span: Span):
        tokens = list(span.subtree)
        if span.root.pos_ == "VERB" or span.root.pos_ == "AUX":
            return RelationshipResponse(action=span.root.text, prep=None, detailed_action=self.convert_tokens_to_text(tokens), target=span.text)
        is_conj = span.root.dep_ == "conj"
        prep = span.root.text if span.root.pos_ == "ADP" else None
        for token in span.root.ancestors:
            if token.pos_ == "VERB" or token.pos_ == "AUX":
                tokens.insert(0, token)
                return RelationshipResponse(action=token.text, prep=prep, detailed_action=self.convert_tokens_to_text(tokens), target=span.text)
            elif is_conj:
                is_conj = False
            elif not is_conj:
                tokens.insert(0, token)
            
            if token.pos_ == "ADP" and prep == None:
                prep = token.text
            if token.dep_ == "conj":
                is_conj = True
        return None
    
    def convert_tokens_to_text(self, tokens: Iterator[Token]):
        return " ".join(token.text.strip() for token in tokens)