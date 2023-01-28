using Microsoft.AspNetCore.Mvc;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Controllers.Admin
{
    public class AdminSurgeryController : Controller
    {
        private readonly IConfiguration _config;

        public AdminSurgeryController(IConfiguration config)
        {
            _config = config;
        }
        public object Index()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable ActiveSurgeries = Surgery.SelectActiveSurgeries(_config);
                DataTable ApprovedSurgeries = Surgery.SelectApprovedSurgeries(_config);
                ViewBag.ActiveSurgeries = null;
                ViewBag.ApprovedSurgeries = null;
                if (ActiveSurgeries.Rows.Count > 0)
                {
                    ViewBag.ActiveSurgeries = ActiveSurgeries.Rows;
                }
                if (ApprovedSurgeries.Rows.Count > 0)
                {
                    ViewBag.ApprovedSurgeries = ApprovedSurgeries.Rows;
                }

                return View("~/Views/Admin/Surgery/Index.cshtml");
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Surgery/Index.cshtml");
            }

        }

        public IActionResult ApproveSurgery(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Surgery.Approve(_config, param);
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



        public object DeleteSurgery(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Surgery.Delete(_config, param);
                DataRow surgeryObj = Surgery.SelectSurgeries(_config, param).Rows[0];
                if (rowsAff > 0)
                {
                    DataRow TestVisit = Visit.SelectVisits(_config, surgeryObj["Patient_Username"].ToString(), DateTime.Parse(surgeryObj["Visit_Start_Date"].ToString())).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = DateTime.Parse(surgeryObj["Visit_Start_Date"].ToString());
                    v.Patient_Username = surgeryObj["Patient_Username"].ToString();
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) - float.Parse(surgeryObj["Surgery_Cost"].ToString());
                    v.UpdateVisit(_config, surgeryObj["Patient_Username"].ToString(), DateTime.Parse(surgeryObj["Visit_Start_Date"].ToString()));
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
            DataTable Rooms = Room.SelectRooms(_config, "Surgery");
            DataTable Types = TypeDetail.SelectTypeDetails(_config, "Surgery");
            ViewBag.Visit = null;
            ViewBag.Rooms = null;
            ViewBag.Types = null;
            if (Visits.Rows.Count > 0)
            {
                ViewBag.Visit = Visits.Rows[0];
            }
            if (Rooms.Rows.Count > 0)
            {
                ViewBag.Rooms = Rooms.Rows;
            }
            DataTable doctors = Employee.SelectDoctors(_config);
            if (doctors.Rows.Count > 0)
            {
                ViewBag.Doctors = doctors.Rows;
            }
            else
            {
                TempData["DangerMessage"] = "No Doctor Type Found";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            if (Types.Rows.Count > 0)
            {
                ViewBag.Types = Types.Rows;
                return View("~/Views/Admin/Surgery/Create.cshtml");
            }

            TempData["DangerMessage"] = "Please add first a Room/TypeDetails For Surgery";
            return RedirectToAction("Create", "Room");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create(Surgery R)
        {
            //return R;
            try
            {
                if (R.Room_Number == 0)
                {
                    TempData["DangerMessage"] = "Please Select Room Number";
                    return Redirect("/Admin/Surgery/Create/" + R.Patient_Username + "?param2=" + R.Visit_Start_Date);
                }
                R.InsertSurgery(_config);
                DataRow TestVisit = Visit.SelectVisits(_config, R.Patient_Username, R.Visit_Start_Date).Rows[0];
                Visit v = new Visit();
                v.Visit_Start_Date = R.Visit_Start_Date;
                v.Patient_Username = R.Patient_Username;
                v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) + R.Surgery_Cost;
                v.UpdateVisit(_config, R.Patient_Username, R.Visit_Start_Date);

                TempData["SuccessMessage"] = "Successfully Added Your Surgery Request";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Database Error, Try Again Later";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        public object Edit(int param)
        {

            try
            {

                DataTable Surgeries = Surgery.SelectSurgeries(_config, param);
                if (Surgeries.Rows.Count == 0)
                {
                    TempData["DangerMessage"] = "No Surgery Reservation Found"; 
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                else
                {
                    ViewBag.SurgeryObj = Surgeries.Rows[0];
                }
                DataTable rooms = Room.SelectRooms(_config, "Surgery");
                if (rooms.Rows.Count > 0)
                {
                    ViewBag.rooms = rooms.Rows;
                }
                else
                {
                    TempData["DangerMessage"] = "No Room Type Found";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                DataTable doctors = Employee.SelectDoctors(_config);
                if (doctors.Rows.Count > 0)
                {
                    ViewBag.Doctors = doctors.Rows;
                }
                else
                {
                    TempData["DangerMessage"] = "No Doctor Type Found";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                return View("~/Views/Admin/Surgery/edit.cshtml");
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Database Error";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Surgery SurgeryObj)
        {

                try
                {
                    int rowsAff = SurgeryObj.UpdateSurgery(_config);
                    TempData["SuccessMessage"] = "Successfully Updated";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                catch (Exception e)
                {
                    TempData["DangerMessage"] = "Couldn't Update To the system, please check the data entered and avoid entering a repeated room type name";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }

        }


        public IActionResult FetchRoomNumbers(string param)
        {
            DataTable Rooms = Room.SelectRooms(_config, param);
            ViewBag.Rooms = Rooms.AsEnumerable();
            return PartialView("_RoomNumber");
        }
        public object FetchCost(string Type_Name)
        {
            DataTable TypeDetails = TypeDetail.SelectTypeDetails(_config, "Surgery", Type_Name);

            if (TypeDetails.Rows.Count > 0)
            {
                return TypeDetails.Rows[0]["Type_Cost"];
            }
            return 0;
        }
    }



}

