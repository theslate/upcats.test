using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace UpCataas
{
    public class KnownUserHandler : AuthorizationHandler<KnownUserRequirement>
    {
        private readonly UserManager _manager;

        public KnownUserHandler(UserManager manager)
        {
            _manager = manager;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KnownUserRequirement requirement)
        {
            var id = context.User.GetObjectId();
            if (_manager.GetUser(id) != null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
