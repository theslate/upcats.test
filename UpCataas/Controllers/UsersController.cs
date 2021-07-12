using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using UpCataas.Models;

namespace UpCataas.Controllers
{
    /// <summary>
    /// Controls users.
    /// </summary>
    [Route("[controller]/[action]")]
    public class UsersController : Controller
    {
        private UserManager _manager;

        public UsersController(UserManager manager)
        {
            _manager = manager;
        }
        
        /// <summary>
        /// Creates and account
        /// </summary>
        /// <returns>Created account info.</returns>
        /// <response code="200">Returns created account info</response>
        /// <response code="401">User not logged in</response>
        [Authorize]
        [HttpPost]
        public User CreateAccount()
        {
            var userId = User.GetObjectId();
            if (userId == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            return _manager.CreateUser(userId);
        }
        
        /// <summary>
        /// Returns account info of user that is logged in.
        /// </summary>
        /// <returns>account info.</returns>
        /// <response code="200">Returns account info</response>
        /// <response code="401">User not logged in</response>
        /// <response code="403">User forbidden</response>
        [Authorize("KnownUser")]
        [HttpGet]
        public User Me()
        {
            var userId = User.GetObjectId();
            return _manager.GetUser(userId);
        }
    }
}
