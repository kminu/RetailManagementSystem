using RMS.Library.Model;

namespace RMS.Library.Data
{
    public interface IUserData
    {
        Task<List<UserModel>> GetUserById(string Id);
    }
}