using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB;
using MongoDB.Driver;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class WallService
    {
        private readonly IMongoCollection<Wall> _wall;

        public WallService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _wall = database.GetCollection<Wall>("Walls");
        }

        public List<Wall> Get()
        {
            return _wall.Find(wall => true).ToList();
        }

        public Wall Get(string ownerID)
        {
            return _wall.Find(wall => wall.ownerID == ownerID).FirstOrDefault();
        }

        public Wall GetByWallId(string wallID)
        {
            return _wall.Find(wall => wall.ID == wallID).FirstOrDefault();
        }

        public Wall Create(Wall wall)
        {
            _wall.InsertOne(wall);
            return (wall);
        }

        public void Update(Wall wall)
        {
            _wall.ReplaceOne(w => w.ID == wall.ID, wall);
        }

        public void Remove(Wall wall)
        {
            _wall.DeleteOne(w => w.ID == wall.ID);
        }

        public void Remove(string wallID)
        {
            _wall.DeleteOne(w => w.ID == wallID);
        }

    }
}
