﻿using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        protected readonly FirebaseDataContext _firebaseDataContext;
        protected static readonly IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

        public BaseApiController()
        {
            _firebaseDataContext = new FirebaseDataContext();
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //Removed this from AccountController so it can be shared with all controllers but noone else, hence the protected modifier
        //Also Why did you decided to use only email and PhoneNumbers here
        public async Task<User> GetUser(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
                return null;

            List<User> users = await _firebaseDataContext.GetData<User>("Account");

            int count = users.Where(u => u.Email == accountID || u.PhoneNumber.ToString() == accountID).ToList().Count;

            if (count != 0)
            {
                return users.Where(u => u.Email == accountID || u.PhoneNumber.ToString() == accountID).ToList()[0];
            }

            return null;
        }

        public async Task<int> GetId(string baseNode) => (await _firebaseDataContext.GetData<object>(baseNode)).Count;

        public string FormatAmountString(string amount)
        {
            return String.Format("{0:n}", float.Parse(amount));
        }

        protected string[] GetNameAndData(string input)
        {
            string[] vals = new string[2];

            foreach (char item in input)
            {
                if (item == ',')
                {
                    vals[1] = input.Substring(0, input.IndexOf(item));
                    vals[0] = input.Substring(input.IndexOf(item) + 1, input.Length - (input.IndexOf(item) + 1));

                    break;
                }
            }

            return vals;
        }
    }
}
