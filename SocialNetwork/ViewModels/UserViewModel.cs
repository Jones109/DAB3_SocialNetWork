using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class UserViewModel
    {
        public List<User> Followers { get; set; }
        public List<User> Following { get; set; }
        public User User { get; set; }
    }
}
