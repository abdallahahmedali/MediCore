using Microsoft.AspNetCore.Mvc;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Controllers.Admin
{
    public class AdminTestController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminTestController(IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }
        public object Index()
        {
            try
            {
                DataTable ActiveTests = Test.SelectActiveTests(_config);
                DataTable ApprovedTests = Test.SelectApprovedTests(_config);
                ViewBag.ActiveTests = null;
                ViewBag.ApprovedTests = null;
                if (ActiveTests.Rows.Count > 0)
                {
                    ViewBag.ActiveTests = ActiveTests.Rows;
                }
                if (ApprovedTests.Rows.Count > 0)
                {
                    ViewBag.ApprovedTests = ApprovedTests.Rows;
                }

                return View("~/Views/Admin/Test/Index.cshtml");
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Test/Index.cshtml");
            }

        }

        public IActionResult ApproveTest(int param)
        {
            try
            {
                int rowsAff = Test.Approve(_config, param);
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



        public object DeleteTest(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Test.Delete(_config, param);
                DataRow testObj = Test.SelectTests(_config, param).Rows[0];
                if (rowsAff > 0)
                {
                    DataRow TestVisit = Visit.SelectVisits(_config, testObj["Patient_Username"].ToString(), DateTime.Parse(testObj["Visit_Start_Date"].ToString())).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = DateTime.Parse(testObj["Visit_Start_Date"].ToString());
                    v.Patient_Username = testObj["Patient_Username"].ToString();
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) - float.Parse(testObj["Test_Cost"].ToString());
                    v.UpdateVisit(_config, testObj["Patient_Username"].ToString(), DateTime.Parse(testObj["Visit_Start_Date"].ToString()));
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
            DataTable Rooms = Room.SelectRooms(_config, "Test");
            DataTable Types = TypeDetail.SelectTypeDetails(_config, "Test");
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
                return View("~/Views/Admin/Test/Create.cshtml");
            }

            TempData["DangerMessage"] = "Please add first a Room/TypeDetails For Test";
            return RedirectToAction("Create", "Room");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create(Test R, IFormFile TestReport)
        {
            try
            {
                    DataRow TestVisit = Visit.SelectVisits(_config, R.Patient_Username, R.Visit_Start_Date).Rows[0];
                    Visit v = new Visit();
                    v.Visit_Start_Date = R.Visit_Start_Date;
                    v.Patient_Username = R.Patient_Username;
                    v.Visit_Total_Cost = float.Parse(TestVisit["Visit_Total_Cost"].ToString()) + R.Test_Cost;
                    v.UpdateVisit(_config, R.Patient_Username, R.Visit_Start_Date);

                    string stringFileName = null;
                    if (TestReport != null)
                    {
                        stringFileName = UploadFile(TestReport);
                    }
                    R.Test_Report_File = stringFileName;

                    if (R.Room_Number == 0)
                    {
                        TempData["DangerMessage"] = "Please Select Room Number";
                        return Redirect("/Admin/Test/Create/" + R.Patient_Username + "?param2=" + R.Visit_Start_Date);
                    }
                    R.InsertTest(_config);
                

                TempData["SuccessMessage"] = "Successfully Added Your Test Request";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Database Error, Try Again Later";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }

        private String UploadFile(IFormFile profilePictureFile)
        {
            String fileName = null;

            if (profilePictureFile != null)
            {
                String Dir = Path.Combine(_webHostEnvironment.WebRootPath, "files");
                fileName = Guid.NewGuid().ToString() + "-" + profilePictureFile.FileName;
                String filePath = Path.Combine(Dir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profilePictureFile.CopyTo(fileStream);
                }
            }

            return fileName;

        }

        public object Edit(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {

                DataTable Tests = Test.SelectTests(_config, param);
                if (Tests.Rows.Count == 0)
                {
                    TempData["DangerMessage"] = "No Test Reservation Found";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                else
                {
                    ViewBag.TestObj = Tests.Rows[0];
                }
                DataTable rooms = Room.SelectRooms(_config, "Test");
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
                return View("~/Views/Admin/Test/edit.cshtml");
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
        public object Update(Test TestObj, IFormFile TestReport)
        {
            try
            {
                string stringFileName = "";
                if (TestReport != null)
                {
                    stringFileName = UploadFile(TestReport);
                }
                TestObj.Test_Report_File = stringFileName;

                int rowsAff = TestObj.UpdateTest(_config);
                TempData["SuccessMessage"] = "Successfully Updated";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Couldn't Update To the system, please Test the data entered and avoid entering a repeated room type name";

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
            DataTable TypeDetails = TypeDetail.SelectTypeDetails(_config, "Test", Type_Name);

            if (TypeDetails.Rows.Count > 0)
            {
                return TypeDetails.Rows[0]["Type_Cost"];
            }
            return 0;
        }
    }



}

