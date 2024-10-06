using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<MUser> GetUsers()
        {
            return this.GetDbSet<MUser>();
        }

        public bool UserExists(int userId)
        {
            return this.GetDbSet<MUser>().Any(x => x.UserId == userId);
        }

        public void AddUser(MUser user)
        {
            var maxId = this.GetDbSet<MUser>().Max(x => x.UserId) + 1;
            user.UserId = maxId;
            user.UpdDt = DateTime.Now;
            this.GetDbSet<MUser>().Add(user);
            UnitOfWork.SaveChanges();  // Persist the user to the database
        }

        public void UpdateUser(MUser user)
        {
            this.GetDbSet<MUser>().Update(user);
            user.UpdDt = DateTime.Now;
            UnitOfWork.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var userToDelete = this.GetDbSet<MUser>().FirstOrDefault(x => x.Deleted != true && x.UserId == userId);
            if (userToDelete != null)
            {
                userToDelete.Deleted = true;
                userToDelete.UpdDt = DateTime.Now;
            }
            UnitOfWork.SaveChanges();
        }

    }
}
