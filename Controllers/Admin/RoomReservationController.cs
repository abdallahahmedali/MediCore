using Microsoft.AspNetCore.Mvc;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Controllers.Admin
{
    public class RoomReservationController : Controller
    {
        private readonly IConfiguration _config;

        public RoomReservationController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable ActiveRoomRes = RoomReservation.SelectActiveRoomReservations(_config);
                DataTable ApprovedRoomRes = RoomReservation.SelectApprovedRoomReservations(_config);
                ViewBag.ActiveRoomRes = null;
                ViewBag.ApprovedRoomRes = null;
                if (ActiveRoomRes.Rows.Count > 0)
                {
                    ViewBag.ActiveRoomRes = ActiveRoomRes.Rows;
                }
                if (ApprovedRoomRes.Rows.Count > 0)
                {
                    ViewBag.ApprovedRoomRes = ApprovedRoomRes.Rows;
                }

                return View("~/Views/Admin/RoomReservation/Index.cshtml");
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/RoomReservation/Index.cshtml");
            }

        }

        public IActionResult ApproveReservation(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = RoomReservation.Approve(_config, param);
                if (rowsAff > 0)
                {
                    TempData["SuccessMessage"] = "Successfully Approved";
                }
                else
                {
                    TempData["DangerMessage"] = "Couldn't approve it, Try again later";
                }
            }
            catch (Exception)
            { 
                TempData["DangerMessage"] = "Database Error";
            }

            return Redirect(HttpContext.Request.Headers["Referer"]);
        }



        public object DeleteReservation(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = RoomReservation.Delete(_config, param);
                DataRow ResObj = RoomReservation.SelectRoomReservations(_config, param).Rows[0];
                if (rowsAff > 0)
                {

                    DataRow TestVisit = Visit.SelectVisits(_config, ResObj["Patient_Username"].ToString(), DateTime.Parse(ResObj["Visit_Start_Date"].ToString())).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = DateTime.Parse(ResObj["Visit_Start_Date"].ToString());
                    v.Patient_Username = ResObj["Patient_Username"].ToString();
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) - float.Parse(ResObj["Reservation_Cost"].ToString());
                    v.UpdateVisit(_config, ResObj["Patient_Username"].ToString(), DateTime.Parse(ResObj["Visit_Start_Date"].ToString()));
                    TempData["SuccessMessage"] = "Successfully Deleted (Rejected)";
                }
                else
                {
                    TempData["DangerMessage"] = "Couldn't reject it, Try again later";
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
            }

            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        public IActionResult Create(string param, DateTime param2)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable Visits = Visit.SelectVisits(_config, param, param2);
            DataTable Rooms = Room.SelectRooms(_config, "General");
            ViewBag.Visit = null;
            ViewBag.Rooms = null;
            if (Visits.Rows.Count > 0)
            {
                ViewBag.Visit = Visits.Rows[0];
            }
            if (Rooms.Rows.Count > 0)
            {
                ViewBag.Rooms = Rooms.Rows;
                return View("~/Views/Admin/RoomReservation/Create.cshtml");
            }
            
            TempData["DangerMessage"] = "Please add first a Room For General Room Reservation";
            return RedirectToAction("Create", "Room");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create(RoomReservation R)
        {
            
            //return R;

                if (R.Room_Number == 0)
                {
                    TempData["DangerMessage"] = "Please Select Room Number";
                    return Redirect("/Admin/RoomReservation/Create/"+R.Patient_Username+"?param2="+R.Visit_Start_Date);
                }
                if(R.Reservation_End_Date <= R.Reservation_Start_Date)
                {
                    TempData["DangerMessage"] = "Please Select An Appropriate Date Range";
                    return Redirect("/Admin/RoomReservation/Create/" + R.Patient_Username + "?param2=" + R.Visit_Start_Date);
                }
                R.InsertReservation(_config);

                DataRow TestVisit = Visit.SelectVisits(_config, R.Patient_Username, R.Visit_Start_Date).Rows[0];
                Visit v = new Visit();
                v.Visit_Start_Date = R.Visit_Start_Date;
                v.Patient_Username = R.Patient_Username;
                v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) + R.Reservation_Cost;
                v.UpdateVisit(_config, R.Patient_Username, R.Visit_Start_Date);

            TempData["SuccessMessage"] = "Successfully Added Your Room Reservation";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                using (DataTable RoomRes = RoomReservation.SelectRoomReservations(_config, param))
                {

                    if (RoomRes.Rows.Count > 0)
                    {
                        ViewBag.RoomRes = RoomRes.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Room Reservation Found";
                        return Redirect(HttpContext.Request.Headers["Referer"]);
                    }


                    return View("~/Views/Admin/RoomReservation/Edit.cshtml");
                }
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update([Bind("Patient_Username, Visit_Start_Date, Room_Type_Name, Room_Number, Type_Name, Reservation_Cost, Reservation_Start_Date, Reservation_End_Date, Reservation_Is_Confirmed")] RoomReservation Res, int Old_Reservation_Cost)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DataRow TestVisit = Visit.SelectVisits(_config, Res.Patient_Username, Res.Visit_Start_Date).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = Res.Visit_Start_Date;
                    v.Patient_Username = Res.Patient_Username;
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) - Old_Reservation_Cost + Res.Reservation_Cost;
                    v.UpdateVisit(_config, Res.Patient_Username, Res.Visit_Start_Date);
                    int rowsAff = Res.UpdateRoomReservation(_config);
                    TempData["SuccessMessage"] = "Successfully Updated";

                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                catch (Exception e)
                {
                    TempData["DangerMessage"] = "Couldn't Update To the system, please check the data entered and avoid entering a repeated room type name";

                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }

            }
            else
            {
                TempData["DangerMessage"] = "Invalid Data Entered or Maybe Some Data are Messing";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }


        public object FetchCost(string Room_Type_Name, int Room_Number)
        {
            DataTable Rooms = Room.SelectRooms(_config, Room_Type_Name, Room_Number);

            if(Rooms.Rows.Count > 0)
            {
                return Rooms.Rows[0]["Room_Cost"];
            }
            return 0;
        }
    }
}
