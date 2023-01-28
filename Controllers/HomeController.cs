using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using HospitalAppl.Data;    
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HospitalAppl.Libraies;
using System.IO;
using System.Xml.Linq;
using System.Data;
using HospitalAppl.Libraies;

namespace HospitalAppl.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration _config;

    public HomeController(ILogger<HomeController> logger,IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public IActionResult Index()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        return View();
    }
 

    [HttpGet]    
    public IActionResult PatientProfile(string? PatientUsername)
    {
        if(HttpContext.Session.GetString("Username") == null){
            return RedirectToAction("Index","Home");
        }
        if(PatientUsername != null)
        {
            // Select User Data From DB
            DataTable Users = new DataTable();
            DataTable Patients = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var query = "SELECT * FROM Users WHERE Username = @Username";
            SqlCommand cmd = new SqlCommand(query,con);
            cmd.Parameters.AddWithValue("@Username", PatientUsername);

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Users.Load(sdr);
                sdr.Close();
            }
            DataRow User = Users.Rows[0];
            con.Close();
        // Select Most Recent Checks From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Checks = new DataTable();
            var SelectChecksQuery = @"
                SELECT TOP 3 * FROM Checks AS C
                LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
            SelectChecksCmd.Parameters.AddWithValue("@Username", PatientUsername);

            con.Open();
            SqlDataReader SelectChecksReader = SelectChecksCmd.ExecuteReader();

            if (SelectChecksReader.HasRows)
            {
                Checks.Load(SelectChecksReader);
                SelectChecksReader.Close();
            }
            con.Close();

        // Select Most Recent Surgeries From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Surgeries = new DataTable();
            var SelectSurgeriesQuery = @"
                SELECT TOP 3 * FROM Surgeries AS S
                LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
            SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

            con.Open();
            SqlDataReader SelectSurgeriesReader = SelectSurgeriesCmd.ExecuteReader();

            if (SelectSurgeriesReader.HasRows)
            {
                Surgeries.Load(SelectSurgeriesReader);
                SelectSurgeriesReader.Close();
            }
            con.Close();

        // Select Most Recent Tests From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Tests = new DataTable();
            var SelectTestsQuery = @"
                SELECT TOP 3 * FROM Tests AS T
                LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
            SelectTestsCmd.Parameters.AddWithValue("@Username", PatientUsername);

            con.Open();
            SqlDataReader SelectTestsReader = SelectTestsCmd.ExecuteReader();

            if (SelectTestsReader.HasRows)
            {
                Tests.Load(SelectTestsReader);
                SelectTestsReader.Close();
            }
            con.Close();

            ProfileVM PatientModel = new ProfileVM(User,Patients,Checks,Surgeries,Tests);
            return View(PatientModel);
        }
        else
        {
            // Select User Data From DB
            DataTable Users = new DataTable();
            DataTable Patients = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var query = "SELECT * FROM Users WHERE Username = @Username";
            SqlCommand cmd = new SqlCommand(query,con);
            cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Users.Load(sdr);
                sdr.Close();
            }
            DataRow User = Users.Rows[0];
            con.Close();

            DataTable TotalVisitTransactions = new DataTable();
            DataTable PaidVisitTransactions = new DataTable();
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string TotalVisitQuery = @"
                SELECT v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost, COALESCE(SUM(Transaction_Paid_Amount), 0) as Total_Paid 
                FROM Visits as v
                LEFT JOIN FinancialTransactions as fn ON fn.Visit_Start_Date = v.Visit_Start_Date AND fn.Patient_Username = v.Patient_Username
                WHERE v.Patient_Username = @Username
                GROUP BY v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost
                HAVING COALESCE(SUM(Transaction_Paid_Amount), 0) < Visit_Total_Cost
            ";
            string PaidVisitQuery =  "SELECT SUM(Transaction_Paid_Amount) AS PVC FROM FinancialTransactions FT  WHERE Patient_Username = @Username";
            SqlCommand TotalVisitCmd = new SqlCommand(TotalVisitQuery,con);
            SqlCommand PaidVisitCmd = new SqlCommand(PaidVisitQuery,con);
            TotalVisitCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));
            PaidVisitCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader TotalVisitReader = TotalVisitCmd.ExecuteReader();

            if (TotalVisitReader.HasRows)
            {
                TotalVisitTransactions.Load(TotalVisitReader);
                TotalVisitReader.Close();
            }

            con.Close();
        // Select Most Recent Checks From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Checks = new DataTable();
            var SelectChecksQuery = @"
                SELECT TOP 3 * FROM Checks AS C
                LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON C.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
            SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader SelectChecksReader = SelectChecksCmd.ExecuteReader();

            if (SelectChecksReader.HasRows)
            {
                Checks.Load(SelectChecksReader);
                SelectChecksReader.Close();
            }
            con.Close();

        // Select Most Recent Surgeries From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Surgeries = new DataTable();
            var SelectSurgeriesQuery = @"
                SELECT TOP 3 * FROM Surgeries AS S
                LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
            SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader SelectSurgeriesReader = SelectSurgeriesCmd.ExecuteReader();

            if (SelectSurgeriesReader.HasRows)
            {
                Surgeries.Load(SelectSurgeriesReader);
                SelectSurgeriesReader.Close();
            }
            con.Close();

        // Select Most Recent Tests From DB
            con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            DataTable Tests = new DataTable();
            var SelectTestsQuery = @"
                SELECT TOP 3 * FROM Tests AS T
                LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
                INNER JOIN Users U ON T.Patient_Username = @Username AND U.Username = @Username
            ";
            SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
            SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader SelectTestsReader = SelectTestsCmd.ExecuteReader();

            if (SelectTestsReader.HasRows)
            {
                Tests.Load(SelectTestsReader);
                SelectTestsReader.Close();
            }
            con.Close();

            ProfileVM PatientModel = new ProfileVM(User,Patients,Checks,Surgeries,Tests,TotalVisitTransactions);
            return View(PatientModel);
        }

    }

    public IActionResult EmployeeProfile()
    {
        if(HttpContext.Session.GetString("Username") == null){
            return RedirectToAction("Index","Home");
        }
        // Select User Data From DB
        DataTable Users = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var query = "SELECT * FROM Users U,Employees E WHERE U.Username = @Username AND E.Username = @Username";
        SqlCommand cmd = new SqlCommand(query,con);
        cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader sdr = cmd.ExecuteReader();

        if (sdr.HasRows)
        {
            Users.Load(sdr);
            sdr.Close();
        }
        DataRow User = Users.Rows[0];
        con.Close();
        // Select Most Recent Employee Patients From DB
        con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        DataTable Patients = new DataTable();
        var SelectPatientsQuery = @"
            SELECT TOP 3 SUBQ2.PUsername AS Patient_Username,First_Name,Middle_Name,Last_Name,SUBQ2.Date,SUBQ2.Profile_Pic,SUBQ2.ST_Name AS Service_Type_Name FROM
            (
                SELECT V.Patient_Username AS PUsername,First_Name,Middle_Name,Last_Name,Check_Date AS Date,Profile_Pic,ST.Service_Type_Name AS ST_Name
                FROM Visits AS V,Checks AS C,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
                WHERE Doctor_Username=@Username 
                AND U.Username=V.Patient_Username 
                AND V.Patient_Username = C.Patient_Username
                AND V.Visit_Start_Date = C.Visit_Start_Date
                AND C.Type_Name = TD.Type_Name
                AND TD.Service_Type_Name=ST.Service_Type_Name

            UNION

                SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Surgery_Date AS Date,Profile_Pic,ST.Service_Type_Name
                FROM Visits AS V,Surgeries AS S,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
                WHERE Doctor_Username=@Username 
                AND U.Username=V.Patient_Username 
                AND V.Patient_Username = S.Patient_Username
                AND V.Visit_Start_Date = S.Visit_Start_Date
                AND S.Type_Name = TD.Type_Name
                AND TD.Service_Type_Name=ST.Service_Type_Name
            UNION

                SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Test_Date AS Date,Profile_Pic,ST.Service_Type_Name
                FROM Visits AS V,Tests AS T,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
                WHERE Doctor_Username=@Username 
                AND U.Username=V.Patient_Username 
                AND V.Patient_Username = T.Patient_Username
                AND V.Visit_Start_Date = T.Visit_Start_Date
                AND T.Type_Name = TD.Type_Name
                AND TD.Service_Type_Name=ST.Service_Type_Name
            ) AS SUBQ2
            WHERE SUBQ2.Date IN (
            SELECT MAX(SUBQ.ServiceDate) FROM
            (
                    SELECT MAX(C.Check_Date) AS ServiceDate,V.Patient_Username AS P_Username 
                    FROM Visits AS V,Checks AS C
                    WHERE Doctor_Username=@Username 
                    AND V.Patient_Username = C.Patient_Username
                    GROUP BY(V.Patient_Username)

                UNION

                    SELECT MAX(S.Surgery_Date) AS ServiceDate,V.Patient_Username AS P_Username 
                    FROM Visits AS V,Surgeries AS S
                    WHERE Doctor_Username=@Username 
                    AND V.Patient_Username = S.Patient_Username
                    GROUP BY(V.Patient_Username)

                UNION

                    SELECT MAX(T.Test_Date) AS ServiceDate,V.Patient_Username AS P_Username 
                    FROM Visits AS V,Tests AS T
                    WHERE Doctor_Username=@Username 
                    AND V.Patient_Username = T.Patient_Username
                    GROUP BY(V.Patient_Username)

            ) AS SUBQ GROUP BY (P_Username)

            )
        ";

        SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
        SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader SelectPatientsReader = SelectPatientsCmd.ExecuteReader();

        if (SelectPatientsReader.HasRows)
        {
            Patients.Load(SelectPatientsReader);
            SelectPatientsReader.Close();
        }
        con.Close();
       // Select Most Recent Checks From DB
        con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        DataTable Checks = new DataTable();
        var SelectChecksQuery = @"
            SELECT TOP 3 * FROM Checks AS C
            LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
            INNER JOIN Users U ON C.Doctor_Username = @Username AND U.Username = @Username
        ";
        SqlCommand SelectChecksCmd = new SqlCommand(SelectChecksQuery,con);
        SelectChecksCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader SelectChecksReader = SelectChecksCmd.ExecuteReader();

        if (SelectChecksReader.HasRows)
        {
            Checks.Load(SelectChecksReader);
            SelectChecksReader.Close();
        }
        con.Close();

       // Select Most Recent Surgeries From DB
        con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        DataTable Surgeries = new DataTable();
        var SelectSurgeriesQuery = @"
            SELECT TOP 3 * FROM Surgeries AS S
            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
        ";
        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader SelectSurgeriesReader = SelectSurgeriesCmd.ExecuteReader();

        if (SelectSurgeriesReader.HasRows)
        {
            Surgeries.Load(SelectSurgeriesReader);
            SelectSurgeriesReader.Close();
        }
        con.Close();

       // Select Most Recent Tests From DB
        con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        DataTable Tests = new DataTable();
        var SelectTestsQuery = @"
            SELECT TOP 3 * FROM Tests AS T
            LEFT JOIN RoomTypes RT ON T.Room_Type_Name = RT.Room_Type_Name
            INNER JOIN Users U ON T.Doctor_Username = @Username AND U.Username = @Username
        ";
        SqlCommand SelectTestsCmd = new SqlCommand(SelectTestsQuery,con);
        SelectTestsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader SelectTestsReader = SelectTestsCmd.ExecuteReader();

        if (SelectTestsReader.HasRows)
        {
            Tests.Load(SelectTestsReader);
            SelectTestsReader.Close();
        }
        con.Close();

        ProfileVM EmployeeModel = new ProfileVM(User,Patients,Checks,Surgeries,Tests);
        return View(EmployeeModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(User UserObj)
    {
        
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        // Check User Info Matches A Record In The DB
        var getUserQuery = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
        SqlCommand getUserCmd = new SqlCommand(getUserQuery, con);

        // Check Wether The User Is A Patient Or An Employee
        var getPatientQuery = "SELECT * FROM Patients WHERE Username = @Username";
        SqlCommand getPatientCmd = new SqlCommand(getPatientQuery, con);

        // Bind Parameter Value For getUserQuery
        getUserCmd.Parameters.AddWithValue("@Username", UserObj.Username);
        getUserCmd.Parameters.AddWithValue("@Password",Encrypt.GetSHA1(UserObj.Password));

        // Bind Parameter Value For getPatientQuery
        getPatientCmd.Parameters.AddWithValue("@Username", UserObj.Username);

        con.Open();
        SqlDataReader Users = getUserCmd.ExecuteReader();
        DataTable UserTable = new DataTable();
        if (Users.HasRows)
        {   
            UserTable.Load(Users);
            HttpContext.Session.SetString("Username", UserObj.Username);
            HttpContext.Session.SetString("ProfilePicture", UserTable.Rows[0]["Profile_Pic"].ToString());
            Users.Close();
            SqlDataReader Patients = getPatientCmd.ExecuteReader();

            // Store User Type In The Session
            if (Patients.HasRows) HttpContext.Session.SetString("UserType", "Patient"); else HttpContext.Session.SetString("UserType", "Employee");
            if(Convert.ToBoolean(UserTable.Rows[0]["Is_Admin"]) == true) HttpContext.Session.SetString("UserType", "Admin");
            Patients.Close();
            con.Close();
            TempData["SuccessMessage"] = "Logged In Succesfully !";
            if(HttpContext.Session.GetString("UserType") == "Admin") return RedirectToAction("Index","Visit");
            if(HttpContext.Session.GetString("UserType") == "Employee"){
                return RedirectToAction("EmployeeProfile");
            }else return RedirectToAction("PatientProfile");

        }

        TempData["DangerMessage"] = "Failed To Login Username And Password Don't Match !";
        return RedirectToAction("CreateUser", "User");

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
