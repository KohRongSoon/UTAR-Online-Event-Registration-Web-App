using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UTAR_Online_Event_Registration_Web_App.Models;

namespace UTAR_Online_Event_Registration_Web_App.Controllers
{
    static class current
    {
        public static int session_id;
        public static string session_name;
        public static int session_event;
    }
   
    public class HomeController : Controller
    {

        private readonly IConfiguration _server;

        public HomeController(IConfiguration server)
        {
            this._server = server;
        }

        public IActionResult Login()
        {
            current.session_id = 0;
            current.session_name = "";
            ViewBag.Message = TempData["message"] as string;

            UserDataModel userDataModel = new UserDataModel();
            return View(userDataModel);
        }

        [HttpPost]
        public IActionResult Login(int id, [Bind("userID,name,email,password,phonum")] UserDataModel userDataModel)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand(@"select * from u_login(:u_email,:u_password)", sqlConnection);

                sqlcmd.Parameters.AddWithValue("u_email", userDataModel.email);
                sqlcmd.Parameters.AddWithValue("u_password", userDataModel.password);

                int result = (int)sqlcmd.ExecuteScalar();
                

                if (result == 1)
                {
                    NpgsqlCommand sql = new NpgsqlCommand(@"select _id, _name from usertable where _email = @email and _password = @password", sqlConnection);
                    sql.Parameters.AddWithValue("email", userDataModel.email);
                    sql.Parameters.AddWithValue("password", userDataModel.password);
                    var db = sql.ExecuteReader();
                    while (db.Read())
                    {
                        current.session_id = Convert.ToInt32(db[0]);
                        current.session_name = Convert.ToString(db[1]);
                    }
                   

                    TempData["message"] = "Login Sucessfully! Welcome " + current.session_name;
                    return RedirectToAction(nameof(EventViewU));
                }
                else if (result == 2)
                {
                    NpgsqlCommand sql = new NpgsqlCommand(@"select _id, _name from usertable where _email = @email and _password = @password", sqlConnection);
                    sql.Parameters.AddWithValue("email", userDataModel.email);
                    sql.Parameters.AddWithValue("password", userDataModel.password);
                    var db = sql.ExecuteReader();
                    while (db.Read())
                    {
                        current.session_id = Convert.ToInt32(db[0]);
                        current.session_name = Convert.ToString(db[1]);
                    }

                    TempData["message"] = "Login Sucessfully! Welcome " + current.session_name;
                    return RedirectToAction(nameof(EventViewStaff));
                }
                else
                {
                    TempData["message"] = "Login Unsucessfully!";
                    return RedirectToAction(nameof(Login));
                }
                
            }
        }

        public IActionResult Register()
        {
            UserDataModel userDataModel = new UserDataModel();
            return View(userDataModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(int id, [Bind("userID,name,email,password,phonum")] UserDataModel userDataModel)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand("user_insert", sqlConnection);
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_name", DbType.String)).Value = userDataModel.name;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_email", DbType.String)).Value = userDataModel.email;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_password", DbType.String)).Value = userDataModel.password;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_phone_num", DbType.String)).Value = userDataModel.phonum;
                sqlcmd.CommandType = CommandType.StoredProcedure;
                var reader = sqlcmd.ExecuteReader();
                Console.WriteLine(reader);
            }
            TempData["message"] = "Register Sucessfully!";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult EventViewU()
        {
            if(current.session_id != 0) { 
            ViewData["id"] = current.session_id;
            ViewData["name"] = current.session_name;

            ViewBag.Message = TempData["message"] as string;

            EventDataModel list = new EventDataModel();

            String sql = "SELECT * FROM eventtable WHERE status = 'O'";
            Database db = new Database(sql, this._server);
            if (db.data.HasRows)
            {
                while (db.data.Read())
                {
                    list.ListModel.Add(new EventDataModel
                    {
                        eventID = Convert.ToInt32(db.data[0]),
                        userId = Convert.ToInt32(db.data[1]),
                        eventName = db.data[2].ToString(),
                        eventDescription = db.data[3].ToString(),
                        eventDate = Convert.ToDateTime(db.data[4]),
                        eventPrice = Convert.ToInt32(db.data[5]),
                        eventTicQty = Convert.ToInt32(db.data[6])
                    });
                }
            }

            db.Close();

            List<EventDataModel> model = list.ListModel.ToList();
            return View(model);
            }
            else
                return View();
        }
        public IActionResult EventDetails(int id)
        {
            current.session_event = id;
            
            EventDataModel eventDataModel = new EventDataModel();
            if (id > 0)
                eventDataModel = FetchEventByID(id);
            return View(eventDataModel);
        }

        [HttpPost]
        public IActionResult EventDetails()
        {
            Console.WriteLine(current.session_event);

            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand("create_invoice", sqlConnection);
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_id", DbType.Int32)).Value = current.session_id;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_id", DbType.Int32)).Value = current.session_event;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("i_date", DbType.DateTime)).Value = DateTime.Now;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("p_type", DbType.String)).Value = "Cash";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                var reader = sqlcmd.ExecuteReader();
                Console.WriteLine(reader);
            }
            TempData["message"] = "Event Register Sucessfully!";
            return RedirectToAction(nameof(EventViewU));
        }

        public IActionResult RegisteredEvent()
        {
            TicketDataModel list = new TicketDataModel();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand(@"select * from ticketsalestable where user_id = @id", sqlConnection);
                sqlcmd.Parameters.AddWithValue("id", current.session_id);
                var db = sqlcmd.ExecuteReader();
                if (db.HasRows)
                {
                    while (db.Read())
                    {
                        list.ListModel.Add(new TicketDataModel
                        {
                            invoice_num = Convert.ToInt32(db[0]),
                            userId = Convert.ToInt32(db[1]),
                            eventId = Convert.ToInt32(db[2]),
                            paymentDate = Convert.ToDateTime(db[3]),
                            paymentType = db[4].ToString()


                        });
                    }
                }

                sqlConnection.Close();
            }
            List<TicketDataModel> model = list.ListModel.ToList();
            return View(model);

        }
        public IActionResult EventViewStaff()
        {
            ViewData["id"] = current.session_id;
            ViewData["name"] = current.session_name;

            ViewBag.Message = TempData["message"] as string;


            EventDataModel list = new EventDataModel();

            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand(@"select * from eventtable where user_id = @id and status = 'O'", sqlConnection);
                sqlcmd.Parameters.AddWithValue("id", current.session_id);
                var db = sqlcmd.ExecuteReader();
                if (db.HasRows)
                {
                    while (db.Read())
                    {
                        list.ListModel.Add(new EventDataModel
                        {
                            eventID = Convert.ToInt32(db[0]),
                            userId = Convert.ToInt32(db[1]),
                            eventName = db[2].ToString(),
                            eventDescription = db[3].ToString(),
                            eventDate = Convert.ToDateTime(db[4]),
                            eventPrice = Convert.ToInt32(db[5]),
                            eventTicQty = Convert.ToInt32(db[6]),
                            eventStatus = Convert.ToChar(db[7])

                        });
                    }
                }

                sqlConnection.Close();

            }
            List<EventDataModel> model = list.ListModel.ToList();
            return View(model);

         
        }
        public IActionResult EventCreate()
        {
            EventDataModel eventDataModel = new EventDataModel();
            return View(eventDataModel);
        }

        [HttpPost]
        public IActionResult EventCreate(int id, [Bind("eventID,userId,eventName,eventDescription,eventDate,eventPrice,eventTicQty,eventStatus")] EventDataModel eventDataModel)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand("event_insert", sqlConnection);
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_id", DbType.Int32)).Value = current.session_id;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_name", DbType.String)).Value = eventDataModel.eventName;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_description", DbType.String)).Value = eventDataModel.eventDescription;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_date", DbType.DateTime)).Value = eventDataModel.eventDate;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_price", DbType.Int32)).Value = eventDataModel.eventPrice;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_qty", DbType.Int32)).Value = eventDataModel.eventTicQty;
                sqlcmd.CommandType = CommandType.StoredProcedure;
                var reader = sqlcmd.ExecuteReader();
                Console.WriteLine(reader);

            }
            TempData["message"] = "Event Create Sucessfully!";
            return RedirectToAction(nameof(EventViewStaff));
        }

        public IActionResult EventEdit(int? id)
        {
            EventDataModel eventDataModel = new EventDataModel();
            if (id > 0)
                eventDataModel = FetchEventByID(id);
            return View(eventDataModel);
        }

        [HttpPost]
        public IActionResult EventEdit(int id, [Bind("eventID,userId,eventName,eventDescription,eventDate,eventPrice,eventTicQty,eventStatus")] EventDataModel eventDataModel)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand("event_update", sqlConnection);
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_id", DbType.Int32)).Value = id;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_name", DbType.String)).Value = eventDataModel.eventName;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_description", DbType.String)).Value = eventDataModel.eventDescription;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("e_date", DbType.DateTime)).Value = eventDataModel.eventDate;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_price", DbType.Int32)).Value = eventDataModel.eventPrice;
                sqlcmd.Parameters.AddWithValue(new NpgsqlParameter("u_qty", DbType.Int32)).Value = eventDataModel.eventTicQty;
                sqlcmd.CommandType = CommandType.StoredProcedure;
                var reader = sqlcmd.ExecuteReader();
                Console.WriteLine(reader);

            }
            TempData["message"] = "Event Update Sucessfully!";
            return RedirectToAction(nameof(EventViewStaff));
        }

        public IActionResult EventDelete(int id)
        {
            EventDataModel eventDataModel = new EventDataModel();
            if (id > 0)
                eventDataModel = FetchEventByID(id);
            current.session_event = id;
            return View(eventDataModel);
        }
        [HttpPost]
        public IActionResult EventDelete()
        {
            Console.WriteLine(current.session_event);
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand(@"update eventtable set status = @status where _id = @id", sqlConnection);
                sqlcmd.Parameters.AddWithValue("status", "X");
                sqlcmd.Parameters.AddWithValue("id", current.session_event);
                sqlcmd.ExecuteNonQuery();

                sqlConnection.Close();

            }
            TempData["message"] = "Event Delete Sucessfully!";
            return RedirectToAction(nameof(EventViewStaff));

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [NonAction]
        public EventDataModel FetchEventByID(int? id)
        {
            EventDataModel eventDataModel = new EventDataModel();

            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_server.GetConnectionString("MyServer")))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlcmd = new NpgsqlCommand(@"select * from eventtable where _id = @id", sqlConnection);
                sqlcmd.Parameters.AddWithValue("id", id);
                var db = sqlcmd.ExecuteReader();

                while (db.Read())
                {
                    eventDataModel.eventID = Convert.ToInt32(db[0]);
                    eventDataModel.userId = Convert.ToInt32(db[1]);
                    eventDataModel.eventName = db[2].ToString();
                    eventDataModel.eventDescription = db[3].ToString();
                    eventDataModel.eventDate = Convert.ToDateTime(db[4]);
                    eventDataModel.eventPrice = Convert.ToInt32(db[5]);
                    eventDataModel.eventTicQty = Convert.ToInt32(db[6]);
                    eventDataModel.eventStatus = Convert.ToChar(db[7]);
                    
                }

                sqlConnection.Close();

            }
            return eventDataModel;
        }

    }
}
