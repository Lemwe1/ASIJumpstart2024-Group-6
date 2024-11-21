using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> RetrieveAll(int? id = null, string firstName = null);
        UserViewModel RetrieveUser(int id);
        void Add(MUser model);
        void Update(MUser model, bool isPasswordUpdate = false);
        Task<bool> UpdateUserInformationAsync(UserViewModel model); // Add this method
        void Delete(int id);
        LoginResult AuthenticateUser(string userCode, string password, ref MUser user);

        MUser GetByUserCode(string userCode);
        MUser GetByEmail(string email);
        MUser GetByResetToken(string token);
        MUser GetByVerificationToken(string token);
    }
}