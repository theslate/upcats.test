using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace UpCataas.Controllers
{
    /// <summary>
    /// Allows access to cache images
    /// </summary>
    [Authorize("KnownUser")]
    [Route("[controller]")]
    public class ImagesController : Controller
    {
        private IMemoryCache _cache;

        public ImagesController(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        /// <summary>
        /// Returns a cached cat image.
        /// </summary>
        /// <returns>Cached cat image.</returns>
        /// <response code="200">Returns account info</response>
        /// <response code="401">User not logged in</response>
        /// <response code="403">User forbidden</response>
        [HttpGet]
        [Route("{cacheKey}")]
        public IMagickImage Get(string cacheKey = "")
        {
            object image;
            if (_cache.TryGetValue(cacheKey, out image))
            {
                return image as IMagickImage;
            }

            return null;
        }
    }
}
