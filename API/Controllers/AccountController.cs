using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private CommunicationsController communicationsController;
        public AccountController (ITokenService tokenService)
        {
            _tokenService = tokenService;
            communicationsController = new CommunicationsController();
        }
        #region SignUp
        [HttpPost("signup")]
        public async Task<ActionResult<UserDto>> SignUp(SignUpDto signUpDto)
        {
            using var hmac = new HMACSHA512();

            if (await GetUser(signUpDto.Email) != null)
            {
                return BadRequest("You already have an account");
            }

            User appUser = new User()
            {
                OrganizationName = signUpDto.Organization,
                Email = signUpDto.Email,
                APIKey = GenerateAPIKey(),
                OrganizationSenderCode = signUpDto.SenderId,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signUpDto.Password)),
                //Create the hash for the apiKey
                //AccountType = signUpDto.AccountType,
                //PhoneNumber = signUpDto.Phonenumber,
            };

            appUser.Id = await GetId("Account");

            _firebaseDataContext.StoreData("Account/" + appUser.Id, appUser);

            communicationsController.SendMessage("76199359", "yewotheu123456789@gmail.com", appUser.OrganizationName + " Created an account.", "Core Communications Account Creation");

            return new UserDto() {
                APIKey = appUser.APIKey,
                Email = appUser.Email,
                OrganizationName = appUser.OrganizationName,
                OrganizationSenderCode = appUser.OrganizationSenderCode,
                Token= _tokenService.CreateToken(appUser),
            };
        }
        #endregion
        #region Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //Checks if the account exists
            if (await GetUser(loginDto.AccountId) == null)
                return BadRequest("This account does not exist");

            User user = await GetUser(loginDto.AccountId);

            using var hmac = new HMACSHA512(user.PasswordSalt);
            Byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Wrong password");
            }

            return new UserDto()
            {
                APIKey = user.APIKey,
                Email = user.Email,
                OrganizationName = user.OrganizationName,
                OrganizationSenderCode = user.OrganizationSenderCode,
                Token = _tokenService.CreateToken(user),
            };
        }
        #endregion
    }
}