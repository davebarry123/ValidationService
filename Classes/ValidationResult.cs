namespace ValidationService.Classes
{
    public class ValidationResult
    {
        public int LineNumber { get; set; }

        public string Message { get; set; }

        public ValidationSeverity Severity { get; set; }
    }

    public enum ValidationSeverity
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }
}
