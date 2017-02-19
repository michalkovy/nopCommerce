using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;

namespace Nop.Plugin.Api.DTOs.Images
{
    [ImageValidation]
    public class ImageDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("picture_id")]
        public int PictureId { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("attachment")]
        public string Attachment { get; set; }

        [JsonIgnore]
        public byte[] Binary { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the SEO friednly filename of the picture
        /// </summary>
        [JsonProperty("seo_filename")]
        public string SeoFilename { get; set; }
    }
}