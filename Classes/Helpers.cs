using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ValidationService.Classes
{
    public static class Helpers
    {
        // specify maximum image height
        private const int MaxHeight = 500;

        // specify maximum image width
        private const int MaxWidth = 500;

        // Specify the features to return  
        private static readonly IList<VisualFeatureTypes?> features =
            new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
            };

        // Specify valid image formats
        private static readonly IList<string> imageFormats =
            new List<string>()
            {
                "jpeg", "gif", "png", "jpg", "tiff", "tif", "bmp"
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
                    validationList.Add(new ImageMetadataValidationRule(body));
                    validationList.Add(new ImageNsfwValidationRule(body));
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

        public static bool IsFileLocal(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(
                    $"\nUnable to open or read local file path:\n{filePath} \n");
                return false;
            }

            return true;
        }

        public static bool IsFileRemote(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    $"\nUnable to open or read remote file path:\n{imageUrl} \n");
                return false;
            }

            return true;
        }

        // Analyze a local image for NSFW content  
        public static async Task<Tuple<bool, ImageMetadata>> IsNsfwLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = 
                    await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
                return
                    Tuple.Create(
                        analysis.Adult.IsAdultContent || analysis.Adult.IsRacyContent ||
                        analysis.Adult.IsGoryContent,
                        new ImageMetadata
                        {
                            IsAdult = analysis.Adult.IsAdultContent,
                            IsRacy = analysis.Adult.IsRacyContent,
                            IsGory = analysis.Adult.IsGoryContent,
                        });
            }
        }

        // Analyze a remote image for NSFW content
        public static async Task<Tuple<bool, ImageMetadata>> IsNsfwRemoteAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imagePath, features);
            return
                Tuple.Create(
                    analysis.Adult.IsAdultContent || analysis.Adult.IsRacyContent ||
                    analysis.Adult.IsGoryContent,
                    new ImageMetadata
                    {
                        IsAdult = analysis.Adult.IsAdultContent,
                        IsRacy = analysis.Adult.IsRacyContent,
                        IsGory = analysis.Adult.IsGoryContent,
                    });
        }

        // Analyze a local image for size and format  
        public static async Task<Tuple<bool, ImageMetadata>> IsValidLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis =
                    await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
                bool imageFormatIsValid = Helpers.imageFormats.Contains(analysis.Metadata.Format.ToLowerInvariant());
                bool imageSizeIsValid = analysis.Metadata.Width <= Helpers.MaxWidth 
                    && analysis.Metadata.Height <= Helpers.MaxHeight;
                return 
                    Tuple.Create(
                        imageFormatIsValid && imageSizeIsValid, 
                        new ImageMetadata 
                        { 
                            Height = analysis.Metadata.Height, 
                            Width = analysis.Metadata.Width, 
                            Format = analysis.Metadata.Format 
                        });
            }
        }

        // Analyze a remote image for size and format  
        public static async Task<Tuple<bool, ImageMetadata>> IsValidRemoteAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imagePath, features);
            bool imageFormatIsValid = Helpers.imageFormats.Contains(analysis.Metadata.Format.ToLowerInvariant());
            bool imageSizeIsValid = analysis.Metadata.Width <= Helpers.MaxWidth
                && analysis.Metadata.Height <= Helpers.MaxHeight;
            return
                Tuple.Create(
                    imageFormatIsValid && imageSizeIsValid,
                    new ImageMetadata
                    {
                        Height = analysis.Metadata.Height,
                        Width = analysis.Metadata.Width,
                        Format = analysis.Metadata.Format
                    });
        }
    }
}
