using Microsoft.AspNetCore.Mvc;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Controllers.Admin
{
    public class AdminConsultationController : Controller
    {
        private readonly IConfiguration _config;

        public AdminConsultationController(IConfiguration config)
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
                DataTable Consultations = MedicalConsultation.SelectConsultations(_config);
                ViewBag.Consultations = null;
                if (Consultations.Rows.Count > 0)
                {
                    ViewBag.Consultations = Consultations.Rows;
                }


                return View("~/Views/Admin/Consultation/Index.cshtml");
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Consultation/Index.cshtml");
            }

        }

        public object Create(int param)
        {
            DataTable Checks = Check.SelectChecks(_config, param);
            if(Checks.Rows.Count > 0)
            {
                if(Boolean.Parse(Checks.Rows[0]["Check_Has_Medical_Consultation"].ToString()) == false)
                {
                    if(Checks.Rows[0]["Check_Cost"] is not System.DBNull){
                        MedicalConsultation m = new MedicalConsultation();
                        m.Consultation_Date = DateTime.Now.AddDays(7);
                        m.Check_Date = DateTime.Parse(Checks.Rows[0]["Check_Date"].ToString());
                        m.Room_Number = Int32.Parse(Checks.Rows[0]["Room_Number"].ToString());
                        m.Room_Type_Name = Checks.Rows[0]["Room_Type_Name"].ToString();
                        m.Type_Name = Checks.Rows[0]["Type_Name"].ToString();
                        m.Visit_Start_Date = DateTime.Parse(Checks.Rows[0]["Visit_Start_Date"].ToString());
                        m.Patient_Username = Checks.Rows[0]["Patient_Username"].ToString();
                        m.Doctor_Username = Checks.Rows[0]["Doctor_Username"].ToString();
                        m.Consultation_Cost = Int32.Parse(Checks.Rows[0]["Check_Cost"].ToString())*.5;
                        m.InsertConsultation(_config);
                        Check.UpdateConsultFlag(_config, param);
                        TempData["SuccessMessage"] = "Medical Consultation Is Added Successfully";
                    }else{
                        TempData["DangerMessage"] = "Please Assign A Room For This Check First !";
                    }
                }
                else
                {
                    TempData["DangerMessage"] = "There is already a Medical Consultation For this Check";
                }
                
            }
            else
            {
                TempData["DangerMessage"] = "Medical Consultation Failed To be Added";
            }
            return Redirect("/Admin/AdminCheck/View/"+param);
        }
        public IActionResult Edit(int param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {

                DataTable Consultations = MedicalConsultation.SelectConsultations(_config, param);
                if (Consultations.Rows.Count == 0)
                {
                    TempData["DangerMessage"] = "No Consultation Reservation Found";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ConsultationObj = Consultations.Rows[0];
                }
                DataTable rooms = Room.SelectRooms(_config, "Consultation");
                if (rooms.Rows.Count > 0)
                {
                    ViewBag.rooms = rooms.Rows;
                }
                else
                {
                    TempData["DangerMessage"] = "No Room Type Found";
                    return RedirectToAction("Index");
                }
                return View("~/Views/Admin/Consultation/edit.cshtml");
            }
            catch (Exception e)
            {
                throw e;
                TempData["DangerMessage"] = "Database Error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Update(MedicalConsultation ConsultationObj)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }

            try
            {
                int rowsAff = ConsultationObj.UpdateConsultation(_config);
                TempData["SuccessMessage"] = "Successfully Updated";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Couldn't Update To the system, please Consultation the data entered and avoid entering a repeated room type name";

                return RedirectToAction("Index");

            }
        }


        public IActionResult FetchRoomNumbers(string param)
        {
            DataTable Rooms = Room.SelectRooms(_config, param);
            ViewBag.Rooms = Rooms.AsEnumerable();
            return PartialView("_RoomNumber");
        }
    }



}

