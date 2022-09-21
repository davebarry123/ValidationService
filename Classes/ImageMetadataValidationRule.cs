using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.KeyVault;
using System.Reflection;

namespace ValidationService.Classes
{
    public class ImageMetadataValidationRule : BaseValidationRule
    {
        public ImageMetadataValidationRule(string imagePath)
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

            Console.WriteLine("Images being analyzed for metadata attributes...");
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
                await Helpers.IsValidLocalAsync(computerVision, this.Body) :
                await Helpers.IsValidRemoteAsync(computerVision, this.Body);
            result.Add(
                new ValidationResult
                {
                    LineNumber = 0,
                    Message = $"Image {this.Body} is {(validationResult.Item1 ? string.Empty : "not ")}suitable for uploading based on metadata. " +
                                $"Image format: {validationResult.Item2.Format}, height {validationResult.Item2.Height}, " +
                                $"width {validationResult.Item2.Width}",
                    Severity = validationResult.Item1 ? ValidationSeverity.Info : ValidationSeverity.Error
                });

            return result;
        }
    }
}
