using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Neighbor_Network.Controllers
{
    public class BlockController : Controller
    {
        public static List<int> mCount = new List<int>();
        // GET: Block
        public ActionResult Index()
        {
            return View("HomePage");
        }

        // GET: Block/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Block/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Block/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinBlock(string blockId, string userId)
        {
            try
            {
                // TODO: Add insert logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Join_Block";
                        cmd.Parameters.AddWithValue("uidp", int.Parse(userId));
                        cmd.Parameters.AddWithValue("blidp", int.Parse(blockId));
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Block/Edit/5
        public ActionResult Edit(int id)
        {

            return View();
        }

        // POST: Block/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Block/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Block/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveBlock(string xid)
        {
          //  try
           // {
                // TODO: Add insert logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Leave_Block";
                        cmd.Parameters.AddWithValue("midp", int.Parse(xid));
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
         //   }
         //   catch
         //   {
             //   return RedirectToAction(nameof(Index));
          //  }
        }

        public DataTable FetchBlocks(int id)
        {
            try
            {
                // TODO: Add insert logic here
                List<int> bid = new List<int>();
                DataTable blockList = new DataTable();
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        List<string> countList = new List<string>();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GetBlocks";
                        cmd.Parameters.AddWithValue("uidp", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(blockList);
                        
                            foreach(DataRow row in blockList.Rows)
                            {
                                bid.Add(Convert.ToInt32(row["blid"]));
                            }
                        }
                    }
                }
                mCount = GetTotals(bid).ToList();
                return blockList;
            }
            catch
            {
                return null;
            }
        }

        public bool MemberCheck(int id)
        {
            try
            {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select max(mid) from members m inner join users u on m.mid = u.uid where u.uid = ?uidp and blid > 0";
                        cmd.Parameters.AddWithValue("uidp", id);
                        int x = Convert.ToInt32(cmd.ExecuteScalar());
                        if (x > 0)
                            return true;
                        else
                            return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public int[] GetTotals(List<int> x)
        {
            int[] c = new int[x.Count];
            using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
            {
                int n = 0;
                foreach (int i in x)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select count(mid) from members where blid = ?uidp";
                        cmd.Parameters.AddWithValue("uidp", i);
                        int test = Convert.ToInt32(cmd.ExecuteScalar());
                        c[n] = test;
                        n++;
                        conn.Close();
                    }
                }
            }
            return c;
        }

        public int CheckApplication(int id)
        {
            try
            {
                int x = 0;
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select count(uid) from applications where uid = ?uidp and status = \"Pending\"";
                        cmd.Parameters.AddWithValue("uidp", id);
                        x = Convert.ToInt32(cmd.ExecuteScalar());
                        conn.Close();
                    }
                    if (x > 0)
                    {
                        int a = 0;
                        using(var cmd = conn.CreateCommand())
                        {
                            conn.Open();
                            cmd.CommandText = "select count(uid) from approvals where uid = ?uidp and applicationdate = (select max(applicationdate) from applications where uid = ?uidp);";
                            cmd.Parameters.AddWithValue("uidp", id);
                            a = Convert.ToInt32(cmd.ExecuteScalar());
                            conn.Close();
                        }
                        return a;
                    }

                    else
                        return -1;
                }

            }
            catch
            {
                return -2;
            }
        }
    }
}