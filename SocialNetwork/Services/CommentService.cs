using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetwork.Models;

namespace SocialNetWork.Models
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            var database = client.GetDatabase("SocialNetworkDb");
            _comments = database.GetCollection<Comment>("Comments");
        }

        public List<Comment> Get()
        {
            return _comments.Find(comment => true).ToList();
        }

        public Comment Get(string id)
        {
            return _comments.Find<Comment>(comment => comment.Id == id).FirstOrDefault();
        }

        public Comment Create(Comment comment)
        {
            _comments.InsertOne(comment);
            return comment;
        }

        public void Update(string id, Comment commentIn)
        {
            _comments.ReplaceOne(comment => comment.Id == id, commentIn);
        }

        public void Remove(Comment commentIn)
        {
            _comments.DeleteOne(comment => comment.Id == commentIn.Id);
        }

        public void Remove(string id)
        {
            _comments.DeleteOne(comment => comment.Id == id);
        }
    }
}