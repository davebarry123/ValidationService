using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.KeyVault;
using System.Reflection;

namespace ValidationService.Classes
{
    public class ImageNsfwValidationRule : BaseValidationRule
    {
        public ImageNsfwValidationRule(string imagePath)
        {
            this.Name = Assembly.GetExecutingAssembly()?.GetName()?.Name;
            this.Body = imagePath;
        }

        public override async Task<List<ValidationResult>> Validate()
        {
            var result = new List<ValidationResult>();

            string keyVaultName = Helpers.GetConfigurationSetting("KeyVaultName");
            string secretName = Helpers.GetConfigurationSetting("KvPrimaryKeyName");

            var kv = new KeyVaultClient(Helpers.GetAccessToken);
            var cognitiveServicesKey = kv.GetSecretAsync("https://" + keyVaultName + ".vault.azure.net", secretName).GetAwaiter().GetResult();

            ComputerVisionClient computerVision = new ComputerVisionClient(
                            new ApiKeyServiceClientCredentials(cognitiveServicesKey.Value),
                            new DelegatingHandler[] { });
            computerVision.Endpoint = Helpers.GetConfigurationSetting("ComputerVisionEndpoint");

            Console.WriteLine("Images being analyzed for NSFW content...");
            bool imageIsLocal = Helpers.IsFileLocal(this.Body);
            bool imageIsRemote = Helpers.IsFileRemote(this.Body);

            if (!imageIsLocal && !imageIsRemote)
            {
                return new List<ValidationResult>
                {
                    new ValidationResult
                    {
                        LineNumber = 0,
                        Message = $"Image {this.Body} cannot be found, please provide a valid local or remote path",
                        Severity = ValidationSeverity.Error
                    }
                };
            }

            var validationResult = imageIsLocal ? 
                await Helpers.IsNsfwLocalAsync(computerVision, this.Body) : 
                await Helpers.IsNsfwRemoteAsync(computerVision, this.Body);
            result.Add(
                new ValidationResult
                {
                    LineNumber = 0,
                    Message = $"Image {this.Body} is {(validationResult.Item1 ? "not " : string.Empty)}suitable for uploading based on content type. " +
                    $"Image is racy: {validationResult.Item2.IsRacy}, gory: {validationResult.Item2.IsGory}, " +
                    $"adult: {validationResult.Item2.IsAdult}",
                    Severity = validationResult.Item1 ? ValidationSeverity.Error : ValidationSeverity.Info
                });

            return result;
        }
    }
}
