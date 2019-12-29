using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor_Network.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Runtime.Caching;
using System.Collections;

namespace Neighbor_Network.Controllers
{
    public class UserController : Controller
    {
        public static string keyword;
        private const string CacheKey = "userId";
        // GET: User
        public ActionResult Index()
        {
            return View("Login");
        }

        // GET: User/Details/5
        public ActionResult HomePage(int id)
        {
            ViewBag.ID = 0;
            ViewBag.ID2 = 0;
            return View("HomePage");
        }

        public ActionResult GetID(int id)
        {
            ViewBag.ID = id;
            return View("HomePage");
        }

        public ActionResult GetID2(int id)
        {
            ViewBag.ID2 = id;
            return View("HomePage");
        }

        // GET: User/Create
        public ActionResult ShowProfile(string name, string lname, string photo, string bio, string gender, string street, string city, string state, string mid)
        {
            ViewBag.Fname = name;
            ViewBag.Lname = lname;
            ViewBag.Photo = photo;
            ViewBag.Bio = bio;
            ViewBag.Gender = gender;
            ViewBag.Street = street;
            ViewBag.City = city;
            ViewBag.State = state;
            ViewBag.Mid = mid;
            return View("Results");
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(User user)
        {
           // try
         //   {
                // TODO: Add insert logic here
                using(var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        string ps = HashPassword(user.Pass);
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "insert_user";
                        cmd.Parameters.AddWithValue("phonep",user.Phone);
                        cmd.Parameters.AddWithValue("streetp", user.Street);
                        cmd.Parameters.AddWithValue("cityp", user.City);
                        cmd.Parameters.AddWithValue("statep", user.State);
                        cmd.Parameters.AddWithValue("unamep", user.Uname);
                        cmd.Parameters.AddWithValue("passp", ps);
                        cmd.Parameters.AddWithValue("fnamep", user.Fname);
                        cmd.Parameters.AddWithValue("lnamep", user.Lname);
                        cmd.Parameters.AddWithValue("genderp", user.Gender);
                        cmd.ExecuteNonQuery();
                    }
                }
                ViewBag.Message = "Success";
                return RedirectToAction(nameof(Index));
          //  }
          //  catch
          //  {
         //       ViewBag.Message = "An unknown exception has ocurred";
              //  return RedirectToAction(nameof(Index));
          //  }
        }

        // GET: User/Edit/5
        public string[] GetProfile(int uidp)
        {
            string[] profile = new string[2];
           try
           {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select * from Profiles p inner join Users u on p.pid = u.pid where uid = ?uidp";
                        cmd.Parameters.AddWithValue("uidp", uidp);
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                profile[0] = dr["bio"].ToString();
                                profile[1] = dr["photo"].ToString();
                            }
                        }
                    }
                }
            }
            catch{
                profile = null;
            }
            return profile;
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            try
            {
                bool eval = false;
                int id = -1;
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select uid, uname, pass from Users where uname = ?unamep";
                        cmd.Parameters.AddWithValue("unamep", user.Uname);
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                eval = ValidatePassword(dr["pass"].ToString(), user.Pass);
                                if (eval)
                                {
                                    id = Convert.ToInt32(dr["uid"]);
                                    CacheItemPolicy cpolicy = new CacheItemPolicy();
                                    cpolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                                    ObjectCache cache = MemoryCache.Default;
                                    cache.Add(CacheKey, id, cpolicy);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (eval)
                {
                    return RedirectToAction(nameof(HomePage));
                }
                else
                {
                    ViewBag.Message = "Incorrect username or password";
                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                return RedirectToAction(nameof(Index)); 
            }
        }

        // GET: User/Delete/5
        public ActionResult Logout()
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(CacheKey);
            return View("Login");
        }

        public ActionResult ShowResults(string searchBox)
        {
            keyword = searchBox;
            ViewBag.Mid = 0;
            return View("Results");
        }

        public void ClearCache()
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(CacheKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendRequest(string app, string id1)
        {
            try
            {
                // TODO: Add delete logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "send_request";
                        cmd.Parameters.AddWithValue("id1", app);
                        cmd.Parameters.AddWithValue("id2", id1);
                        cmd.ExecuteNonQuery();
                    }
                }
                return View("HomePage");
            }
            catch
            {
                return View("HomePage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept(string myID, string friend)
        {
         //   try
          //  {
                // TODO: Add delete logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "accept_request";
                        cmd.Parameters.AddWithValue("id1", friend);
                        cmd.Parameters.AddWithValue("id2", myID);
                        cmd.ExecuteNonQuery();
                    }
                }
                return View("HomePage");
           // }
         //   catch
          //  {
            //    return View("HomePage");
           // }
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProfile(Parent prf)
        {
            try
            {
                // TODO: Add delete logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    int pid = -1;
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "insert_profile";
                        cmd.Parameters.AddWithValue("biop", prf.Profile.Bio);
                        cmd.Parameters.AddWithValue("picp", prf.Profile.Pic);
                        cmd.ExecuteNonQuery();
                    }
                    using(var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select max(pid) from Profiles";
                        pid = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    using(var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "update Users set pid = ?p where uid = ?uidp";
                        cmd.Parameters.AddWithValue("p", pid);
                        cmd.Parameters.AddWithValue("uidp", CheckSession());
                        cmd.ExecuteNonQuery();
                    }
                }
                return View("HomePage");
           }
           catch
            {
                return View("HomePage");
            }
        }

        public string HashPassword(string x)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(x, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string passHash = Convert.ToBase64String(hashBytes);
            return passHash;
        }

        public bool ValidatePassword(string x, string y)
        {
            bool result = true;
            byte[] hashBytes = Convert.FromBase64String(x);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(y, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            for(int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    result = false;
            }
            return result;
        }

        public int CheckSession()
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(CacheKey))
                return Convert.ToInt32(cache.Get(CacheKey));
            else
                return -1;
        }

        public DataTable GetPeople(int id)
        {

            DataTable peopleList = new DataTable();
            try
            {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Get_People";
                        cmd.Parameters.AddWithValue("uidp", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(peopleList);
                        }
                        conn.Close();
                    }

                }
                return peopleList;
            }

            catch
            {
                return null;
            }
        }
        public DataTable GetRequests(int id)
        {

            DataTable peopleList = new DataTable();
            try
            {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "get_requests";
                        cmd.Parameters.AddWithValue("id1", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(peopleList);
                        }
                        conn.Close();
                    }

                }
                return peopleList;
            }

            catch
            {
                return null;
            }
        }

        public DataTable GetApplcations(int id)
        {

            DataTable applicationList = new DataTable();
            try
            {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Get_Applications";
                        cmd.Parameters.AddWithValue("midp", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(applicationList);
                        }
                        conn.Close();
                    }

                }
                return applicationList;
            }

            catch
            {
                return null;
            }
        }

        public DataTable SearchUser(string search1)
        {

            DataTable userList = new DataTable();
            try
            {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "search_people";
                        cmd.Parameters.AddWithValue("keyword", search1);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(userList);
                        }
                        conn.Close();
                    }

                }
                return userList;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(string id1, string app, string adate)
        {
        //   try
           // {
                // TODO: Add delete logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Approve";
                        cmd.Parameters.AddWithValue("midp", id1);
                        cmd.Parameters.AddWithValue("uidp", app);
                        cmd.Parameters.AddWithValue("adate", adate);
                        cmd.ExecuteNonQuery();
                    }
                }
                return View("HomePage");
           // }
          //  catch
          //  {
            //    return View("HomePage");
            //}
        }
    }
}