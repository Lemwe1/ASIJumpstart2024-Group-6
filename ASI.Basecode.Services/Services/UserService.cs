using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = repository;
        }

        public IEnumerable<UserViewModel> RetrieveAll(int? id = null, string firstName = null)
        {
            var data = _userRepository.GetUsers()
                .Where(x => x.Deleted != true
                        && (!id.HasValue || x.UserId == id)
                        && (string.IsNullOrEmpty(firstName) || x.FirstName.Contains(firstName)))
                .Select(s => new UserViewModel
                {
                    Id = s.UserId,
                    Name = string.Concat(s.FirstName, " ", s.LastName),
                    Description = s.Remarks,
                });
            return data;
        }

        public UserViewModel RetrieveUser(int id)
        {
            var data = _userRepository.GetUsers().FirstOrDefault(x => x.Deleted != true && x.UserId == id);
            var model = new UserViewModel
            {
                Id = data.UserId,
                UserCode = data.UserCode,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Password = PasswordManager.DecryptPassword(data.Password)
            };
            return model;
        }

        public MUser GetByEmail(string email)
        {
            // Retrieve user by email from the repository
            return _userRepository.GetUsers().SingleOrDefault(u => u.Mail == email && !u.Deleted);
        }

        public MUser GetByResetToken(string token)
        {
            // Retrieve user by reset token from the repository
            return _userRepository.GetUsers().SingleOrDefault(u => u.PasswordResetToken == token && u.PasswordResetExpiration > DateTime.Now && !u.Deleted);
        }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Add(UserViewModel model)
        {
            var newModel = new MUser
            {
                UserCode = model.UserCode,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Mail = model.Mail,
                Password = PasswordManager.EncryptPassword(model.Password),  // Encrypt the password
                UserRole = 1,  // Assuming 1 is for regular users
                InsDt = DateTime.Now,
                UpdDt = DateTime.Now,
                Deleted = false
            };

            _userRepository.AddUser(newModel);
        }

        /// <summary>
        /// Updates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Update(MUser model)
        {
            var existingData = _userRepository.GetUsers().FirstOrDefault(s => !s.Deleted && s.UserId == model.UserId);
            if (existingData != null)
            {
                existingData.UserCode = model.UserCode;
                existingData.FirstName = model.FirstName;
                existingData.LastName = model.LastName;
                existingData.Password = PasswordManager.EncryptPassword(model.Password);  // Ensure the password is encrypted
                existingData.PasswordResetToken = model.PasswordResetToken;
                existingData.PasswordResetExpiration = model.PasswordResetExpiration;

                _userRepository.UpdateUser(existingData);  // Call the repository to update the user in the database
            }
        }



        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Delete(int id)
        {
            _userRepository.DeleteUser(id);
        }

        public LoginResult AuthenticateUser(string userCode, string password, ref MUser user)
        {
            user = new MUser();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _userRepository.GetUsers().Where(x => x.UserCode == userCode &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }
    }
}
