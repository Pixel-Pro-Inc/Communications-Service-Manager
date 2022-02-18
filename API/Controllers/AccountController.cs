using API.DTO;
using API.Entities;
using API.Interfaces;
using IronOcr;
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
        #region OmangFill
        [HttpPost("omangfill")]
        public async Task<ActionResult<OmangFillDto>> OmangFill(OmangImageDto omangImageDto)
        {
            string omangImage = omangImageDto.img;

            if (!string.IsNullOrEmpty(omangImage))
            {
                OcrResult result = null;
                var Ocr = new IronTesseract();
                string path = await GetImage(omangImage);
                using (var Input = new OcrInput(@"" + path))
                {
                    result = await Ocr.ReadAsync(Input);
                }

                string r = result.Text;

                OmangFillDto omangFillDto = await getDataFromScan(r);

                if (omangFillDto == null)
                    return BadRequest("Please try again with a different picture");

                GC.Collect();
                GC.WaitForPendingFinalizers();

                return omangFillDto;
            }

            return BadRequest("You have to enter an image");
        }
        private async Task<OmangFillDto> getDataFromScan(string data)
        {
            OmangFillDto omangFillDto = new OmangFillDto();

            data = data.Replace(System.Environment.NewLine, "_");
            data = data.Replace(' ', '_');

            for (int i = 0; i < 1000; i++)
            {
                data = data.Replace("__", "_");
            }

            #region Collect Info
            //Omang Number Check
            int streak = 0;
            int g = 0;
            int e = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (Int32.TryParse(data[i].ToString(), out g))
                {
                    streak++;
                    if (streak == 9)
                    {
                        int omangNumber = Int32.Parse(data.Substring(i - 8, 9));
                        e = i + 1;
                    }
                }
                else
                {
                    streak = 0;
                }
            }

            //Surname
            int block = 0;
            streak = 0;
            int e2 = 0;
            for (int i = e; i < data.Length; i++)
            {
                if (Char.IsUpper(data[i]))
                {
                    streak++;
                }
                else
                {
                    if (streak > 2 && block == 0)
                    {
                        omangFillDto.lastname = data.Substring(i - streak, streak);
                        e2 = i + 1;
                        block = 1;
                        streak = 0;
                    }
                }
            }

            //Yewo
            int block1 = 0;
            streak = 0;
            int e3 = 0;
            for (int i = e2; i < data.Length; i++)
            {
                if (Char.IsUpper(data[i]))
                {
                    streak++;
                }
                else
                {
                    if (streak > 2 && block1 == 0)
                    {
                        omangFillDto.firstname = data.Substring(i - streak, streak);
                        e3 = i + 1;
                        block1 = 1;
                        streak = 0;
                    }
                }
            }

            string birthDateStore = "";

            //Birthday Check
            streak = 0;
            g = 0;

            for (int i = e3; i < data.Length; i++)
            {
                if (Int32.TryParse(data[i].ToString(), out g) || data[i] == '/')
                {
                    streak++;
                    if (streak == 10)
                    {
                        string temp = data.Substring(i - 9, 10);
                        birthDateStore = temp;
                    }
                }
                else
                {
                    streak = 0;
                }
            }
            #endregion

            //Validation

            if (omangFillDto.lastname == "")
                return null;

            if (omangFillDto.firstname == "")
                return null;

            omangFillDto.dateofbirth = birthDateStore.Replace('/', ' ');

            //Manipulations
            omangFillDto.firstname = omangFillDto.firstname.Replace("_", string.Empty);
            omangFillDto.lastname = omangFillDto.lastname.Replace("_", string.Empty);

            omangFillDto.firstname = omangFillDto.firstname.ToLower();
            omangFillDto.lastname = omangFillDto.lastname.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            omangFillDto.firstname = textInfo.ToTitleCase(omangFillDto.firstname);
            omangFillDto.lastname = textInfo.ToTitleCase(omangFillDto.lastname);

            return omangFillDto;
        }
        private async Task<string> GetImage(string base64)
        {
            int n = base64.IndexOf("base64,");

            base64 = base64.Remove(0, n + 7);

            byte[] imgBytes = Convert.FromBase64String(base64);

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OutputImage.jpg");

            using (var imageFile = new FileStream(path, FileMode.Create))
            {
                imageFile.Write(imgBytes, 0, imgBytes.Length);
                imageFile.Flush();
            }

            return path;
        }
        #endregion
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
        //I removed GetUser from here and put it in BaseApiController cause I needed it in MessageController
        #endregion
        #region GetAdminAccounts
        [HttpGet("users/admin")]
        public async Task<ActionResult<List<UserDto>>> GetAdminAccounts()
        {
            List<UserDto> userDtos = new List<UserDto>();

            List<User> users = await _firebaseDataContext.GetData<User>("Account");

            foreach (var item in users)
            {
                if (item.AccountType == "Developer")
                    continue;

                userDtos.Add(new UserDto() {
                    AccountType = item.AccountType,
                    Email = item.Email,
                    Firstname = item.FirstName,
                    Lastname = item.LastName,
                    Phonenumber = item.PhoneNumber,
                    Username = item.GetUserName(),
                    Disabled = item.Disabled,
                    Token = ""
                });
            }

            return userDtos;
        }
        #endregion
        #region DeleteAccount
        [HttpGet("users/delete/{accountId}")]
        public async Task<ActionResult<bool>> Delete(string accountId)
        {
            User user = await GetUser(accountId);

            user.Disabled = !user.Disabled;

            _firebaseDataContext.EditData("Account/" + user.Id, user);

            string state = user.Disabled ? "disabled" : "enabled";

            communicationsController.SendMessage(user.PhoneNumber.ToString(), user.Email, user.GetUserName() + $" Your account has been {state}.", "Blue Union Account Deletion");

            return true;
        }
        #endregion
    }
}
