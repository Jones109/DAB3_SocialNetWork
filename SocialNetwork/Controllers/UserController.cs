using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetWork.Models;

namespace SocialNetwork.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly UserViewModel _vm;
        
        public UserController(UserService userService, PostService postService)
        {
            _userService = userService;
            _postService = postService;
            _vm = new UserViewModel();
        }

        public IActionResult Index()
        {
            var users = _userService.Get();

            return View(users);
        }

        public IActionResult Feed(string id)
        {
            var model = _userService.Get(id);

            return View(model);
        }

        public IActionResult Details(string id)
        {
            _vm.User = _userService.Get(id);
            _vm.Followers = _userService.GetFollowers(id);
            _vm.Following = _userService.GetFollowing(id);

            return View(_vm);
        }

        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            _userService.Create(user);

            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, userIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);

            return NoContent();
        }
    }
}