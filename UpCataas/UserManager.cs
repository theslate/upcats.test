using System.Collections.Concurrent;
using System.Linq;
using UpCataas.Models;

namespace UpCataas
{
    public class UserManager
    {
        public ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        public User CreateUser(string userId)
        {
            return _users.AddOrUpdate(
                userId,
                new User{Id = userId, ImageHistory = Enumerable.Empty<string>()},
                (_, u) => u);
        }

        public User GetUser(string id)
        {
            if (id == null)
            {
                return null;
            }

            if (_users.TryGetValue(id, out User user))
            {
                return user;
            }

            return null;
        }
    }
}
