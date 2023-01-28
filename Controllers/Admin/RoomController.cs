using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class RoomController : Controller
    {
        private readonly IConfiguration _config;

        public RoomController(IConfiguration config)
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
                using (DataTable Rooms = Room.SelectRooms(_config))
                {
                    ViewBag.RoomTypea = null;
                    if (Rooms.Rows.Count > 0)
                    {
                        ViewBag.Rooms = Rooms.Rows;
                    }

                    return View("~/Views/Admin/Room/Index.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Room/Index.cshtml");
            }

        }

        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable RoomTypes = RoomType.SelectRoomTypes(_config);
            ViewBag.RoomTypes = RoomTypes.Rows;
            if(RoomTypes.Rows.Count > 0)
            {
                return View("~/Views/Admin/Room/Create.cshtml");
            }
            TempData["DangerMessage"] = "Please add first a Room Type";
            return RedirectToAction("Create","RoomType");
        }

        public IActionResult Edit(string param, int sec_param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable RoomTypes = RoomType.SelectRoomTypes(_config);
            if(RoomTypes.Rows.Count > 0)
            {
                ViewBag.RoomTypes = RoomTypes.Rows;
            }
            else
            {
                TempData["DangerMessage"] = "No Room Type Found, Please insert first a Room Type";
                return RedirectToAction("Index");
            }

            try
            {
                using (DataTable Rooms = Room.SelectRooms(_config, param, sec_param))
                {

                    if (Rooms.Rows.Count > 0)
                    {
                        ViewBag.Room = Rooms.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Room Found";
                        return RedirectToAction("Index");
                    }

                    
                    return View("~/Views/Admin/Room/Edit.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create([Bind("Room_Type_Name, Room_Number, Room_Capacity, Room_Excess_Capacity, Room_Companion, Room_Cost")] Room RoomObj)
        {

                try
                {
                    int rowsAff = RoomObj.InsertRoom(_config);
                    TempData["SuccessMessage"] = "Successfully Added " + RoomObj.Room_Type_Name + " To the system";

                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Add " + RoomObj.Room_Type_Name + "(" + RoomObj.Room_Number + ") To the system, please check the data entered and avoid entering a repeated room type name";
                }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Edit(string Old_Room_Type_Name, int Old_Room_Number, [Bind("Room_Type_Name, Room_Number, Room_Capacity, Room_Excess_Capacity, Room_Companion, Room_Cost")] Room RoomObj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int rowsAff = RoomObj.UpdateRoom(_config, Old_Room_Type_Name, Old_Room_Number);
                    TempData["SuccessMessage"] = "Successfully Updated " + RoomObj.Room_Type_Name + "(" + RoomObj.Room_Number + ") To the system";

                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Update " + Old_Room_Type_Name + "(" + Old_Room_Number + ") To the system, please check the data entered and avoid entering a repeated room type name";

                }

            }
            else
            {
                TempData["DangerMessage"] = "Invalid Data Entered or Maybe Some Data are Messing";
            }
            return RedirectToAction("Index");
        }
    }
}
