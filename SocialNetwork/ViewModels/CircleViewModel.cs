using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Models;
using SocialNetWork.Models;

namespace SocialNetwork.ViewModels
{
    public class CircleViewModel
    {
        public Circle Circle { get; set; }
        public Wall Wall { get; set; }
        public List<Post> Posts { get; set; }
    }
}
