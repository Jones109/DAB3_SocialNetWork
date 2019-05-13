
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Services
{
    public class LoginTestService
    {
        private readonly IMongoCollection<LoginTest> _loginTest;
        private MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        public LoginTestService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _loginTest = database.GetCollection<LoginTest>("LoginTests");
        }

        public List<LoginTest> Get()
        {
            return _loginTest.Find(LoginTest => true).ToList();
        }

        public LoginTest Get(string id)
        {
            return _loginTest.Find(LoginTest => LoginTest.userID == id).FirstOrDefault();
        }

        public LoginTest Create(LoginTest LoginTestIn)
        {
            LoginTestIn.password = HashPass(LoginTestIn.password);
            _loginTest.InsertOne(LoginTestIn);
            return LoginTestIn;
        }

        public void Update(LoginTest LoginTestIn)
        {
            LoginTestIn.password = HashPass(LoginTestIn.password);
            _loginTest.ReplaceOne(l => l.userID == LoginTestIn.userID, LoginTestIn);
        }

        public void Remove(LoginTest LoginTestIn)
        {
            _loginTest.DeleteOne(LoginTest => LoginTest.userID == LoginTestIn.userID);
        }

        public void Remove(string id)
        {
            _loginTest.DeleteOne(LoginTest => LoginTest.userID == id);
        }
        private string HashPass(string pass)
        {
            var passBytes = System.Text.Encoding.ASCII.GetBytes(pass);
            var HashBytes = md5.ComputeHash(passBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < HashBytes.Length; i++)
            {
                sb.Append(HashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public bool Login(LoginTest userToLogIn,out string id)
        {
            var pass = HashPass(userToLogIn.password);
            id = _loginTest.Find(u => u.userName == userToLogIn.userName && u.password == pass).FirstOrDefault().userID;

            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }


}

