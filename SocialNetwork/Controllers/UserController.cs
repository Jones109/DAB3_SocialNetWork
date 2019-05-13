using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetWork.Models;

namespace SocialNetwork.Controllers
{

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

        public IActionResult Index(string id)
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
            return View(_userService.ConstructViewModel(id));
        }

        public IActionResult Follow(string id)
        {
            return View(_userService.ConstructViewModel(id));
        }

        public IActionResult FollowPost(string id, string idToFollow)
        {
            if (_userService.Follow(idToFollow, id))
            return (RedirectToAction("Details", new {id = id}));
            return Content("Could not add follower");
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

            _userService.Update(userIn);

            return NoContent();
        }

        // GET: LoginTest/Delete/5
        public ActionResult Delete(string id)
        {

            return View(_userService.Get(id));
        }

        // POST: LoginTest/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, LoginTest userToDelete)
        {
            try
            {
                _userService.Remove(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Register(string lastUrl)
        {
            return View();
        }

        // POST: LoginTest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User newUser)
        {
            try
            {
                _userService.Create(newUser);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Login(string id)
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User userToLogin)
        {
            string idUser;
            bool canLogIn = _userService.Login(userToLogin, out idUser);
            if (canLogIn)
            {
                HttpContext.Session.Set("UserId", System.Text.Encoding.ASCII.GetBytes(idUser));
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                return Redirect("/User/Feed/" + idUser);
            }
            else
            {
                return View();
            }
        }
    }
}