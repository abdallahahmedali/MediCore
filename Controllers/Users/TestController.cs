using HospitalAppl.Libraies;
using HospitalAppl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace HospitalAppl.Controllers.Users
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        private readonly IConfiguration _config;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public TestController(ILogger<TestController> logger, IConfiguration config,IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(string? select,string? PatientUsername){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if(!string.IsNullOrEmpty(PatientUsername)){
                DataTable Tests;
                if(select == "Completed"){
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        WHERE Test_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Test_Is_Rejected = 0
                        AND Test_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Upcoming"){
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Test_Is_Rejected = 0
                        AND Test_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Test_Is_Rejected = 1;
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Awaiting"){
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Test_Is_Rejected = 0
                        AND Test_Is_Confirmed = 0;
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Test_Is_Rejected = 1;
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else{
                    Tests = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectTestsQuery = @"
                        SELECT * FROM Tests AS T
                        LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                        LEFT JOIN Users A ON T.Doctor_Username = A.Username
                    ";
                    SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                    SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Tests.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }

                return View(Tests);
            }else{
                DataTable Tests;
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    if(select == "Completed"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Test_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON T.Patient_Username = A.Username
                            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
            }else{
                    if(select == "Completed"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                            WHERE Test_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 0
                            AND Test_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                            WHERE Test_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Test_Is_Rejected = 1;
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Tests = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectTestsQuery = @"
                            SELECT * FROM Tests AS T
                            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
                            LEFT JOIN Users A ON T.Doctor_Username = A.Username
                        ";
                        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
                        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectTestsCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Tests.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
            }

                return View(Tests);
            }

        }
  
        [HttpGet]
        public IActionResult CreateRequest(string? PatientUsername){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            DataTable TypeNames = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var SelectTestTypesQuery = @"SELECT * FROM TypeDetails AS TD,ServiceTypes AS ST WHERE TD.Service_Type_Name = ST.Service_Type_Name AND ST.Service_Type_Name='Test'";
            SqlCommand SelectTestTypesCmd = new SqlCommand(SelectTestTypesQuery,con);

            con.Open();
            SqlDataReader sdr = SelectTestTypesCmd.ExecuteReader();

            if (sdr.HasRows)
            {
                TypeNames.Load(sdr);
                sdr.Close();
            }
            con.Close();
            TestVM TestObj= new TestVM(TypeNames); 
            return View(TestObj);
        }

        [HttpPost]
        public IActionResult CreateRequest(TestVM TestObj){
            
            DateTime NewVisitStartDate = Visit.InitVisit(_config,TestObj.Test.Patient_Username,Convert.ToDateTime(TestObj.Test.Test_Date));
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            if(TestObj.Test.Test_Date >= DateTime.Now){
                string InsertRequestQuery = "INSERT INTO Tests(Visit_Start_Date,Patient_Username,Test_Date,Type_Name,Doctor_Username) VALUES(@VisitStartDate,@Username,@Date,@Type,@DoctorUsername);";
                SqlCommand InsertRequestCmd = new SqlCommand(InsertRequestQuery, con);
                InsertRequestCmd.Parameters.AddWithValue("@Username", TestObj.Test.Patient_Username);
                InsertRequestCmd.Parameters.AddWithValue("@Date", TestObj.Test.Test_Date);
                InsertRequestCmd.Parameters.AddWithValue("@Type", TestObj.Test.Type_Name);
                InsertRequestCmd.Parameters.AddWithValue("@VisitStartDate", NewVisitStartDate);
                InsertRequestCmd.Parameters.AddWithValue("@DoctorUsername", HttpContext.Session.GetString("Username"));
                con.Open();
                try{
                    InsertRequestCmd.ExecuteNonQuery();
                }catch(Exception){
                    con.Close();
                    TempData["DangerMessage"] = "Your "+ TestObj.Test.Type_Name + " Request Failed Please Try Again Later !";
                    return RedirectToAction("PatientProfile","Home", new { PatientUsername = TestObj.Test.Patient_Username });
                }

                TempData["SuccessMessage"] = "Your "+ TestObj.Test.Type_Name + " Request Was Sent To The Admin !";
                con.Close();
                return RedirectToAction("PatientProfile","Home", new { PatientUsername = TestObj.Test.Patient_Username });
            }else{

                TempData["DangerMessage"] = "Your "+ TestObj.Test.Type_Name + " Request Failed Please Try Again Later !";
                return RedirectToAction("PatientProfile","Home", new { PatientUsername = TestObj.Test.Patient_Username });
            }
        }

        [HttpPost]
        public IActionResult UploadTestFile(IFormFile TestFile,string TID){
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var InsertTestFileQuery = "UPDATE Tests SET Test_Report_File = @File WHERE Test_ID = @TID";
            SqlCommand InsertTestFileCmd = new SqlCommand(InsertTestFileQuery,con);
            // Binding Parameters For insertUserQuery
            var stringFileName = UploadFile(TestFile);

            InsertTestFileCmd.Parameters.AddWithValue("@File", stringFileName);
            InsertTestFileCmd.Parameters.AddWithValue("@TID", TID);
            // Binding Parameters For InsertTestFileQuery

            InsertTestFileCmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {                   
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            Test test = new Test(_config, HttpContext.Session.GetString("Username"));

            ViewBag.TypeNamesAndCost=Test.SelectTypeNamesAndCost(_config);
            ViewBag.DoctorNames = Test.SelectDoctors(_config);
            ViewBag.TestDetails = Test.SelectTests(_config, HttpContext.Session.GetString("Username"));
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.UserType= HttpContext.Session.GetString("UserType");
            ViewBag.RoomNumbers = Test.SelectRoomNumber(_config);
            ViewBag.TimeNow = DateTime.Now.Date;
            return View("/Views/Test/Create.cshtml",test);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Test test)
        {
            if(test.Test_Date.Date > DateTime.Now.Date)
            {
                DateTime NewVisitStartDate = Visit.InitVisit(_config,HttpContext.Session.GetString("Username"),Convert.ToDateTime(test.Test_Date));
                if (Test.ChechValidHour(_config,test.Room_Number,test.Test_Date)) 
                {
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    con.Open();
                    var insertTestQuery = "INSERT INTO Tests(Patient_Username,Type_Name,Test_Date,Test_Cost,Visit_Start_Date) VALUES (@Patient_Username,@Type_Name,@Test_Date,@Test_Cost,@Visit_Start_Date)";
                    SqlCommand insertTestCmd = new SqlCommand(insertTestQuery, con);

                    //will be auto increamented
                    insertTestCmd.Parameters.AddWithValue("@Patient_Username", HttpContext.Session.GetString("Username"));
                    insertTestCmd.Parameters.AddWithValue("@Type_Name", test.Type_Name);
                    insertTestCmd.Parameters.AddWithValue("@Test_Date", test.Test_Date);
                    insertTestCmd.Parameters.AddWithValue("@Test_Cost", test.Test_Cost);
                    insertTestCmd.Parameters.AddWithValue("@Visit_Start_Date", NewVisitStartDate);
                    try
                    {
                        insertTestCmd.ExecuteNonQuery();
                        con.Close();
                        DataTable Tests = new DataTable();
                        Tests = Test.SelectTests(_config, HttpContext.Session.GetString("Username"));
                    }
                    catch (Exception)
                    {
                        TempData["DangerMessage"] = "Failed To Add Test !";
                        return RedirectToAction("Index");
                    }
        

                    TempData["SuccessMessage"] = "Test Requested Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["DangerMessage"] = "Failure, The Entered Hour Isn't Available";
                    return View("Create");
                }
            }
            else
            {
                
                TempData["DangerMessage"] = "Failure, The Entered Date Is Invalid";
                return View("Create");
            }
        }
        [HttpGet]
        public IActionResult Edit(string? ID)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if (ID == null)
            {
                TempData["DangerMessage"] = "Failure ,Wrong Test";
                return RedirectToAction("Index");
            }
            DataTable Test = new DataTable();
            DataTable test = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
              var SelectTestQuery = "SELECT * FROM Tests WHERE Test_ID = @ID";
            SqlCommand SelectTypeNamesCmd = new SqlCommand(SelectTestQuery, con);
            SelectTypeNamesCmd.Parameters.AddWithValue("@ID", ID);

            con.Open();

            SqlDataReader sdr = SelectTypeNamesCmd.ExecuteReader();

            if (sdr.HasRows)
            {
                test.Load(sdr);
                sdr.Close();
                con.Close();
            }
            else
            {
                con.Close();
                TempData["DangerMessage"] = "Failure";
                return RedirectToAction("Index");
            }

          
            ViewBag.Test = test;
            return View("/Views/User/Test/Edit.cshtml",ViewBag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Test test, int prevID)
        {
            
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var UpdateTestQuery = "Update Tests SET Type_Name=@Type_Name,Test_Date=@Test_Date,Test_Cost=@Test_Cost WHERE Test_ID = @prevID";
            SqlCommand UpdateTestCmd = new SqlCommand(UpdateTestQuery, con);
            //binding params

            UpdateTestCmd.Parameters.AddWithValue("@Type_Name", test.Type_Name);
            UpdateTestCmd.Parameters.AddWithValue("@Test_Date", test.Test_Date);
            UpdateTestCmd.Parameters.AddWithValue("@Test_Cost",test.Test_Cost);
            UpdateTestCmd.Parameters.AddWithValue("@prevID", prevID);
            UpdateTestCmd.ExecuteNonQuery();
            con.Close();
            //if (validation)
            //{


            TempData["SuccessMessage"] = "Test Updated Successfully";
                return RedirectToAction("Index");
            //}
            //else
            //TempData["DangerMessage"] = "Failure";
            //return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(string? ID)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if (ID != null)
            {
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();
                string DeleteTestQuery = "DELETE FROM Tests WHERE Test_ID = @ID";
                SqlCommand DeleteTestCmd = new SqlCommand(DeleteTestQuery, con);
                // Binding Parameters 

                DeleteTestCmd.Parameters.AddWithValue("@ID", ID);
                DeleteTestCmd.ExecuteNonQuery();
                con.Close();
                TempData["SuccessMessage"] = "Test Deleted Successfully";
                return RedirectToAction("Index");
            }

            TempData["DangerMessage"] = "Failure";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetTestCost(string? TypeName)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if (!string.IsNullOrWhiteSpace(TypeName))
            {
                DataTable Tests = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectTestCostQuery = @"
                    SELECT Type_Cost
                    FROM [TypeDetails] 
                    WHERE Type_Name = @TypeName
                ";
                SqlCommand SelectTestCostCmd = new SqlCommand(SelectTestCostQuery,con);
                SelectTestCostCmd.Parameters.AddWithValue("@TypeName",TypeName);

                con.Open();
                SqlDataReader sdr = SelectTestCostCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Tests.Load(sdr);
                    sdr.Close();
                }
                con.Close();        
                return Json(Tests.Rows[0]["Type_Cost"]);
            }
            return null;
        }

        private String UploadFile(IFormFile testDetailFile)
        {
            String fileName = null;

            if(testDetailFile != null){
                String Dir = Path.Combine(_webHostEnvironment.WebRootPath,"files");
                fileName = Guid.NewGuid().ToString() + "-" +  testDetailFile.FileName;
                String filePath = Path.Combine(Dir,fileName);
                
                using(var fileStream = new FileStream(filePath,FileMode.Create)){
                    testDetailFile.CopyTo(fileStream);
                }
            }

            return fileName;

        }

    }
}
