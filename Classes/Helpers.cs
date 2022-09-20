namespace ValidationService.Classes
{
    public static class Helpers
    {
        public static List<BaseValidationRule> GetValidationRules(string body, ValidationType type)
        {
            var validationList = new List<BaseValidationRule>();
            switch (type)
            {
                case ValidationType.Text:
                    //validationList.Add(new GrammarValidationRule(body));
                    validationList.Add(new SpellingValidationRule(body));
                    break;
                default:
                    break;
            }

            return validationList;
        }
    }
}
