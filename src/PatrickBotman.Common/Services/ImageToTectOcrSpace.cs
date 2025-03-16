using Microsoft.Extensions.Options;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Models;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Services
{
    public class ImageToTectOcrSpace: IImageToTextService
    {
        private readonly HttpClient _httpClient;
        private readonly OCRConfiguration _ocrConfiguration;

        public ImageToTectOcrSpace(IOptionsSnapshot<OCRConfiguration> ocrConfiguration, HttpClient httpClient)
        {
            _ocrConfiguration = ocrConfiguration.Value;
            _httpClient = httpClient;
        }

        public string? GetText(byte[] image)
        {
            using var form = new MultipartFormDataContent();

            var base64String = Convert.ToBase64String(image);

            form.Add(new StringContent(_ocrConfiguration.ApiKey), "apikey");
            form.Add(new StringContent("auto"), "language");
            form.Add(new StringContent("2"), "OCREngine");
            form.Add(new StringContent("jpg"), "filetype");

            var jpgContent = new ByteArrayContent(image);
            jpgContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            form.Add(jpgContent, "file", "file.jpg");

            var res = _httpClient.PostAsync("https://api.ocr.space/parse/image", form).Result.Content.ReadFromJsonAsync<OCRSpaceResult>().Result;

            return res!.ParsedResults[0].ParsedText.ReplaceLineEndings(" ");
        }
    }
}
