namespace ValidationService.Classes
{
    using System.Reflection;
    using WeCantSpell.Hunspell;

    public class SpellingValidationRule : BaseValidationRule
    {
        public SpellingValidationRule(string body)
        {
            this.Name = Assembly.GetExecutingAssembly()?.GetName()?.Name;
            this.Body = body;
        }

        public override async Task<List<ValidationResult>> Validate()
        {
            var result = new List<ValidationResult>();
            var spell = await WordList.CreateFromFilesAsync("Dictionaries/English (American).dic");
            string[] lines = this.Body.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int lineNum = 1; lineNum < lines.Length + 1; lineNum++)
            {
                string? line = lines[lineNum - 1];
                var words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (!spell.Check(word))
                    {
                        result.Add(
                            new ValidationResult 
                            { 
                                LineNumber = lineNum, 
                                Message = $"Invalid spelling {word}, suggested spelling(s) {string.Join(", ", spell.Suggest(word))}",
                                Severity = ValidationSeverity.Error
                            });
                    }
                }
            }

            return result;
        }
    }
}
