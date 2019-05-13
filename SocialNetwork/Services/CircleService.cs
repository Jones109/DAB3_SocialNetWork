using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public List<Circle> Get()
        {
            return _circles.Find(circle => true).ToList();
        }

        public Circle Get(string id)
        {
            return _circles.Find<Circle>(circle => circle.Id == id).FirstOrDefault();
        }

        public Circle Create(Circle circle)
        {
            _circles.InsertOne(circle);
            return circle;
        }

        public void Update(string id, Circle userIn)
        {
            _circles.ReplaceOne(circle => circle.Id == id, userIn);
        }

        public void Remove(Circle circleIn)
        {
            _circles.DeleteOne(circle => circle.Id == circleIn.Id);
        }

        public void Remove(string id)
        {
            _circles.DeleteOne(circle => circle.Id == id);
        }

    }
}
