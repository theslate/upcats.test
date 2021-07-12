using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace UpCataas.Controllers
{
    /// <summary>
    /// Humanely transforms cats.
    /// </summary>
    [Authorize("KnownUser")]
    [ApiController]
    [Route("[controller]")]
    public class UpcatController : ControllerBase
    {
        public IImageTransformService _service;
        private readonly UserManager _manager;

        public UpcatController(IImageTransformService service, UserManager manager)
        {
            _service = service;
            _manager = manager;
        }

        /// <summary>
        /// Humanely flips a cat.
        /// </summary>
        /// <param name="resource">The CATAAS cat to flip.</param>
        /// <returns>A flipped cat.</returns>
        /// <response code="200">Returns the newly flipped cat</response>
        /// <response code="404">Cat not found</response>
        /// <response code="401">User not authorized</response>
        /// <response code="403">User Forbidden</response>
        [HttpGet]
        [Route("flip")]
        [Route("flip/{*resource}")]
        public async Task<IMagickImage> Get(string resource = "")
        {
            var flippedCat = await _service.Flip(new Uri(Path.Combine("https://cataas.com/cat", resource ?? "")));

            var user = _manager.GetUser(User.GetObjectId());
            user.ImageHistory = user.ImageHistory.Append(flippedCat.Signature);

            return flippedCat;
        }
    }
}
