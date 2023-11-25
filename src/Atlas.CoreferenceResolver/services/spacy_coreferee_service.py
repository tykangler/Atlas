import spacy
import coreferee
import logging

class SpacyCorefereeService:
    MODEL = "en_core_web_sm"

    def __init__(self):
        self.pipeline = spacy.load(self.MODEL)
        self.pipeline.add_pipe('coreferee')

    def process(self, document):
        prediction = self.pipeline(document)
        logging.info(prediction._.coref_chains)
        return prediction._.coref_chains
