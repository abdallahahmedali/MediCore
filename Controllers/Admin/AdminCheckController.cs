using Microsoft.AspNetCore.Mvc;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Controllers.Admin
{
    public class AdminCheckController : Controller
    {
        private readonly IConfiguration _config;

        public AdminCheckController(IConfiguration config)
        {
            _config = config;
        }
        public object Index()
        {            
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable ActiveChecks = Check.SelectActiveChecks(_config);
                DataTable ApprovedChecks = Check.SelectApprovedChecks(_config);
                ViewBag.ActiveChecks = null;
                ViewBag.ApprovedChecks = null;
                if (ActiveChecks.Rows.Count > 0)
                {
                    ViewBag.ActiveChecks = ActiveChecks.Rows;
                }
                if (ApprovedChecks.Rows.Count > 0)
                {
                    ViewBag.ApprovedChecks = ApprovedChecks.Rows;
                }

                return View("~/Views/Admin/Check/Index.cshtml");
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Check/Index.cshtml");
            }

        }

        public IActionResult ApproveCheck(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Check.Approve(_config, param);
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



        public object DeleteCheck(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Check.Delete(_config, param);
                DataRow checkObj = Check.SelectChecks(_config, param).Rows[0];
                if (rowsAff > 0)
                {
                    DataRow TestVisit = Visit.SelectVisits(_config, checkObj["Patient_Username"].ToString(), DateTime.Parse(checkObj["Visit_Start_Date"].ToString())).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = DateTime.Parse(checkObj["Visit_Start_Date"].ToString());
                    v.Patient_Username = checkObj["Patient_Username"].ToString();
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) - float.Parse(checkObj["Check_Cost"].ToString());
                    v.UpdateVisit(_config, checkObj["Patient_Username"].ToString(), DateTime.Parse(checkObj["Visit_Start_Date"].ToString()));
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
                return RedirectToAction("Index","Home");
            }
            DataTable Visits = Visit.SelectVisits(_config, param, param2);
            DataTable Rooms = Room.SelectRooms(_config, "Check");
            DataTable Types = TypeDetail.SelectTypeDetails(_config,"Check");
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
                return View("~/Views/Admin/Check/Create.cshtml");
            }

            TempData["DangerMessage"] = "Please add first a Room/TypeDetails For Check Room Reservation";
            return RedirectToAction("Create", "Room");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create(Check R)
        {
            //return R;
            try
            {
                    if (R.Room_Number == 0)
                    {
                        TempData["DangerMessage"] = "Please Select Room Number";
                        return Redirect("/Admin/Check/Create/" + R.Patient_Username + "?param2=" + R.Visit_Start_Date);
                    }
                    R.InsertCheck(_config);
                    DataRow TestVisit = Visit.SelectVisits(_config, R.Patient_Username, R.Visit_Start_Date).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = R.Visit_Start_Date;
                    v.Patient_Username = R.Patient_Username;
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) + R.Check_Cost;
                    v.UpdateVisit(_config, R.Patient_Username, R.Visit_Start_Date);


                TempData["SuccessMessage"] = "Successfully Added Your Check Request";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error, Try Again Later";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        public object Edit(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            try
            {

                DataTable Checks = Check.SelectChecks(_config, param);
                if (Checks.Rows.Count == 0)
                {
                    TempData["DangerMessage"] = "No Check Reservation Found";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                else
                {
                    ViewBag.CheckObj = Checks.Rows[0];
                }
                DataTable rooms = Room.SelectRooms(_config, "Check");
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
                return View("~/Views/Admin/Check/edit.cshtml");
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
        public object Update(Check CheckObj)
        {
            try
            {
                int rowsAff = CheckObj.UpdateCheck(_config);
                TempData["SuccessMessage"] = "Successfully Updated";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Couldn't Update To the system, please check the data entered and avoid entering a repeated room type name";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        public IActionResult View(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable Checks = Check.SelectChecks(_config, param);
                ViewBag.Check = null;
                if(Checks.Rows.Count > 0)
                {
                    ViewBag.Check = Checks.Rows[0];
                    DataTable Consultations = MedicalConsultation.SelectConsultations(_config, Checks.Rows[0]);
                    DataTable Users = Models.Patient.SelectPatient(_config, Checks.Rows[0]["Patient_Username"].ToString());
                    ViewBag.Consultations = null;
                    ViewBag.User = null;
                    if(Consultations.Rows.Count > 0)
                    {
                        ViewBag.Consultations = Consultations.Rows;
                    }
                    if (Users.Rows.Count > 0)
                    {
                        ViewBag.User = Users.Rows[0];
                    }
                    return View("~/Views/Admin/Check/view_consult.cshtml");
                }
                else
                {
                    TempData["DangerMessage"] = "No Check Found With Given ID";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }

            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return Redirect(HttpContext.Request.Headers["Referer"]);
                throw;
            }
        }

        public object FetchCost(string Type_Name)
        {
            DataTable TypeDetails = TypeDetail.SelectTypeDetails(_config,"Check", Type_Name);

            if(TypeDetails.Rows.Count > 0)
            {
                return TypeDetails.Rows[0]["Type_Cost"];
            }
            return 0;
        }
    }



}

