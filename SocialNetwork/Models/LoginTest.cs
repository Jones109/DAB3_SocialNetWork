using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace SocialNetwork.Models
{
    public class LoginTest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string userID {get; set;}

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement]
        [BsonRequired]
        public string userName { get; set; }

        [BsonElement]
        [BsonRequired]
        public string password { get; set; }

        [BsonElement("Following")]
        public List<follower> FollowingId { get; set; }

        [BsonElement("Followers")]
        public List<follower> Followers { get; set; }

        [BsonElement("Circles")]
        public List<follower> Circles { get; set; }

        [BsonElement("Wall")]
        public string Wall { get; set; }
    }
}
