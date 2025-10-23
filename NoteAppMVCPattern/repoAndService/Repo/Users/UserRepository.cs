using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo.User
{
    public class UserRepository : IUserRepository
    {
        private readonly List<AppUser> _users = new();

        public void Add(AppUser user) => _users.Add(user);
        public AppUser Get(string id) => _users.FirstOrDefault(u => u.Id == id);
        public void Update(AppUser user)
        {
            AppUser u = Get(user.Id);
            if (u != null)
            {
                u.UserName = user.UserName;
                u.Id = user.Id;
            }
        }
        public void Delete(string id)
        {
            _users.Remove(Get(id));
        }
        public List<AppUser> GetAll() => _users;
    }
}
