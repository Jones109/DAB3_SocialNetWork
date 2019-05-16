using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetWork.Models;

namespace SocialNetwork.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Followers = new List<User>();
            Following = new List<User>();
            FeedPosts = new List<Post>();
            Followable = new List<User>();
        }
        public List<User> Followers { get; set; }
        public List<User> Following { get; set; }
        public List<Post> FeedPosts { get; set; }
        public User User { get; set; }
        public List<User> Users { get; set; }
        public List<User> Followable { get; set; }
        public List<Post> UserPosts { get; set; }
    }
}
