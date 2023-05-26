import os
import re
import jamspell
from typing import Dict, Text, Any, List

from rasa.engine.graph import GraphComponent, ExecutionContext
from rasa.engine.recipes.default_recipe import DefaultV1Recipe
from rasa.engine.storage.resource import Resource
from rasa.engine.storage.storage import ModelStorage
from rasa.shared.nlu.training_data.message import Message
from rasa.shared.nlu.training_data.training_data import TrainingData


@DefaultV1Recipe.register(
    [DefaultV1Recipe.ComponentType.MESSAGE_TOKENIZER], is_trainable=False
)
class TextPreprocessor(GraphComponent):
    uncommons_chars_regex: re.Pattern = None
    spaces_regex: re.Pattern = None
    corrector: jamspell.TSpellCorrector = None

    @staticmethod
    def required_packages() -> List[Text]:
        return ["jamspell"]

    def __init__(self) -> None:
        super().__init__()
        self.uncommons_chars_regex = re.compile(r'[^а-я\d\s]')
        self.spaces_regex = re.compile(r'\s+')
        JAMSPELL_FOLDER = os.environ["JAMSPELL_FOLDER"]
        SPELLING_MODEL_FILE = os.environ["SPELLING_MODEL_FILE"]
        model_file_path = os.path.join(JAMSPELL_FOLDER, SPELLING_MODEL_FILE)
        self.corrector = jamspell.TSpellCorrector()
        self.corrector.LoadLangModel(model_file_path)

    @classmethod
    def create(
        cls,
        config: Dict[Text, Any],
        model_storage: ModelStorage,
        resource: Resource,
        execution_context: ExecutionContext,
    ):
        return cls()

    def process_training_data(self, training_data: TrainingData) -> TrainingData:
        self._process_messages(training_data.nlu_examples)
        return training_data

    def process(self, messages: List[Message]) -> List[Message]:
        self._process_messages(messages)
        return messages
    
    def _process_messages(self, messages: List[Message]):
        for message in messages:
            if 'text' in message.data.keys(): 
                text = str(message.data['text'])
                text = text.lower().replace("ё", "е")
                text = self.uncommons_chars_regex.sub(' ', text.strip())
                text = self.spaces_regex.sub(' ', text.strip())
                message.data['text'] = self.corrector.FixFragment(text.strip())
