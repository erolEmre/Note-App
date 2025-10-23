using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo.User;

namespace NoteAppMVCPattern.Services.User
{
    // Services/UserService.cs
    public class UserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public AppUser GetUser(string id)
        {
            return _repo.Get(id);
            
        }
        public void AddUser(string id, string name) 
        {
            AppUser user = new AppUser();
            user.Id = id;
            user.UserName = name;
            _repo.Add(user);
        }
        public bool UpdateUser(string id, string name)
        {
            var user = GetUser(id);
            if (user == null)
                return false;

            user.UserName = name;
            _repo.Update(user);
            return true;
        }
        public bool DeleteUser(string id)
        {
            AppUser user = GetUser(id);
            if (user == null)
            {
                return false;
            }
            _repo.Delete(user.Id);
            return true;

        }
        public List<AppUser> GetAllUsers() => _repo.GetAll();
    }

}
