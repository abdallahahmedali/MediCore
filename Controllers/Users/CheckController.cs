using HospitalAppl.Libraies;
using HospitalAppl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HospitalAppl.Controllers.Users
{
    public class CheckController : Controller
    {
        private readonly ILogger<CheckController> _logger;

        private readonly IConfiguration _config;

        public CheckController(ILogger<CheckController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(string? select,string? PatientUsername){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if(!string.IsNullOrEmpty(PatientUsername)){
                DataTable Checks;
                if(select == "Completed"){
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                        AND Check_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Check_Is_Rejected = 0
                        AND Check_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Upcoming"){
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                        AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Check_Is_Rejected = 0
                        AND Check_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                        AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Check_Is_Rejected = 1;
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Awaiting"){
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                        AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Check_Is_Rejected = 0
                        AND Check_Is_Confirmed = 0;
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                        AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Check_Is_Rejected = 1;
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else{
                    Checks = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectChecksQuery = @"
                        SELECT * FROM Checks AS C
                        LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON C.Doctor_Username = A.Username
                    ";
                    SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                    SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Checks.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }

                return View(Checks);                
            }else{
                DataTable Checks;
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    if(select == "Completed"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                            AND Check_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON C.Patient_Username = A.Username
                            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
                }else{
                   if(select == "Completed"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                            AND Check_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 0
                            AND Check_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                            AND Check_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Check_Is_Rejected = 1;
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Checks = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectChecksQuery = @"
                            SELECT * FROM Checks AS C
                            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON C.Doctor_Username = A.Username
                        ";
                        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Checks.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
                }

                return View(Checks);       
            }

        }
        // Create Action
        [HttpGet]
        public IActionResult CreateCheck()
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            DataTable DepartmentsList = Department.SelectDepartments(_config);
            DataTable CheckTypesList = TypeDetail.SelectTypeDetails(_config,"Check");
            
            var CheckObj = new Check(DepartmentsList);
            ViewBag.CheckTypes = CheckTypesList;
            DataTable dt = Get_Type_Name(CheckObj);
            return View(CheckObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCheck(Check CheckObj)
        {
            DateTime NewVisitStartDate = Visit.InitVisit(_config,HttpContext.Session.GetString("Username"),Convert.ToDateTime(CheckObj.Check_Date));
            //get doctor user name
            DataTable doctor = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            if(!string.IsNullOrWhiteSpace(CheckObj.SelectedDepartmentName.ToString()) && !string.IsNullOrWhiteSpace(CheckObj.EmployeeUsername.ToString()) && CheckObj.Check_Date >= DateTime.Now){
            //Insert into DataBase
                con.Open(); //ID auto inc
                string query = "INSERT into Checks (Visit_Start_Date,Patient_Username,Type_Name,Doctor_Username,Check_Date)Values(@VisitStartDate,@Patient_Username,@type,@Doctor_Username,@date);";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@date", CheckObj.Check_Date);
                cmd.Parameters.AddWithValue("@VisitStartDate", NewVisitStartDate);
                cmd.Parameters.AddWithValue("@Patient_Username", HttpContext.Session.GetString("Username"));
                cmd.Parameters.AddWithValue("@type", CheckObj.Type_Name);
                cmd.Parameters.AddWithValue("@Doctor_Username", CheckObj.EmployeeUsername);
                try{
                    cmd.ExecuteNonQuery();
                    con.Close();
                }catch(Exception){
                    con.Close();
                    TempData["DangerMessage"] = "Failed To Book Your Check !";
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }

                TempData["SuccessMessage"] = "Check Booked Successfully";
                return RedirectToAction("Index");
            }else{
                TempData["DangerMessage"] = "Failed To Book Your Check !";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

        }

        
        [HttpGet]
        public IActionResult CheckDetail(string? CID){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if (!string.IsNullOrWhiteSpace(CID))
            {
                DataTable Check = new DataTable();
                DataTable Consultations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectCheckDetailsQuery = @"
                    SELECT *
                    FROM Checks C
                    LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                    WHERE Check_ID = @CID
                ";

                SqlCommand SelectCheckDetailsCmd = new SqlCommand(SelectCheckDetailsQuery,con);
                SelectCheckDetailsCmd.Parameters.AddWithValue("@CID",CID);

                con.Open();
                SqlDataReader sdr = SelectCheckDetailsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Check.Load(sdr);
                    sdr.Close();
                }
                con.Close();
                con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectConsultationsQuery = @"
                    SELECT *
                    FROM MedicalConsultations MC
                    LEFT JOIN RoomTypes RT ON MC.Room_Type_Name = RT.Room_Type_Name
                    WHERE Check_Date = @CDate AND Type_Name = @TName AND Patient_Username = @PName AND Visit_Start_Date = @VisitStartDate
                ";

                SqlCommand SelectConsultationsCmd = new SqlCommand(SelectConsultationsQuery,con);
                SelectConsultationsCmd.Parameters.AddWithValue("@CDate",Check.Rows[0]["Check_Date"]);
                SelectConsultationsCmd.Parameters.AddWithValue("@TName",Check.Rows[0]["Type_Name"]);
                SelectConsultationsCmd.Parameters.AddWithValue("@PName",Check.Rows[0]["Patient_Username"]);
                SelectConsultationsCmd.Parameters.AddWithValue("@VisitStartDate",Check.Rows[0]["Visit_Start_Date"]);

                con.Open();
                sdr = SelectConsultationsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Consultations.Load(sdr);
                    sdr.Close();
                }
                con.Close();
                Check CheckObj = new Check(Check,Consultations);
                return View(CheckObj);
            }

            return View();
        }

        // Edit Action 
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Check CheckObj)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "UPDATE Checks Set ";    // will be changed
            SqlCommand cmd = new SqlCommand(query, con);



            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";
            return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public IActionResult Detail_Patient()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Doctor_Username, Type_Name, Check_Date, Check_Has_Medical_Consultation ,Check_Details,Check_Cost From Checks; ";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Patient"] = Details;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Detail_Doctor()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Patient_Username, Type_Name, Check_Date, Check_Has_Medical_Consultation ,Check_Details From Checks; ";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Doctor"] = Details;
            return RedirectToAction("Index", "Home");
        }

        //Delete Action
        [HttpDelete]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Delete From Checks where ";    // will be changed
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";
            return RedirectToAction("Index", "Home");
        }

        // Get List of Type Name
        public DataTable Get_Type_Name(Check CheckObj)
        {
            DataTable Specialities = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var query = "SELECT Type_Name From TypeDetails;";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                Specialities.Load(sdr);
                sdr.Close();
            }
            con.Close();

            //CheckObj.type_name_list = Specialities;
            return Specialities;
        }
        [HttpGet]   
        public IActionResult GetEmployees(string? departmentName)
        {
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                IEnumerable<SelectListItem> Employees = Employee.GetEmployeesFromDepartment(_config.GetConnectionString("DefaultConnection"),departmentName);
                return Json(Employees);
            }
            return null;
        }

        public IActionResult GetCheckDetails(string? CID)
        {
            if (!string.IsNullOrWhiteSpace(CID))
            {
                DataTable Checks = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectChecksQuery = @"
                    SELECT Check_Details   
                    FROM Checks 
                    WHERE Check_ID = @CID
                ";
                SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
                SelectChecksCmd.Parameters.AddWithValue("@CID",CID);

                con.Open();
                SqlDataReader sdr = SelectChecksCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Checks.Load(sdr);
                    sdr.Close();
                }
                con.Close();        
                return Json(Checks.Rows[0]["Check_Details"]);
            }
            return null;
        }

        [HttpPost]
        public IActionResult SaveCheckDetails(string? CheckId,string? CheckDetails){

            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var InsertCheckDetailsQuery = "UPDATE Checks SET Check_Details = @Check_Details WHERE Check_ID=@CID;";
            SqlCommand InsertCheckDetailsCmd = new SqlCommand(InsertCheckDetailsQuery, con);
            InsertCheckDetailsCmd.Parameters.AddWithValue("@CID", CheckId);
            InsertCheckDetailsCmd.Parameters.AddWithValue("@Check_Details", CheckDetails);


            InsertCheckDetailsCmd.ExecuteNonQuery();
            con.Close();
            TempData["SuccessMessage"]="Check Details Updated Successfully !";
            return RedirectToAction("Index","Check");
        }

        [HttpGet]
        public IActionResult GetCheckCost(string? TypeName)
        {
            if (!string.IsNullOrWhiteSpace(TypeName))
            {
                DataTable Checks = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectCheckCostQuery = @"
                    SELECT Type_Cost
                    FROM [TypeDetails] 
                    WHERE Type_Name = @TypeName
                ";
                SqlCommand SelectCheckCostCmd = new SqlCommand(SelectCheckCostQuery,con);
                SelectCheckCostCmd.Parameters.AddWithValue("@TypeName",TypeName);

                con.Open();
                SqlDataReader sdr = SelectCheckCostCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Checks.Load(sdr);
                    sdr.Close();
                }
                con.Close();        
                return Json(Checks.Rows[0]["Type_Cost"]);
            }
            return null;
        }

    }
}
