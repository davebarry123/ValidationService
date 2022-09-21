namespace ValidationService.Classes
{
    public sealed class ImageMetadata
    {
        public int Height { get; set; } = 0;

        public int Width { get; set; } = 0;

        public string Format { get; set; } = string.Empty;

        public bool IsRacy { get; set; }

        public bool IsGory { get; set; }

        public bool IsAdult { get; set; }
    }
}
