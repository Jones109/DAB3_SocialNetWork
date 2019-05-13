using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialNetwork.Models
{
    public class Wall
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("ownerID")]
        public string ownerID { get; set; }

        [BsonElement("postIDs")]
        public string postIDs { get; set; }

        [BsonElement("Followers")]
        public List<follower> Followers { get; set; } // User IDs
        
        [BsonElement("owner")]
        public string owner { get; set; } // owner ID

        [BsonElement("BlackList")]
        public List<blacklistedUser> BlackList { get; set; } // user IDs

        [BsonElement("type")]
        public string type { get; set; } // type of owner
    }

    public class follower
    {
        public string followerID { get; set; }
        public string followerName { get; set; }
    }
    public class blacklistedUser
    {
        public string userID { get; set; }
        public string userName { get; set; }
    }
}
