using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialNetwork.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("OwnerName")]
        public string OwnerName { get; set; }


        [BsonElement("Post")]
        public string Post { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("LastEdited")]
        public DateTime LastEdited { get; set; }

        [BsonElement("IsEdited")]
        public bool IsEdited { get; set; }

    }
}
