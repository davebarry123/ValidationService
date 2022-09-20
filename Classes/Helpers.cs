using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ValidationService.Classes
{
    public static class Helpers
    {
        // Specify the features to return  
        private static readonly IList<VisualFeatureTypes?> features =
            new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
        };

        public static List<BaseValidationRule> GetValidationRules(string body, ValidationType type)
        {
            var validationList = new List<BaseValidationRule>();
            switch (type)
            {
                case ValidationType.Text:
                    validationList.Add(new GrammarValidationRule(body));
                    validationList.Add(new SpellingValidationRule(body));
                    break;

                case ValidationType.Image:
                    validationList.Add(new ImageValidationRule(body));
                    break;

                default:
                    break;
            }

            return validationList;
        }

        public static async Task<string> GetAccessToken(string azureTenantId, string clientId, string appSecret)
        {
            var context = new AuthenticationContext("https://login.windows.net/" + Helpers.GetConfigurationSetting("TenantId"));
            var credential = new ClientCredential(Helpers.GetConfigurationSetting("ClientId"), Helpers.GetConfigurationSetting("ClientSecret"));
            var tokenResult = await context.AcquireTokenAsync("https://vault.azure.net", credential);
            return tokenResult.AccessToken;
        }

        public static string GetConfigurationSetting(string configurationName)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            return config[configurationName];
        }

        // Analyze a local image  
        public static async Task AnalyzeLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(
                    imageStream, features);
                DisplayResults(analysis, imagePath);
            }
        }

        // Display the most relevant caption for the image  
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);
            if (analysis.Description.Captions.Count != 0)
            {
                Console.WriteLine(analysis.Description.Captions[0].Text + "\n");
            }
            else
            {
                Console.WriteLine("No description generated.");
            }
        }
    }
}
