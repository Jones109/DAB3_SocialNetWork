using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SocialNetWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetWork.Controllers
{

   
    public class PostController : Controller
    {
        private PostService _postService;
        private UserService _userService;
        private WallService _wallService;
        

        public PostController(PostService postService, UserService userService, WallService wallService)
        {
            _postService = postService;
            _userService = userService;
            _wallService = wallService;

        }

        // GET: Post
        public ActionResult Index()
        {
            return View(_postService.Get());
        }

        // GET: Post/Details/5
        public ActionResult Details(string id)
        {
            return View();
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string text, IFormFile file)
        {
            try
            {
                string currentUserId = HttpContext.Session.GetString("UserId");
                User currentUser = _userService.Get(currentUserId);

                Post post = new Post();

                post.OwnerId = currentUserId;
                post.Text = text;
                post.WallId = currentUser.Wall;
                post.Comments = new List<Comment>();
                post.CreationTime= DateTime.Now;
                post.OwnerName = currentUser.UserName;

                if (file != null)
                {
                    string path = null;
                    if (UploadPicture(file, ref path))
                        post.ImgUri = path;
                }

                Post createdPost = _postService.Create(post);

                Wall newWall = _wallService.GetByWallId(createdPost.WallId);
                if (newWall.postIDs==null)
                {
                    newWall.postIDs = new List<string>();
                }
               

                newWall.postIDs.Add(new string(createdPost.Id));

                _wallService.Update(newWall);

                return RedirectToAction("Feed", "User");
            }
            catch(Exception e)
            {
                return View();
            }
        }


        public ActionResult CreateForCircle()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForCircle(string wallId ,string text, IFormFile file)
        {
            try
            {
                string currentUserId = HttpContext.Session.GetString("UserId");
                User currentUser = _userService.Get(currentUserId);

                Post post = new Post();
                post.OwnerId = currentUserId;
                post.Text = text;
                post.WallId = wallId;
                post.Comments = new List<Comment>();
                post.CreationTime = DateTime.Now;
                post.OwnerName = currentUser.UserName;

                if (file != null)
                {
                    string path = null;
                    if (UploadPicture(file, ref path))
                        post.ImgUri = path;
                }

                Post createdPost = _postService.Create(post);

                Wall newWall = _wallService.GetByWallId(post.WallId);
                if (newWall.postIDs == null)
                {
                    newWall.postIDs = new List<string>();
                }


                newWall.postIDs.Add(new string(createdPost.Id));

                _wallService.Update(newWall);

                return RedirectToAction("Feed", "User");
            }
            catch (Exception e)
            {
               return View();
            }
        }

        // GET: Post/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Post post)
        {
            try
            {

                Post editedPost = _postService.Get(id);
                if (post.ImgUri != null)
                {
                    editedPost.ImgUri = post.ImgUri;
                }

                if (post.Text != null)
                {
                    editedPost.Text = post.Text;
                }

                _postService.Update(id, editedPost);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, Post post)
        {
            try
            {
                _postService.Remove(post);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool UploadPicture(IFormFile file, ref string path)
        {
            //Check if file is selected
            if (file == null || file.Length == 0)
                return false; //Content("File not selected");

            //Check for correct filetype
            string extension = Path.GetExtension(file.FileName);
            if (extension != ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                return false; //Content("Incorrect filetype");

            //Check for filesize > 4mb
            if (file.Length > 400000)
                return false; // Content("Too large file");

            //Add timestamp to filename
            var timeStamp = DateTime.Now.ToShortTimeString();
            var fileName = timeStamp.Replace(':', '.') + file.FileName;

            CurrentDirectoryHelpers.SetCurrentDirectory();
            
            //Combine filepath
            var imgUrl = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/PostPictures", fileName);

            //Write code to file
            using (var stream = new FileStream(imgUrl, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            path = imgUrl;

            return true;
        }
    }
}