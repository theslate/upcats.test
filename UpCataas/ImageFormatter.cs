using System;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace UpCataas
{
    /// <summary>
    /// Formats IMagickImage into an HttpResponse
    /// </summary>
    public class ImageFormatter : OutputFormatter
    {
        public string[] ImageMimeTypes = new[] {"png", "jpg", "jpeg", "bmp", "tiff", "webp" };

        public ImageFormatter()
        {
            var mediaTypes = ImageMimeTypes.Select(t => new MediaTypeHeaderValue($"image/{t}"));
            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        public bool CanWriteTypes(Type type)
        {
            return CanWriteType(type);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IMagickImage).IsAssignableFrom(type);
        }

        public static async Task WriteResponseBodyAsync(IMagickImage image, PipeWriter writer)
        {
            var rawImage = image.ToByteArray();
            await writer.WriteAsync(rawImage);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var image = context.Object as IMagickImage;
            if (image == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.HttpContext.Response.Headers["ETag"] = image.Signature;
            await WriteResponseBodyAsync(image, context.HttpContext.Response.BodyWriter);
        }
    }
}