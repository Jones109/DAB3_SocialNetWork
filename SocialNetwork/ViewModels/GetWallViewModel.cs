using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Models;
using SocialNetWork.Models;

namespace SocialNetwork.ViewModels
{
    public class GetWallViewModel
    {
        public List<Post> posts = new List<Post>();
        public Wall wall = new Wall();
    } 
}
