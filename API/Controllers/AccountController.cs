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

            if (await GetUser(signUpDto.Phonenumber.ToString()) != null)
            {
                return BadRequest("You already have an account");
            }

            if (await GetUser(signUpDto.Email) != null)
            {
                return BadRequest("You already have an account");
            }                            

            User appUser = new User()
            {
                FirstName = signUpDto.Firstname,
                LastName = signUpDto.Lastname,
                Email = signUpDto.Email,
                DateOfBirth = signUpDto.Dateofbirth,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signUpDto.Password)),
                //Create the hash for the apiKey
                AccountType = signUpDto.AccountType,
                PhoneNumber = signUpDto.Phonenumber,
            };

            appUser.Id = await GetId("Account");

            _firebaseDataContext.StoreData("Account/" + appUser.Id, appUser);

            communicationsController.SendMessage("76199359", "yewotheu123456789@gmail.com", appUser.GetUserName() + " Created an account.", "Blue Union Account Creation");

            return new UserDto() {
                Firstname = signUpDto.Firstname,
                Lastname = signUpDto.Lastname,
                Email = signUpDto.Email,
                AccountType = signUpDto.AccountType,
                Phonenumber = signUpDto.Phonenumber,
                Token = _tokenService.CreateToken(appUser),
                Disabled = appUser.Disabled,
                Username = appUser.GetUserName()
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

            if ((await GetUser(loginDto.AccountId)).Disabled)
                return BadRequest("Your account has been disabled");

            User user = await GetUser(loginDto.AccountId);

            using var hmac = new HMACSHA512(user.PasswordSalt);
            Byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Wrong password");
            }

            return new UserDto() {
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                Phonenumber = user.PhoneNumber,
                Token = _tokenService.CreateToken(user),
                AccountType = user.AccountType,
                Disabled = user.Disabled,
                Username = user.GetUserName()
            };
        }
        #endregion
    }
}
