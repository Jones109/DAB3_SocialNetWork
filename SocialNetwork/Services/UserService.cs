using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetWork.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ViewModels;

namespace SocialNetwork.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _posts;
        private readonly IMongoCollection<Wall> _walls;
        private readonly IMongoCollection<Circle> _circles;
        private MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _users = database.GetCollection<User>("Users");
            _walls = database.GetCollection<Wall>("Walls");
            _posts = database.GetCollection<Post>("Posts");
            _circles = database.GetCollection<Circle>("Circles");
        }

        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }

        public User Get(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public bool Follow(string idToFollow, string followerId)
        {
            var UserToFollow = _users.Find<User>(u => u.Id == idToFollow).FirstOrDefault();
            var follower = _users.Find<User>(u => u.Id == followerId).FirstOrDefault();

            if (follower != null && UserToFollow != null)
            {
                if (follower.FollowingId == null)
                    follower.FollowingId = new List<string>();
                if(UserToFollow.Followers == null)
                    UserToFollow.Followers = new List<string>();
                /*
                follower f1 = new follower { followerID = followerId, followerName = follower.Name };
                follower f2 = new follower { followerID = idToFollow, followerName = following.Name };
                */

                UserToFollow.Followers.Add(followerId);
                follower.FollowingId.Add(idToFollow);
                try
                {
                    _users.ReplaceOne<User>(u => u.Id == idToFollow, UserToFollow);
                    _users.ReplaceOne<User>(u => u.Id == followerId, follower);
                }
                catch (MongoBulkWriteException)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public List<Post> GetUserPosts(string id)
        {
            User u = Get(id);
            Wall w = _walls.Find(wa => wa.ID == u.Wall).FirstOrDefault();
            List<Post> posts = new List<Post>();

            if (w.postIDs != null)
            {
                foreach (var post in w.postIDs)
                {
                    posts.Add(_posts.Find(p => p.Id == post).FirstOrDefault());
                }
            }

            return posts;
        }
        
        public List<Post> GetFeedPosts(string id)
        {
            List<Post> posts = new List<Post>();
            List<Wall> followingWall = new List<Wall>();
            List<string> WallIds = new List<string>();
            List<User> Following = GetFollowing(id);
            
            if (Following != null)
            {
                foreach (var following in Following)
                {
                    WallIds.Add(following.Wall);
                }
                foreach (var wallId in WallIds)
                {
                    Wall temp = _walls.Find(w => w.ID == wallId).FirstOrDefault();
                    bool IsBlacklisted = false;

                    foreach (var blacklist in temp.BlackList)
                    {
                        if (blacklist.userID == id)
                            IsBlacklisted = true;
                    }
                    if (!IsBlacklisted)
                    followingWall.Add(temp);
                }

                foreach (var wall in followingWall)
                {
                    foreach (var post in wall.postIDs)
                    {
                        posts.Add(_posts.Find(p => p.Id == post).FirstOrDefault());
                    }
                }
            }

            posts.AddRange(GetCirclePosts(id));
            return posts;
        }

        public bool Blacklist(string BlacklisterId, string idToBlacklist)
        {
            User u = Get(BlacklisterId);
            User bUser = Get(idToBlacklist);
            Wall userWall = _walls.Find(w => w.ID == u.Wall).FirstOrDefault();

            if (userWall.BlackList == null)
                userWall.BlackList = new List<blacklistedUser>();

            blacklistedUser b = new blacklistedUser();
            b.userID = idToBlacklist;
            b.userName = bUser.Name;

            userWall.BlackList.Add(b);

            _walls.ReplaceOne(w => w.ID == u.Wall, userWall);

            return true;
        }

        public UserViewModel ConstructViewModel(string id)
        {
            UserViewModel vm = new UserViewModel();

            vm.User = Get(id);
            vm.Followers = GetFollowers(id);
            vm.Following = GetFollowing(id);
            vm.FeedPosts = GetFeedPosts(id);
            vm.Followable = GetFollowable(id);
            vm.Blacklisted = GetBlacklisted(id);
            //vm.UserPosts = GetUserPosts(id);
            vm.Users = Get();

            return vm;
        }

        public List<User> GetFollowing(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            List<User> followers = new List<User>();
            if (model == null)
                return followers;
            if (model.FollowingId != null)
            {
                foreach (var fo in model.FollowingId)
                {
                    followers.Add(_users.Find<User>(user => user.Id == fo).FirstOrDefault());
                }
            }

            return followers;
        }

        public List<User> GetBlacklisted(string id)
        {
            var u = Get(id);
            Wall w = _walls.Find(wa => wa.ID == u.Wall).FirstOrDefault();

            List<User> blackListedUsers = new List<User>();

            if (w.BlackList != null)
                foreach (var b in w.BlackList)
                {
                    blackListedUsers.Add(_users.Find(ui => ui.Id == b.userID).FirstOrDefault());
                }

            return blackListedUsers;
        }

        public List<User> GetFollowers(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            List<User> followers = new List<User>();

            if (model.Followers != null)
            {
                foreach (var fo in model.Followers)
                {
                    followers.Add(_users.Find<User>(user => user.Id == fo).FirstOrDefault());
                }
            }

            return followers;
        }

        public List<Circle> GetCircles(string id)
        {
            var u = Get(id);
            List<Circle> Circles = new List<Circle>();

            if(u.Circles == null)
                u.Circles = new List<string>();

            foreach (var c in u.Circles)
            {
                Circles.Add(_circles.Find(ci => ci.Id == c).FirstOrDefault());
            }

            return Circles;
        }

        public List<Post> GetCirclePosts(string id)
        {
            var c = GetCircles(id);
            List<Post> posts = new List<Post>();
            List<Wall> walls = new List<Wall>();

            foreach (var circle in c)
            {
                walls.Add(_walls.Find(wa => wa.ID == circle.WallId).FirstOrDefault());
            }

            foreach (var wall in walls)
            {
                foreach (var post in wall.postIDs)
                {
                    posts.Add(_posts.Find(p => p.Id == post).FirstOrDefault());
                }
            }


            return posts;
        }

        public List<User> GetFollowable(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            var allUsers = Get();

            List<User> Followable = new List<User>();

            try
            {
                if (allUsers != null) 
                {
                    if (model.FollowingId != null) //If model follows anyone
                        foreach (var user in allUsers)
                        {
                            bool exist = false;
                            foreach (var modelUser in model.FollowingId)
                            {
                                if (modelUser == user.Id || model.Id == user.Id)
                                    exist = true;
                            }
                            if (!exist)
                                Followable.Add(user);
                        }
                    else //If user doesn't follow anyone
                        foreach (var user in allUsers)
                        {
                            if (model.Id != user.Id)
                                Followable.Add(user);
                        }
                }
            }
            catch (NullReferenceException)
            {
                return Followable;
            }

            return Followable;
        }
        
        public User Create(User user)
        {
            user.Password = HashPass(user.Password);
            _users.InsertOne(user);
            return user;
        }

        public void Update(User LoginTestIn)
        {
            LoginTestIn.Password = HashPass(LoginTestIn.Password);
            _users.ReplaceOne(l => l.Id == LoginTestIn.Id, LoginTestIn);
        }

        public void UpdateNotPassword(User LoginTestIn)
        {
           
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
            if (userToLogIn != null && !string.IsNullOrEmpty(userToLogIn.Password) && !string.IsNullOrEmpty(userToLogIn.UserName))
            {
                var pass = HashPass(userToLogIn.Password);
                try { 
                id = _users.Find(u => u.UserName == userToLogIn.UserName && u.Password == pass).FirstOrDefault().Id;
                }
                catch
                {
                    id = "";
                    return false;
                }
                if (string.IsNullOrEmpty(id))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                id = "";
                return false;
            }
        }
    }
}