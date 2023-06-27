using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Data;
using ShopWebApplication1.Models;
using ShopWebApplication1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;

namespace ShopWebApplication1.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyshopContext _context;

        public UsersController(MyshopContext context)
        {
            _context = context;
        }


        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email,Password")] User user)
        {
            if (ModelState["Email"].ValidationState == ModelValidationState.Valid &&
                ModelState["Password"].ValidationState == ModelValidationState.Valid)
            {
                var UCheck = new UserCheck();
                if (!UCheck.UserExist(user.Email))
                {
                    var str = UCheck.MakeUserToken(user.Email, user.Password);
                    user.Token = str;
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    UCheck.WriteClientUserToken(str);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    ModelState.AddModelError("Email", "Такий e-mail вже існує.");
                }
            }
            return View(user);
        }

        // GET: Users/Login
        public async Task<IActionResult> Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user)
        {
            if (ModelState["Email"].ValidationState == ModelValidationState.Valid &&
                ModelState["Password"].ValidationState == ModelValidationState.Valid)
            {
                var UCheck = new UserCheck();
                if (UCheck.CheckLoginUser(user.Email, user.Password))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Не вірний логін або пароль.");
                }
            }
            return View(user);
        }

        // GET: Users/Logout
        public async Task<IActionResult> Logout()
        {
            var UCheck = new UserCheck();
            UCheck.DeleteClientUserToken();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Users/Profile
        public async Task<IActionResult> Profile()
        {
            var profile = new ViewProfile();
            var uid = new UserCheck().GetCurUserId();
            if (uid > 0)
            {
                var user = await _context.Users.FindAsync(uid);
                if (user != null)
                {
                    profile.Name = user.Name;
                    profile.Surname = user.Surname;
                    profile.Patronymic = user.Patronymic;
                    profile.Address = user.Address;
                    profile.PhoneNumber = user.PhoneNumber;
                }
            }
            ViewBag.EnableMakeOrderBtn = false;
            return View(profile);
        }

        // POST: Users/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("Name,Surname,Patronymic,Address,PhoneNumber")] ViewProfile profile)
        {
            ViewBag.EnableMakeOrderBtn = false;
            if (ModelState.IsValid)
            {
                var uid = new UserCheck().GetCurUserId();
                if (uid > 0)
                {
                    var tbluser = await _context.Users.FindAsync(uid);
                    if (tbluser != null)
                    {
                        tbluser.Name = profile.Name;
                        tbluser.Surname = profile.Surname;
                        tbluser.Patronymic = profile.Patronymic;
                        tbluser.Address = profile.Address;
                        tbluser.PhoneNumber = profile.PhoneNumber;
                        _context.Update(tbluser);
                        await _context.SaveChangesAsync();
                        ViewBag.EnableMakeOrderBtn = true;
                    }
                }
            }
            return View(profile);
        }

        // GET: Basketts/GotoMakeNewOrder
        public async Task<IActionResult> GotoMakeNewOrder()
        {
            return RedirectToAction("MakeNewOrder", "Orders");
        }

        //==========================================================================
        public async Task<IActionResult> Test()
        {
 //           var UCheck = new UserCheck(_context, HttpContext);
            var UCheck = new UserCheck();
            string msg = "No Token!";
            string Token;
            if (!UCheck.ReadClientUserToken(out Token))
            {
 //               Token = UCheck.MakeUserToken("aaa", "bbb");
 //               UCheck.WriteClientUserToken(Token);
            }
            else 
            {
                msg = "Token:";
                //               msg = "Readed Token. Delete Token!";
                //               UCheck.DeleteClientUserToken();
            }
            //await Response.WriteAsync("Hello World!");
            ViewBag.Msg = msg;
            ViewBag.Token = Token;
            return View();
        }
    }
}

