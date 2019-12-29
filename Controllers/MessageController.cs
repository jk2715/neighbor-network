using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using Neighbor_Network.Models;

namespace Neighbor_Network.Controllers
{
    public class MessageController : Controller
    {
        public static DataTable imsgDT = new DataTable();
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }

        // GET: Message/Details/5
        public DataTable GetThreads(int id)
        {
            DataTable threadList = new DataTable();
            //  try
            // {
            using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
            {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        DataTable tdt = new DataTable();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Fetch_Member_Threads";
                        cmd.Parameters.AddWithValue("uidp", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(tdt);
                        }
                        threadList = tdt;
                        conn.Close();
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                         conn.Open();
                         DataTable tdt = new DataTable();
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = "Fetch_Block_Threads";
                         cmd.Parameters.AddWithValue("uidp", id);
                         using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                         {
                            dr.Fill(tdt);
                         }
                         threadList.Merge(tdt);
                         conn.Close();
                    }
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    DataTable tdt = new DataTable();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Fetch_Author_Threads";
                    cmd.Parameters.AddWithValue("uidp", id);
                    using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                    {
                        dr.Fill(tdt);
                    }
                    threadList.Merge(tdt);
                    conn.Close();
                }
            }
                return threadList;
         //   }
       
          //  catch
          //  {
          //      return null;
           // }
        }

        // GET: Message/Create
        public DataTable GetMessages(int id)
        {

            DataTable messageList = new DataTable();
          //  try
           // {
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select m.msgid, m.mauthor, m.tid, m.mtimestamp, m.mbody, m.replyid, concat(mb.fname, \" \", mb.lname) as Poster from messages m inner join threads t on m.tid = t.tid and m.msgid <> t.imsg inner join users mb on m.mauthor = mb.uid where m.tid = ?tidp order by mtimestamp asc";
                        cmd.Parameters.AddWithValue("tidp", id);
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(messageList);
                        }
                        conn.Close();
                    }

                }
                return messageList;
           // }

           // catch
        //    {
            //    return null;
           // }
        }


        // POST: Message/Create
     //   [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult OpenThread(string id, string  title, string time, string msg, string msgid, string poster, string location)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.Id = id;
                ViewBag.Title = title;
                ViewBag.Time = time;
                ViewBag.Msg = msg;
                ViewBag.MsgId = msgid;
                ViewBag.Poster = poster;
                ViewBag.Location = location;
                return View("Threads");
            }
            catch
            {
                return View();
            }
        }

        // GET: Message/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Message/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(Message message, string author, string tid, string replyid, string time, string title, string msg, string imsgid)
        {
            try
            {
                // TODO: Add update logic here
                try
                {
                    // TODO: Add insert logic here
                    using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            conn.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "PostReply";
                            cmd.Parameters.AddWithValue("uidp", int.Parse(author));
                            cmd.Parameters.AddWithValue("tidp", int.Parse(tid));
                            cmd.Parameters.AddWithValue("imsgp", int.Parse(replyid));
                            cmd.Parameters.AddWithValue("mbodyp", message.Body);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    ViewBag.Id = tid;
                    ViewBag.Title = title;
                    ViewBag.Time = time;
                    ViewBag.Msg = msg;
                    ViewBag.MsgId = imsgid;
                    return View("Threads");
                }
                catch
                {
                    return View("HomePage");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Message/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Message/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostThread(Parent msg, string id, string arr, string checkH)
        {
          //  try
          //  {
                DataTable dtThread = new DataTable();
                string[] str = new string[500];
                if(arr != null) { 
                  str = arr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                // TODO: Add delete logic here
                using (var conn = new MySqlConnection("Server=localhost;Database=neighbor_network;Uid=root;Pwd=Jr-2781995;"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Post_Thread";
                        cmd.Parameters.AddWithValue("uidp", Convert.ToInt32(id));
                        cmd.Parameters.AddWithValue("title", msg.Message.Title);
                        cmd.Parameters.AddWithValue("body", msg.Message.Body);
                        cmd.Parameters.AddWithValue("coordinates", msg.Message.Coordinates);
                        cmd.Parameters.AddWithValue("checkp", checkH);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "select * from messages where msgid = (select max(msgid) from messages)";
                        using (MySqlDataAdapter dr = new MySqlDataAdapter(cmd))
                        {
                            dr.Fill(dtThread);
                        }
                        conn.Close();
                    }
                    if(arr != null) {
                        using (var cmd = conn.CreateCommand())
                        {
                            for (int i = 0; i < str.Length; i++)
                            {
                                conn.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "add_members_threads";
                                cmd.Parameters.AddWithValue("midp", int.Parse(str[i]));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                    
                }
                ViewBag.Id = dtThread.Rows[0]["tid"];
                ViewBag.Title = msg.Message.Title;
                ViewBag.Time = dtThread.Rows[0]["mtimestamp"];
                ViewBag.Msg = msg.Message.Body;
                ViewBag.MsgId = dtThread.Rows[0]["msgid"];
                return View("Threads");
          //  }
            //catch
         //   {
          //      return View();
           // }
        }
    }
}