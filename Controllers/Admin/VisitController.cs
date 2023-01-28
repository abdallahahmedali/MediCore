using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;


namespace HospitalAppl.Controllers.Admin
{
    public class VisitController : Controller
    {
        private readonly IConfiguration _config;

        public VisitController(IConfiguration config)
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
                DataTable Visits = Visit.SelectVisits(_config);
                DataTable ActiveVisits = Visit.SelectActiveVisits(_config);
                DataTable ApprovedVisits = Visit.SelectApprovedVisits(_config);
                ViewBag.Visits = null;
                ViewBag.ActiveVisits = null;
                ViewBag.ApprovedVisits = null;
                ViewBag.PatientsCount = Patient.PatientsCount(_config);
                if (Visits.Rows.Count > 0)
                {
                    ViewBag.Visits = Visits.Rows;
                }
                if (ActiveVisits.Rows.Count > 0)
                {
                    ViewBag.ActiveVisits = ActiveVisits.Rows;
                }
                if (ApprovedVisits.Rows.Count > 0)
                {
                    ViewBag.ApprovedVisits = ApprovedVisits.Rows;
                }

                return View("~/Views/Admin/Visit/Index.cshtml");
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Visit/Index.cshtml");
            }
        }
        public IActionResult ViewVisitPatient(string param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable Visits = Visit.SelectUserVisits(_config, param);
                DataTable ActiveVisits = Visit.SelectUserActiveVisits(_config, param);
                DataTable Patients = Patient.SelectPatient(_config, param);
                ViewBag.Visits = null;
                ViewBag.ActiveVisits = null;
                ViewBag.PatientObj = null;
                if (Visits.Rows.Count > 0)
                {
                    ViewBag.Visits = Visits.Rows;
                }
                if (ActiveVisits.Rows.Count > 0)
                {
                    ViewBag.ActiveVisits = ActiveVisits.Rows[0];
                }
                if (Patients.Rows.Count > 0)
                {
                    ViewBag.PatientObj = Patients.Rows[0];
                }

                return View("~/Views/Admin/Visit/view_patient.cshtml");
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Database Error";
                return RedirectToAction("Index");
            }
        }


        public IActionResult Create()
        {
            DataTable Patients = Patient.SelectPatients(_config);
            ViewBag.Patients = Patients.Rows;
            return View("~/Views/Admin/Visit/create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Patient_Username)
        {
            try
            {
                DataTable Visits = Visit.SelectUserActiveVisits(_config, Patient_Username);
                if (Visits.Rows.Count > 0)
                {
                    TempData["DangerMessage"] = "There is an opened Visit for this Patient!";
                }
                else
                {
                    Visit.InsertVisit(_config, Patient_Username);
                    TempData["SuccessMessage"] = "Visit is Created Successfully!";
                }
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Failed To Add Visit To " + Patient_Username;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Close(string param, DateTime param2)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable Visits = Visit.SelectVisits(_config, param, param2);
                if(Visits.Rows.Count > 0)
                {
                    Visit.LockVisit(_config, param, param2);
                    TempData["SuccessMessage"] = "Successfully Closed the Visit!";
                }
                else
                {
                    TempData["DangerMessage"] = "No Visit Found";
                }
            }
            catch (Exception e)
            {

                TempData["DangerMessage"] = "Database Error";
            }
            return RedirectToAction("Index");
        }

        public IActionResult ViewVisit(string param, DateTime param2)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                DataTable Visits = Visit.SelectVisits(_config, param, param2);
                ViewBag.Visit = null;
                if (Visits.Rows.Count > 0)
                {
                    ViewBag.Visit = Visits.Rows[0];
                }
                DataTable RoomRes = RoomReservation.SelectVisitRoomReservations(_config, param, param2);
                ViewBag.RoomRes = null;
                if (RoomRes.Rows.Count > 0)
                {
                    ViewBag.RoomRes = RoomRes.Rows;
                }
                DataTable Surgeries = Surgery.SelectVisitActiveSurgeries(_config, param, param2);
                ViewBag.Surgeries = null;
                if (Surgeries.Rows.Count > 0)
                {
                    ViewBag.Surgeries = Surgeries.Rows;
                }
                DataTable Tests = Test.SelectVisitActiveTests(_config, param, param2);
                ViewBag.Tests = null;
                if (Tests.Rows.Count > 0)
                {
                    ViewBag.Tests = Tests.Rows;
                }
                DataTable Checks = Check.SelectVisitActiveChecks(_config, param, param2);
                ViewBag.Checks = null;
                if (Checks.Rows.Count > 0)
                {
                    ViewBag.Checks = Checks.Rows;
                }
            }
            catch (Exception)
            {

                TempData["DangerMessage"] = "No Visit With Given Parameters";
                return RedirectToAction("Index");
            }
            


            return View("~/Views/Admin/Visit/view_visit.cshtml");
        }
    }
}
