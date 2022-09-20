namespace ValidationService.Classes
{
    using Newtonsoft.Json;
    using System.Reflection;

    public class GrammarValidationRule : BaseValidationRule
    {
        public GrammarValidationRule(string body)
        {
            this.Name = Assembly.GetExecutingAssembly()?.GetName()?.Name; 
            this.Body = body;
        }

        public override async Task<List<ValidationResult>> Validate()
        {
            var result = new List<ValidationResult>();
            string[] lines = this.Body.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int lineNum = 1; lineNum < lines.Length + 1; lineNum++)
            {
                result.AddRange(await this.CheckGrammar(lineNum, lines[lineNum - 1]));
            }

            return result;
        }

        private async Task<List<ValidationResult>> CheckGrammar(int lineNum, string line)
        {
            var output = new List<ValidationResult>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.languagetool.org/v2/check"),
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "text", line },
                        { "language", "auto" },
                    }),
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var op = JsonConvert.DeserializeObject<GrammarResponse>(body);
                if (op?.matches?.Length > 0)
                {
                    foreach (var match in op.matches)
                    {
                        output.Add(
                            new ValidationResult
                            {
                                LineNumber = lineNum,
                                Message = match.message,
                                Severity = ValidationSeverity.Error
                            });
                    }
                }
            }

            return output;
        }
    }
}
