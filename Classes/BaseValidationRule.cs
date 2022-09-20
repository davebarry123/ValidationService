namespace ValidationService.Classes
{
    public abstract class BaseValidationRule
    {
        public string Name { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public abstract List<ValidationResult> Validate();
    }

    public enum ValidationType
    {
        Text = 0,
        Image = 1,
        Video = 2
    }
}
