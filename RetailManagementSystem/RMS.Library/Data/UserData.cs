using RMS.Library.DataAccess;
using RMS.Library.Model;

namespace RMS.Library.Data;

public class UserData : IUserData
{
    private readonly IDataAccess _dataAccess;

    public UserData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public Task<List<UserModel>> GetUserById(string Id)
    {
        var output = _dataAccess.LoadData<UserModel, dynamic>(
            "dbo.spUserLookup",
            new { Id },
            "RMSDatabase");

        return output;
    }
}