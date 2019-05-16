using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class CircleService
    {
        private readonly IMongoCollection<Circle> _circles;

        public CircleService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _circles = database.GetCollection<Circle>("circles");
        }

        public string GetLoggedInUserId()
        {
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            return httpContextAccessor.HttpContext.Session.GetString("UserId");
        }

        public List<Circle> Get()
        {
            return _circles.Find(circle => true).ToList();
        }

        public List<Circle> GetCirclesForUser(User user)
        {
            var circleIds = user.Circles;
            
            return null;
        }

        public Circle Get(string id)
        {
            return _circles.Find<Circle>(circle => circle.Id == id).FirstOrDefault();
        }

        public string Create(Circle circle)
        {
            _circles.InsertOne(circle);
            return circle.Id;
        }

        public void Update(string id, Circle userIn)
        {
            _circles.ReplaceOne(circle => circle.Id == id, userIn);
        }

        public void Remove(Circle circleIn)
        {
            _circles.DeleteOne(circle => circle.Id == circleIn.Id);
        }

        public bool DeleteWhereCircleIsNull()
        {
            try
            {
                _circles.DeleteOne(circle => circle.Name == null);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }

        public void Remove(string id)
        {
            _circles.DeleteOne(circle => circle.Id == id);
        }


        

    }
}
