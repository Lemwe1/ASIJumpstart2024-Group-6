using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> RetrieveAll(int? id = null, string firstName = null);
        UserViewModel RetrieveUser(int id);
        void Add(MUser model);
        void Update(MUser model);
        void Delete(int id);
        LoginResult AuthenticateUser(string userCode, string password, ref MUser user);

        // Add the following methods:
        MUser GetByUserCode(string userCode);
        MUser GetByEmail(string email);  // Method to get user by email
        MUser GetByResetToken(string token);  // Method to get user by reset token
        MUser GetByVerificationToken(string token);

    }
}
