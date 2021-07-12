using ImageMagick;
using Microsoft.Extensions.Caching.Memory;

namespace UpCataas
{
    public class CachedImageTransformer : IImageTransformer
    {
        private IMemoryCache cache;

        public CachedImageTransformer(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public IMagickImage FlipUpsideDown(IMagickImage image)
        {
            // Get
            return cache.GetOrCreate(image.Signature, entry =>
            {
                image.Flip();
                return image;
            });
        }
    }
}
