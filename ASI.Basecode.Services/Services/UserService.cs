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
        public MUser GetByUserCode(string userCode)
        {
            return _userRepository.GetUsers().SingleOrDefault(u => u.UserCode == userCode && !u.Deleted);
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

        public MUser GetByVerificationToken(string token)
        {
            // Ensure we are correctly filtering for tokens that haven't expired and are not deleted
            return _userRepository.GetUsers().SingleOrDefault(u => u.VerificationToken == token && u.VerificationTokenExpiration > DateTime.Now && !u.Deleted);
        }


        /// <summary>
        /// Adds a new user, including the verification token for email verification.
        /// </summary>
        public void Add(MUser newUser)
        {
            var newModel = new MUser
            {
                UserCode = newUser.UserCode,
                Mail = newUser.Mail,
                Password = PasswordManager.EncryptPassword(newUser.Password),  // Password is already encrypted
                UserRole = 1,  // Assuming 1 is for regular users
                InsDt = DateTime.Now,
                UpdDt = DateTime.Now,
                Deleted = false,
                VerificationToken = newUser.VerificationToken,  // Use the token generated in the controller
                VerificationTokenExpiration = newUser.VerificationTokenExpiration,  // Use the expiration generated in the controller
                IsVerified = false  // Account is initially unverified
            };

            _userRepository.AddUser(newModel);
        }


        /// <summary>
        /// Updates the user details, including resetting or clearing tokens.
        /// </summary>
        public void Update(MUser model, bool isPasswordUpdate = false)
        {
            var existingData = _userRepository.GetUsers().FirstOrDefault(s => !s.Deleted && s.UserId == model.UserId);

            if (existingData != null)
            {
                // Update UserCode, but avoid overwriting verification tokens unless explicitly provided
                existingData.UserCode = model.UserCode;

                // Allow updating First Name and Last Name
                existingData.FirstName = model.FirstName ?? existingData.FirstName;  // Only update if provided
                existingData.LastName = model.LastName ?? existingData.LastName;     // Only update if provided

                // Update password only if it is part of the operation
                if (isPasswordUpdate && !string.IsNullOrEmpty(model.Password))
                {
                    existingData.Password = PasswordManager.EncryptPassword(model.Password);
                }

                // Update the password reset token and expiration if provided
                if (!string.IsNullOrEmpty(model.PasswordResetToken))
                {
                    existingData.PasswordResetToken = model.PasswordResetToken;
                    existingData.PasswordResetExpiration = model.PasswordResetExpiration;
                }

                // Update the verification token and expiration only if provided
                if (!string.IsNullOrEmpty(model.VerificationToken))
                {
                    existingData.VerificationToken = model.VerificationToken;
                    existingData.VerificationTokenExpiration = model.VerificationTokenExpiration;
                }

                // Optionally, allow updating verification status (if needed)
                existingData.IsVerified = model.IsVerified;

                // Persist changes to the database
                _userRepository.UpdateUser(existingData);
            }
        }


        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
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
