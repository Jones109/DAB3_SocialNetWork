using System.Collections.Generic;
using System.Linq;
using SocialNetWork.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;
using System.Security.Cryptography;
using System.Text;

namespace SocialNetwork.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _posts;
        private readonly IMongoCollection<Wall> _walls;
        private MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _users = database.GetCollection<User>("Users");
            _walls = database.GetCollection<Wall>("Walls");
            _posts = database.GetCollection<Post>("Posts");
        }

        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }

        public User Get(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public List<User> GetFollowing(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            List<User> followers = new List<User>();

            //if (model.FollowingId != null)
            //{
            //    foreach (var fo in model.FollowingId)
            //    {
            //        followers.Add(_users.Find<User>(user => user.Id == fo.followerID).FirstOrDefault());
            //    }
            //}

            return followers;
        }

        public List<Post> GetFeedPosts(string id)
        {
            List<Post> posts = new List<Post>();
            List<Wall> followingWall = new List<Wall>();

            var user = Get(id);

            if (user.FollowingId.Count != 0)
            {
                foreach (var wall in user.FollowingId)
                {
                    followingWall.Add(_walls.Find<Wall>(w => w.ID == wall).FirstOrDefault(););
                }
            }

        //}

        public List<User> GetFollowers(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            List<User> followers = new List<User>();

            //if (model.Followers != null)
            //{
            //    foreach (var fo in model.Followers)
            //    {
            //        followers.Add(_users.Find<User>(user => user.Id == fo.followerID).FirstOrDefault());
            //    }
            //}

            return followers;
        }
        
        public User Create(User LoginTestIn)
        {
            LoginTestIn.Password = HashPass(LoginTestIn.Password);
            _users.InsertOne(LoginTestIn);
            return LoginTestIn;
        }

        public void Update(User LoginTestIn)
        {
            LoginTestIn.Password = HashPass(LoginTestIn.Password);
            _users.ReplaceOne(l => l.Id == LoginTestIn.Id, LoginTestIn);
        }

        public void Remove(User LoginTestIn)
        {
            _users.DeleteOne(LoginTest => LoginTest.Id == LoginTestIn.Id);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(LoginTest => LoginTest.Id == id);
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

        public bool Login(User userToLogIn, out string id)
        {
            var pass = HashPass(userToLogIn.Password);
            id = _users.Find(u => u.UserName == userToLogIn.UserName && u.Password == pass).FirstOrDefault().Id;

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