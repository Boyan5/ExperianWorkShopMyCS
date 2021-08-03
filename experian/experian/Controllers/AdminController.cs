using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using experian.Areas.Identity.Data;
using experian.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using experian.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace experian.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IServiceProvider _myservice;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<appUser> _userManager;

        public virtual appUser newUser { get; set; }
        public IActionResult Index()
        {
            return View();
        }

        

        public AdminController(IServiceProvider service, ApplicationDbContext db, UserManager<appUser> userManager)
        {
            _myservice = service;
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("ListUsers");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                //_db.SaveChanges();
                return View("ListUsers");
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
            //_db.SaveChanges();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                //_db.SaveChanges();
                return View(model);
            }
        }

        public IActionResult CreateUser(IFormCollection form) 
        {
            var UserManager = _myservice.GetRequiredService<UserManager<appUser>>();
            string username = form["txtEmail"];
            string email = form["txtEmail"];
            string passw = form["txtPassword"];

            if (!IsValidEmail(email))
            {
                return View("CreateUser");
            }

            if (!ValidatePassword(passw))
            {
                return View("CreateUser");
            }

            ViewBag.Error = false;
            var emailAlreadyExists = _db.Users.Any(x => x.Email == email);
            if (emailAlreadyExists)
            {
                ViewBag.Error = true;
                ViewBag.ErrorText = "Email already exists";
                return View("Index");
            }

            newUser = new appUser();
            newUser.UserName = username;
            newUser.Email = email;
            newUser.EmailConfirmed = true;

            var admin = UserManager.CreateAsync(newUser, passw);
            ViewBag.User = username;
            try
            {
                _db.Users.Add(newUser);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return View("Index");
            }
            
            
            return View("Index");
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        static bool ValidatePassword(string passWord)
        {
            if (passWord.Length < 6)
            {
                return false;
            }
            int validConditions = 0;
            foreach (char c in passWord)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validConditions++;
                    break;
                }
            }
            foreach (char c in passWord)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    validConditions++;
                    break;
                }
            }
            if (validConditions == 0) return false;
            foreach (char c in passWord)
            {
                if (c >= '0' && c <= '9')
                {
                    validConditions++;
                    break;
                }
            }
            if (validConditions == 1) return false;
            if (validConditions == 2)
            {
                char[] special = { '@', '#', '$', '%', '^', '&', '+', '=', '!', '*' };    
                if (passWord.IndexOfAny(special) == -1) return false;
            }
            return true;
        }

    }
}
