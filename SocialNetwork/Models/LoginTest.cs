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

        [BsonElement]
        [BsonRequired]
        public string userName { get; set; }

        [BsonElement]
        [BsonRequired]
        public string password { get; set; }
    }
}
