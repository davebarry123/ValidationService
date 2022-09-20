namespace ValidationService.Classes
{
    using Microsoft.Office.Interop.Word;
    using System.Reflection;

    public class GrammarValidationRule : BaseValidationRule
    {
        public GrammarValidationRule(string body)
        {
            this.Name = Assembly.GetExecutingAssembly().GetName().Name; 
            this.Body = body;
        }

        public override List<ValidationResult> Validate()
        {
            var result = new List<ValidationResult>();
            Application myWord = new Application();
            
            if (!myWord.CheckGrammar(this.Body))
            {
                result.Add(
                    new ValidationResult 
                    { 
                        LineNumber = 1, 
                        Message = $"Invalid grammar {this.Body}", 
                        Severity = ValidationSeverity.Warning 
                    });
            }

            return result;
        }
    }
}
