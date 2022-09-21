using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.KeyVault;
using System.Reflection;

namespace ValidationService.Classes
{
    public class ImageValidationRule : BaseValidationRule
    {
        public ImageValidationRule(string imagePath)
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
            computerVision.Endpoint = "https://imagevalidationcognitiveservice.cognitiveservices.azure.com/";

            Console.WriteLine("Images being analyzed ...");
            var t2 = Helpers.AnalyzeLocalAsync(computerVision, this.Body);

            Task.WhenAll(t2).Wait(5000);
            return result;
        }
    }
}
