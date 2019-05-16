using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetWork.Models;

namespace SocialNetwork.Controllers
{
    public class CommentController : Controller
    {
        private CommentService _commentService;
        private PostService _postService;
        private UserService _userService;

        public CommentController(CommentService commentService, PostService postService, UserService userService)
        {
            _userService = userService;
            _commentService = commentService;
            _postService = postService;
        }

        // GET: Comment
        public ActionResult Index()
        {
            return View(_commentService.Get());
        }

        // GET: Comment/Details/5
        public ActionResult Details(string id)
        {
            return View();
        }


        // GET: Post/Create
        public ActionResult Create(string PostId)
        {
            Comment comment = new Comment();
            comment.Post = PostId;

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string PostId, Comment comment)
        {
            try
            {
                comment.LastEdited = DateTime.Now;
                comment.Post = PostId;
                string currentUserId = HttpContext.Session.GetString("UserId");
                User currentUser = _userService.Get(currentUserId);
                comment.OwnerName = currentUser.Name;
                comment.OwnerId = currentUserId;
  

                Comment newComment = _commentService.Create(comment);

                Post newPost = _postService.Get(comment.Post);


                newPost.Comments.Add(newComment);

                _postService.Update(comment.Post, newPost);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            }
        }

 

        // GET: Comment/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Comment comment)
        {
            try
            {
                Comment editComment = _commentService.Get(comment.Id);

                Post post = _postService.Get(editComment.Post);
                var comments = post.Comments;

                int index = comments.FindIndex(x => x.Id == editComment.Id);

                comments.ElementAt(index).Text = comment.Text;
                comments.ElementAt(index).IsEdited = true;
                comments.ElementAt(index).LastEdited = DateTime.Now;
                
                _commentService.Update(id, comments.ElementAt(index));

                post.Comments = comments;

                _postService.Update(post.Id, post);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, Comment comment)
        {
            try
            {
                _commentService.Remove(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}