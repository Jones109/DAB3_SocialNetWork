﻿using System.Collections.Generic;
using System.Linq;
using SocialNetWork.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _users;
        private readonly IMongoCollection<Wall> _users;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _users = database.GetCollection<User>("Users");
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

            if (model.FollowingId != null)
            {
                foreach (var fo in model.FollowingId)
                {
                    followers.Add(_users.Find<User>(user => user.Id == fo).FirstOrDefault());
                }
            }

            return followers;
        }

        public List<Post> GetFeedPosts(string id)
        {
            List<Post> posts = new List<Post>();

            posts = 

        }

        public List<User> GetFollowers(string id)
        {
            var model = _users.Find<User>(user => user.Id == id).FirstOrDefault();

            List<User> followers = new List<User>();

            if (model.FollowerId != null)
            {
                foreach (var fo in model.FollowerId)
                {
                    followers.Add(_users.Find<User>(user => user.Id == fo).FirstOrDefault());
                }
            }

            return followers;
        }
        
        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(user => user.Id == id);
        }
    }
}