using ImageMagick;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace UpCataas
{
    public interface IImageTransformService
    {
        Task<IMagickImage> Flip(Uri source);
    }

    public class ImageTransformService : IImageTransformService
    {
        private readonly ILogger _logger;
        private readonly IImageTransformer _transformer;
        private readonly HttpClient _client;


        public ImageTransformService(ILogger<ImageTransformService> logger, IImageTransformer transformer, HttpClient client)
        {
            _logger = logger;
            _transformer = transformer;
            _client = client;
        }

        public async Task<IMagickImage> Flip(Uri source)
        {
            try
            {
                var catResponse = await _client.GetAsync(source);
                var image =  new MagickImage(await catResponse.Content.ReadAsByteArrayAsync());
                var flipped = _transformer.FlipUpsideDown(image);
                return flipped;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Warning, e.Message);
                throw;
            }
        }
    }
}
