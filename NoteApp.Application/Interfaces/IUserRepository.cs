using NoteApp.Core.Entities;

namespace NoteApp.Application.Repo.User
{

    public interface IUserRepository
    {
        void Add(AppUser user);
        AppUser Get(string id);
        void Update(AppUser user);
        void Delete(string id);
        List<AppUser> GetAll();
    }


}
