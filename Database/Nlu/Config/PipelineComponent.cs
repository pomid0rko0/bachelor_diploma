/*
namespace Database.Nlu.Config
{
    public class PipelineComponent : object
    {
        public string name { get; set; }
    }
    public class NLP : PipelineComponent
    {
        [Required]
        public string model { get; set; }
    }
    public class Tokenizer : PipelineComponent
    {
        public bool? intent_tokenization_flag { get; set; } = false;
        public string intent_split_symbol { get; set; } = "_";
        public string token_pattern { get; set; } = null;
    }
    public class MitieNLP : NLP
    {
        public new const string name = "MitieNLP";
    }
    public class SpacyNLP : NLP
    {
        public new const string name = "SpacyNLP";
        public bool case_sensitive { get; set; } = false;
    }
    public class WhitespaceTokenizer : Tokenizer
    {
        public new const string name = "WhitespaceTokenizer";
    }
    public class MitieTokenizer : Tokenizer
    {
        public new const string name = "MitieTokenizer";
    }
    public class SpacyTokenizer : Tokenizer
    {
        public new const string name = "SpacyTokenizer";
    }
    public class MitieFeaturizer : PipelineComponent
    {
        public new const string name = "MitieFeaturizer";
        [RegularExpression("mean|max")]
        public string pooling { get; set; } = "mean";
    }
    public class SpacyFeaturizer : PipelineComponent
    {
        public new const string name = "SpacyFeaturizer";
        [RegularExpression("mean|max")]
        public string pooling { get; set; } = "mean";
    }
    public class ConveRTFeaturizer : PipelineComponent
    {
        public new const string name = "ConveRTFeaturizer";
    }
    public class LanguageModelFeaturizer : PipelineComponent
    {
        public new const string name = "LanguageModelFeaturizer";
        [RegularExpression("bert|gpt|gpt2|xlnet|distilbert|roberta")]
        public string model_name { get; set; } = "bert";
        [RegularExpression("rasa/LaBSE|openai-gpt|gpt2|xlnet-base-cased|distilbert-base-uncased|roberta-base")]
        public string model_weights { get; set; } = null;
        public string cache_dir { get; set; } = null;
    }
    public class RegexFeaturizer : PipelineComponent
    {
        public new const string name = "RegexFeaturizer";
        public bool case_sensitive { get; set; } = true;
        public bool use_word_boundaries { get; set; } = true;
        public int? number_additional_patterns { get; set; } = null;
    }
    public class CountVectorsFeaturizer : PipelineComponent
    {
        public class VocabularySize
        {
            public int text { get; set; } = 1000;
            public int response { get; set; } = 1000;
            public int action_text { get; set; } = 1000;
        }
        public new const string name = "CountVectorsFeaturizer";
        [RegularExpression("word|char|char_wb")]
        public string analyzer { get; set; } = "word";
        public int? min_ngram { get; set; } = 1;
        public int? max_ngram { get; set; } = 1;
        public string OOV_token { get; set; } = null;
        public string OOV_words { get; set; } = null;
        public bool use_shared_vocab { get; set; } = false;
        public VocabularySize additional_vocabulary_size { get; set; } = null;
    }
    public class LexicalSyntacticFeaturizer : PipelineComponent
    {
        public new const string name = "LexicalSyntacticFeaturizer";
        [MinLength(3), MaxLength(3)]
        public ICollection<ICollection<string>> features { get; set; } = null;
    }
    public class MitieIntentClassifier : PipelineComponent
    {
        public new const string name = "MitieIntentClassifier";
    }
    public class SklearnIntentClassifier : PipelineComponent
    {
        public new const string name = "SklearnIntentClassifier";
        public ICollection<double> C { get; set; }
        public ICollection<string> kernels { get; set; }
        public ICollection<double> gamma { get; set; }
        public int? max_cross_validation_folds { get; set; }
        public string scoring_function { get; set; }
    }
    public class KeywordIntentClassifier : PipelineComponent
    {
        public new const string name = "KeywordIntentClassifier";
        public bool? case_sensitive { get; set; }
    }
    public class DIETClassifier : PipelineComponent
    {
        public class LayerSize
        {
            public ICollection<int> text { get; set; }
            public ICollection<int> label { get; set; }
        }
        public new const string name = "DIETClassifier";
        public int epochs { get; set; } = 300;
        public LayerSize hidden_layers_sizes { get; set; } = new LayerSize { text = new List<int>(), label = new List<int>() };
        public int embedding_dimension { get; set; } = 20;
        public int number_of_transformer_layers { get; set; } = 2;
        public int transformer_size { get; set; } = 256;
        [Range(0.0, 1.0)]
        public double weight_sparsity { get; set; } = 0.8;
        public bool constrain_similarities { get; set; } = true;
        public bool entity_recognition { get; set; } = true;
        [RegularExpression("linear_norm|softmax")]
        public string model_confidence { get; set; } = "linear_norm";
    }
    public class FallbackClassifier : PipelineComponent
    {
        public new const string name = "FallbackClassifier";
        public double? threshold { get; set; }
        public double? ambiguity_threshold { get; set; }
    }
    public class MitieEntityExtractor : PipelineComponent
    {
        public new const string name = "MitieEntityExtractor";
    }
    public class SpacyEntityExtractor : PipelineComponent
    {
        public new const string name = "SpacyEntityExtractor";
        public ICollection<string> dimensions { get; set; }
    }
    public class CRFEntityExtractor : PipelineComponent
    {
        public new const string name = "CRFEntityExtractor";
        public bool? BILOU_flag { get; set; }
        public ICollection<ICollection<string>> features { get; set; }
        public int? max_iterations { get; set; }
        public double? L1_c { get; set; }
        public double? L2_c { get; set; }
        public ICollection<string> featurizers { get; set; }
        public IDictionary<string, bool> split_entities_by_comma { get; set; }
    }
    public class DucklingEntityExtractor : PipelineComponent
    {
        public new const string name = "DucklingEntityExtractor";
        public string url { get; set; }
        public ICollection<string> dimensions { get; set; }
        public string locale { get; set; }
        public string timezone { get; set; }
        public int? timeout { get; set; }
    }
    public class RegexEntityExtractor : PipelineComponent
    {
        public new const string name = "RegexEntityExtractor";
        public bool? case_sensitive { get; set; } = false;
        public bool? use_lookup_tables { get; set; } = true;
        public bool? use_regexes { get; set; } = true;
        public bool? use_word_boundaries { get; set; } = true;
    }
    public class EntitySynonymMapper : PipelineComponent
    {
        public new const string name = "EntitySynonymMapper";
    }
}
*/
