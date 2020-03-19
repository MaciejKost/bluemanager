using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlueManager.Models;
using BlueManagerPlatform.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace BlueManagerCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        //string cs = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //Check the user name and password
            //Here can be implemented checking logic from the database
            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            using (var con = new SqlConnection(_configuration.GetConnectionString("BlueManagerContext")))
            // using (var con = new SqlConnection(cs))
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM login WHERE username=@username AND password=@password";
                    cmd.Parameters.Add(new SqlParameter("@username", login.Username));
                    cmd.Parameters.Add(new SqlParameter("@password", login.Password));

                    dr = cmd.ExecuteReader();

                    //if (userName == "Admin" && password == "password")
                    if (dr.Read())
                    {
                        var username = dr["username"].ToString();
                        //Create the identity for the user
                        identity = new ClaimsIdentity(new[] {
                                   new Claim(ClaimTypes.Name, username),
                                   new Claim(ClaimTypes.Role, "Admin")
                                   }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;

                    }

                    //if (userName == "Mukesh" && password == "password")
                    //{
                    //    //Create the identity for the user
                    //    identity = new ClaimsIdentity(new[] {
                    //        new Claim(ClaimTypes.Name, userName),
                    //        new Claim(ClaimTypes.Role, "User")
                    //    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    //    isAuthenticated = true;
                    //}

                    if (isAuthenticated)
                    {
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Nieprawidłowe dane logowania");

                    return View();

                }
                finally
                {
                    con.Close();
                }

            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // [HttpPost]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
