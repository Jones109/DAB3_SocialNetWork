using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SocialNetwork.Models;

namespace SocialNetWork.Models
{
    public class Post
    {
        
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("WallId")]
            public string WallId { get; set; }

            [BsonElement("OwnerName")]
            public string OwnerName { get; set; }

            [BsonElement("OwnerId")]
            public string OwnerId { get; set; }

        [BsonElement("CreationTime")]
            public DateTime CreationTime { get; set; }

            [BsonElement("Text")]
            public string Text { get; set; }

            [BsonElement("Comments")]
            public List<Comment> Comments { get; set; }

            [BsonElement("ImgUri")]
            public string ImgUri { get; set; }


    }
}
