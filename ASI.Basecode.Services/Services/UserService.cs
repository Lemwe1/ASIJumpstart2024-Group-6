using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AsiBasecodeDbContext _dbContext;

        public UserService(AsiBasecodeDbContext dbContext, IUserRepository repository, IMapper mapper, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userRepository = repository;
            _logger = logger;
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
                Mail = data.Mail,
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
            return _userRepository.GetUsers().SingleOrDefault(u => u.Mail == email && !u.Deleted);
        }

        public MUser GetByResetToken(string token)
        {
            return _userRepository.GetUsers().SingleOrDefault(u => u.PasswordResetToken == token && u.PasswordResetExpiration > DateTime.Now && !u.Deleted);
        }

        public MUser GetByVerificationToken(string token)
        {
            return _userRepository.GetUsers().SingleOrDefault(u => u.VerificationToken == token && u.VerificationTokenExpiration > DateTime.Now && !u.Deleted);
        }

        public void Add(MUser newUser)
        {
            var newModel = new MUser
            {
                UserCode = newUser.UserCode,
                Mail = newUser.Mail,
                Password = PasswordManager.EncryptPassword(newUser.Password),
                UserRole = 1,
                InsDt = DateTime.Now,
                UpdDt = DateTime.Now,
                Deleted = false,
                VerificationToken = newUser.VerificationToken,
                VerificationTokenExpiration = newUser.VerificationTokenExpiration,
                IsVerified = false
            };

            _userRepository.AddUser(newModel);
        }

        public void Update(MUser model, bool isPasswordUpdate = false)
        {
            var existingData = _userRepository.GetUsers().FirstOrDefault(s => !s.Deleted && s.UserId == model.UserId);

            if (existingData != null)
            {
                existingData.UserCode = model.UserCode;
                existingData.FirstName = model.FirstName ?? existingData.FirstName;
                existingData.LastName = model.LastName ?? existingData.LastName;

                if (isPasswordUpdate && !string.IsNullOrEmpty(model.Password))
                {
                    existingData.Password = PasswordManager.EncryptPassword(model.Password);
                }

                if (!string.IsNullOrEmpty(model.PasswordResetToken))
                {
                    existingData.PasswordResetToken = model.PasswordResetToken;
                    existingData.PasswordResetExpiration = model.PasswordResetExpiration;
                }

                if (!string.IsNullOrEmpty(model.VerificationToken))
                {
                    existingData.VerificationToken = model.VerificationToken;
                    existingData.VerificationTokenExpiration = model.VerificationTokenExpiration;
                }

                existingData.IsVerified = model.IsVerified;
                _userRepository.UpdateUser(existingData);
            }
        }

        public async Task<bool> UpdateUserInformationAsync(UserViewModel model)
        {
            // Fetch the user from the correct DbContext using the provided user ID
            var user = await _dbContext.MUsers.FindAsync(model.Id);

            if (user == null)
            {
                // Log the error and return false if the user is not found
                _logger.LogError($"User with ID {model.Id} not found.");
                return false;
            }

            // Update the user details
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Mail = model.Mail;

            try
            {
                // Attempt to save the changes to the database
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated user with ID {model.Id}");
                return true;
            }
            catch (Exception ex)
            {
                // Log any exceptions and return false
                _logger.LogError($"Error updating user with ID {model.Id}: {ex.Message}");
                return false;
            }
        }

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
