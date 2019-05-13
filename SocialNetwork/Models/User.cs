using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialNetwork.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Following")]
        public List<string> FollowingId { get; set; }

        [BsonElement("Followers")]
        public List<string> FollowerId { get; set; }

        [BsonElement("Circles")]
        public List<string> Circles{ get; set; }

        [BsonElement("Wall")]
        public string Wall { get; set; }
    }
}