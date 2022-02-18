using API.Controllers;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class FirebaseDataContext
    {
        protected static readonly IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = Configuration["FirebaseDataBaseSettings:AuthSecret"],
            BasePath = Configuration["FirebaseDataBaseSettings:BasePath"]
        };

        IFirebaseClient client;

        public FirebaseDataContext()
        {
            client = new FireSharp.FirebaseClient(config);
        }

        public async void DeleteData(string path)
        {
            client = new FireSharp.FirebaseClient(config);

            var response = await client.DeleteAsync(path);
        }

        public async void StoreData(string path, object data)
        {
            client = new FireSharp.FirebaseClient(config);

            var response = await client.SetAsync(path, data);
        }
        public async Task<List<T>> GetData<T>(string path)where T : class, new()
        {
            List<T> objects = new List<T>();

            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = await client.GetAsync(path);

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var item in data)
                {
                    if(item != null)
                    {
                        var _object = new T();

                        if (item.GetType() == typeof(JProperty))
                        {
                            _object = JsonConvert.DeserializeObject<T>(((JProperty)item).Value.ToString());
                        }
                        else
                        {
                            _object = JsonConvert.DeserializeObject<T>(((JObject)item).ToString());
                        }

                        objects.Add(_object);
                    }                    
                }
            }

            return objects;
        }
        public async void EditData(string path, object data)
        {
            client = new FireSharp.FirebaseClient(config);

            var response = await client.UpdateAsync(path, data);
        }
    }
}
