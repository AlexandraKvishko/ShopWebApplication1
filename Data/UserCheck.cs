using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Models;

namespace ShopWebApplication1.Data
{
    public class UserCheck
    {
        private readonly MyshopContext _context;
        private readonly HttpContext _httpContext;
//        private readonly HttpContextAccessor _httpContextAccessor;

        public UserCheck()
        {
            var _httpContextAccessor = new HttpContextAccessor();
            _httpContext = _httpContextAccessor.HttpContext;
            _context = new MyshopContext();
        }

        public UserCheck(MyshopContext context, HttpContext httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        //----------------------------------------------------------------------------
        public string LockStr(string str)
        {
            return str;
        }

        public string MakeUserToken(string login, string password)
        {
            string token = login + password;
            return token;
        }
        //----------------------------------------------------------------------------

        public bool ReadClientUserToken(out string Token)
        {
            Token = null;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                Token = _httpContext.Request.Cookies["UserToken"];
                return true;
            }
            return false;
        }

        public void WriteClientUserToken(string Token)
        {
            _httpContext.Response.Cookies.Append("UserToken", Token);
        }

        public void DeleteClientUserToken()
        {
            _httpContext.Response.Cookies.Delete("UserToken");
        }

        //============================================================================
        public bool UserExist(string Email) 
        {
            var curuser = _context.Users.Where(d => d.Email == Email).FirstOrDefault();
            if (curuser != null) return true;
            return false;
        }

        public bool CheckLoginUser(string login, string password)
        {
            bool res = false;
            string login_token = MakeUserToken(login, password);
            var tbluser = _context.Users.Where(d => d.Token == login_token).FirstOrDefault();
            if ((tbluser != null) && (login_token == tbluser.Token))
            {
                WriteClientUserToken(login_token);
                res = true;
            }
            return res;
        }

        public bool IsUserLogin()
        {
            bool res = false;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                string cookies_token = _httpContext.Request.Cookies["UserToken"];
                if (cookies_token != null)
                {
                    var tbluser = _context.Users.Where(d => d.Token == cookies_token).FirstOrDefault();
                    if ((tbluser != null) && (tbluser.Token == cookies_token)) res = true;
                }
            }
            return res;
        }

        public int GetCurUserId() 
        {
            int id = 0;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                string cookies_token = _httpContext.Request.Cookies["UserToken"];
                if (cookies_token != null)
                {
                    var tbluser = _context.Users.Where(d => d.Token == cookies_token).FirstOrDefault();
                    if ((tbluser != null) && (tbluser.Token == cookies_token)) id = tbluser.Id;
                }
            }
            return id;
        }

        public string GetCurUserName()
        {
            string name = null;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                string cookies_token = _httpContext.Request.Cookies["UserToken"];
                if (cookies_token != null)
                {
                    var tbluser = _context.Users.Where(d => d.Token == cookies_token).FirstOrDefault();
                    if ((tbluser != null) && (tbluser.Token == cookies_token)) name = tbluser.Name;
                }
            }
            return name;
        }

        public bool IsCurUserBaskettFill()
        {
            bool BaskettFill = false;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                string cookies_token = _httpContext.Request.Cookies["UserToken"];
                if (cookies_token != null)
                {
                    var tbluser = _context.Users.Where(d => d.Token == cookies_token).FirstOrDefault();
                    if ((tbluser != null) && (tbluser.Token == cookies_token)) 
                    {
                        var BaskettList = _context.Basketts.Where(d => d.UserId == tbluser.Id).Count();
                        if (BaskettList != 0) BaskettFill = true;
                    }
                }
            }
            return BaskettFill;
        }

        public int GetCurUserRole()
        {
            int role = 0;
            if (_httpContext.Request.Cookies.ContainsKey("UserToken"))
            {
                string cookies_token = _httpContext.Request.Cookies["UserToken"];
                if (cookies_token != null)
                {
                    var tbluser = _context.Users.Where(d => d.Token == cookies_token).FirstOrDefault();
                    if ((tbluser != null) && (tbluser.Token == cookies_token)) role = tbluser.Role;
                }
            }
            return role;
        }
    }
}
