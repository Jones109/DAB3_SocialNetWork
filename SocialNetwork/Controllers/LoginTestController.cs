using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System.Web;

namespace SocialNetwork.Controllers
{
    public class LoginTestController : Controller
    {
        private LoginTestService _loginTestService;
        public LoginTestController(LoginTestService service)
        {
            _loginTestService = service;
        }
        // GET: LoginTest
        public ActionResult Index()
        {
            return View(_loginTestService.Get());
        }

        // GET: LoginTest/Details/5
        public ActionResult Details(string id)
        {
            return View(_loginTestService.Get(id));
        }

        // GET: LoginTest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginTest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoginTest newUser)
        {
            try
            {
                _loginTestService.Create(newUser);

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
        public ActionResult Register(string lastUrl, LoginTest newUser)
        {
            try
            {
                _loginTestService.Create(newUser);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginTest/Edit/5
        public ActionResult Edit(string id)
        {
            return View(_loginTestService.Get(id));
        }

        // POST: LoginTest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, LoginTest updatedUser)
        {
            try
            {
                updatedUser.userID = id;
                _loginTestService.Update(updatedUser);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                Debug.WriteLine("failed");
                return View();
            }
        }

        // GET: LoginTest/Delete/5
        public ActionResult Delete(string id)
        {

            return View(_loginTestService.Get(id));
        }

        // POST: LoginTest/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, LoginTest userToDelete)
        {
            try
            {
                _loginTestService.Remove(id);

                return RedirectToAction(nameof(Index));
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
        public ActionResult Login(string id, LoginTest userToLogin)
        {
            string idUser;
            bool canLogIn = _loginTestService.Login(userToLogin,out idUser);
            if (canLogIn)
            {
                HttpContext.Session.Set("UserId", System.Text.Encoding.ASCII.GetBytes(idUser));
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                return RedirectToAction(id.Split('-')[1], id.Split('-')[0]);
            }
            else
            {
                return View();
            }
        }

    }
}