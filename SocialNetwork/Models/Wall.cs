using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [BsonElement("followers")]
        public string followers { get; set; } // User IDs

        [BsonElement("owner")]
        public string owner { get; set; } // owner ID

        [BsonElement("blackList")]
        public string blackList { get; set; } // user IDs

        [BsonElement("type")]
        public string type { get; set; } // type of owner
    }
}
