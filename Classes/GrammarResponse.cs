namespace ValidationService.Classes
{

    public class GrammarResponse
    {
        public Software software { get; set; }
        public Warnings warnings { get; set; }
        public Language language { get; set; }
        public Match[] matches { get; set; }
        public int[][] sentenceRanges { get; set; }
    }

    public class Software
    {
        public string name { get; set; }
        public string version { get; set; }
        public string buildDate { get; set; }
        public int apiVersion { get; set; }
        public bool premium { get; set; }
        public string premiumHint { get; set; }
        public string status { get; set; }
    }

    public class Warnings
    {
        public bool incompleteResults { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
        public string code { get; set; }
        public Detectedlanguage detectedLanguage { get; set; }
    }

    public class Detectedlanguage
    {
        public string name { get; set; }
        public string code { get; set; }
        public float confidence { get; set; }
        public string source { get; set; }
    }

    public class Match
    {
        public string message { get; set; }
        public string shortMessage { get; set; }
        public Replacement[] replacements { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
        public Context context { get; set; }
        public string sentence { get; set; }
        public Type type { get; set; }
        public Rule rule { get; set; }
        public bool ignoreForIncompleteSentence { get; set; }
        public int contextForSureMatch { get; set; }
    }

    public class Context
    {
        public string text { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
    }

    public class Type
    {
        public string typeName { get; set; }
    }

    public class Rule
    {
        public string id { get; set; }
        public string subId { get; set; }
        public string sourceFile { get; set; }
        public string description { get; set; }
        public string issueType { get; set; }
        public Category category { get; set; }
        public bool isPremium { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Replacement
    {
        public string value { get; set; }
        public string shortDescription { get; set; }
    }

}
