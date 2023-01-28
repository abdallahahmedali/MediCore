using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using HospitalAppl.Data;    
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HospitalAppl.Libraies;
using System.Data;
using Microsoft.Reporting.NETCore;

namespace HospitalAppl.Controllers {
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _webHostEnvironment;
     
    public UserController(ILogger<UserController> logger,IConfiguration config,IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _config = config;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult CreateUser()
    {

        return View("/Views/User/CreateUser.cshtml");
    }

    public IActionResult EditUser()
    {
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists !";
            return RedirectToAction("Index","Home");
        }
        DataTable Users = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        string query;
        if(HttpContext.Session.GetString("UserType") == "Patient"){
            query = "SELECT * FROM Users AS U,Patients AS P WHERE U.Username = P.Username AND P.Username = @Username";
        }else{
            query = "SELECT * FROM Users AS U,Employees AS E WHERE U.Username = E.Username AND E.Username = @Username";
        }
        SqlCommand cmd = new SqlCommand(query,con);
        cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

        con.Open();
        SqlDataReader sdr = cmd.ExecuteReader();

        if (sdr.HasRows)
        {
            Users.Load(sdr);
            sdr.Close();
        }
        con.Close();

        DataRow User = Users.Rows[0];
        if(HttpContext.Session.GetString("UserType") == "Patient"){
            PatientVM PatientObj = new PatientVM(User);
            return View("/Views/User/EditPatient.cshtml", PatientObj);
        }else{
            EmployeeVM EmployeeObj = new EmployeeVM(User);
            return View("/Views/User/EditEmployee.cshtml", EmployeeObj);
        }

    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Username");
        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateUser(PatientVM UserObj,string Gender)
    {
        if(UserObj.User.First_Name.All(Char.IsLetter) && UserObj.User.Middle_Name.All(Char.IsLetter) && UserObj.User.Last_Name.All(Char.IsLetter) && UserObj.Patient.Patient_Birthdate < DateTime.Now && UserObj.User.National_ID.ToString().Length == 14 && !UserObj.User.National_ID.All(Char.IsLetter) && UserObj.User.Phone.ToString().Length == 11 && !UserObj.User.Phone.All(Char.IsLetter) && !string.IsNullOrWhiteSpace(UserObj.User.Nationality)){
           SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var insertUserQuery = "INSERT INTO Users (Username,Password,First_Name,Middle_Name,Last_Name,Street,District,City,Sex,Nationality,National_ID,Phone,Email,Profile_Pic) VALUES (@Username,@Password,@FirstName,@MiddleName,@LastName,@Street,@District,@City,@Sex,@Nationality,@NationalId,@Phone,@Email,@Picture)";
            var insertPatientQuery = "INSERT INTO Patients (Username,Patient_Birthdate) VALUES (@Username,@Birthdate)";
            SqlCommand insertUserCmd = new SqlCommand(insertUserQuery,con);
            SqlCommand insertPatientCmd = new SqlCommand(insertPatientQuery,con);
            // Binding Parameters For insertUserQuery
            var stringFileName = UploadFile(UserObj.User.ProfilePictureFile);
            insertUserCmd.Parameters.AddWithValue("@Password", Encrypt.GetSHA1(UserObj.User.Password));
            insertUserCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
            insertUserCmd.Parameters.AddWithValue("@FirstName", UserObj.User.First_Name);
            insertUserCmd.Parameters.AddWithValue("@MiddleName", UserObj.User.Middle_Name);
            insertUserCmd.Parameters.AddWithValue("@LastName", UserObj.User.Last_Name);
            insertUserCmd.Parameters.AddWithValue("@Email", UserObj.User.Email);
            insertUserCmd.Parameters.AddWithValue("@Street", UserObj.User.Street);
            insertUserCmd.Parameters.AddWithValue("@District", UserObj.User.District);
            insertUserCmd.Parameters.AddWithValue("@City", UserObj.User.City);
            insertUserCmd.Parameters.AddWithValue("@Nationality", UserObj.User.Nationality);
            insertUserCmd.Parameters.AddWithValue("@NationalId", UserObj.User.National_ID);
            insertUserCmd.Parameters.AddWithValue("@Phone", UserObj.User.Phone);
            insertUserCmd.Parameters.AddWithValue("@Picture", stringFileName);
            // Binding Parameters For insertPatientQuery
            insertPatientCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
            insertPatientCmd.Parameters.AddWithValue("@Birthdate", UserObj.Patient.Patient_Birthdate);
            insertPatientCmd.Parameters.AddWithValue("@Picture", stringFileName);

            if (Gender == "F")
                insertUserCmd.Parameters.AddWithValue("@Sex", true);
            else
                insertUserCmd.Parameters.AddWithValue("@Sex", false);
            try{
                insertUserCmd.ExecuteNonQuery();
                insertPatientCmd.ExecuteNonQuery();  
            }catch(Exception){
                con.Close();
                TempData["DangerMessage"] = "Username Already Exists !";
                return RedirectToAction("CreateUser");
            }

            con.Close();
            
            TempData["SuccessMessage"] = "Welcome To Medicore !";
            return RedirectToAction("Index","Home");
        }else{
            TempData["DangerMessage"] = "Failed To Save User Please Try Again !";
            return RedirectToAction("CreateUser");
        }
 
 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditPatient(PatientVM UserObj,String Gender)
    {        
        if(UserObj.User.First_Name.All(Char.IsLetter) && UserObj.User.Middle_Name.All(Char.IsLetter) && UserObj.User.Last_Name.All(Char.IsLetter) && UserObj.Patient.Patient_Birthdate < DateTime.Now  && UserObj.User.National_ID.ToString().Length == 14 && !UserObj.User.National_ID.All(Char.IsLetter) && UserObj.User.Phone.ToString().Length == 11 && !UserObj.User.Phone.All(Char.IsLetter) && !string.IsNullOrWhiteSpace(UserObj.User.Nationality)){
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();
                string updateUserQuery,stringFileName;
                if(UserObj.User.ProfilePictureFile != null){
                    updateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email,Profile_Pic=@Picture WHERE Username ='"+HttpContext.Session.GetString("Username")+"';";
                }else
                    updateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email WHERE Username ='"+HttpContext.Session.GetString("Username")+"';";
                
                string updatePatientQuery = "UPDATE Patients SET Patient_History_Details = @HistoryDetails,Patient_Birthdate = @Birthdate WHERE Username ='" + HttpContext.Session.GetString("Username") + "';";

                SqlCommand upadteUserCmd = new SqlCommand(updateUserQuery,con);
                SqlCommand upadtePatientCmd = new SqlCommand(updatePatientQuery,con);
                // Binding Parameters For 
                upadteUserCmd.Parameters.AddWithValue("@Password", Encrypt.GetSHA1(UserObj.User.Password));
                upadteUserCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
                upadteUserCmd.Parameters.AddWithValue("@FirstName", UserObj.User.First_Name);
                upadteUserCmd.Parameters.AddWithValue("@MiddleName", UserObj.User.Middle_Name);
                upadteUserCmd.Parameters.AddWithValue("@LastName", UserObj.User.Last_Name);
                upadteUserCmd.Parameters.AddWithValue("@Email", UserObj.User.Email);
                upadteUserCmd.Parameters.AddWithValue("@Street", UserObj.User.Street);
                upadteUserCmd.Parameters.AddWithValue("@District", UserObj.User.District);
                upadteUserCmd.Parameters.AddWithValue("@City", UserObj.User.City);
                upadteUserCmd.Parameters.AddWithValue("@Nationality", UserObj.User.Nationality);
                upadteUserCmd.Parameters.AddWithValue("@NationalId", UserObj.User.National_ID);
                upadteUserCmd.Parameters.AddWithValue("@Phone", UserObj.User.Phone);
                // Q2 Params
                upadtePatientCmd.Parameters.AddWithValue("@HistoryDetails", UserObj.Patient.Patient_History_Details);
                upadtePatientCmd.Parameters.AddWithValue("@Birthdate", UserObj.Patient.Patient_Birthdate);

                if (UserObj.User.ProfilePictureFile != null) {
                    stringFileName = UploadFile(UserObj.User.ProfilePictureFile);
                    upadteUserCmd.Parameters.AddWithValue("@Picture", stringFileName);
                }

                if (Gender == "F")
                    upadteUserCmd.Parameters.AddWithValue("@Sex", true);
                else
                    upadteUserCmd.Parameters.AddWithValue("@Sex", false);

                try{
                    upadteUserCmd.ExecuteNonQuery();
                    upadtePatientCmd.ExecuteNonQuery();
                    con.Close();
                }catch(Exception){
                    HttpContext.Session.SetString("Username",UserObj.User.Username);
                    TempData["DangerMessage"] = "Failed To Update User Info !";
                    if(HttpContext.Session.GetString("UserType") == "Employee"){
                        return RedirectToAction("EmployeeProfile","Home");
                    }else return RedirectToAction("PatientProfile","Home");
                }

                HttpContext.Session.SetString("Username",UserObj.User.Username);
                TempData["SuccessMessage"] = "User Info. Updated Successfully !";
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    return RedirectToAction("EmployeeProfile","Home");
                }else return RedirectToAction("PatientProfile","Home");

        }else{
                TempData["DangerMessage"] = "Failed To Update User Info !";
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    return RedirectToAction("EmployeeProfile","Home");
                }else return RedirectToAction("PatientProfile","Home");
        }
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditEmployee(EmployeeVM UserObj,String Gender)
    {
        if(UserObj.User.First_Name.All(Char.IsLetter) && UserObj.User.Middle_Name.All(Char.IsLetter) && UserObj.User.Last_Name.All(Char.IsLetter) && UserObj.User.National_ID.ToString().Length == 14 && !UserObj.User.National_ID.All(Char.IsLetter) && UserObj.User.Phone.ToString().Length == 11 && !UserObj.User.Phone.All(Char.IsLetter) && !string.IsNullOrWhiteSpace(UserObj.User.Nationality)){
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            string UpdateUserQuery,stringFileName;
            if(UserObj.User.ProfilePictureFile != null){
                UpdateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email,Profile_Pic=@Picture WHERE Username ='"+HttpContext.Session.GetString("Username")+"';";
            }else
                UpdateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email WHERE Username =@Username;";
            
            string UpdateEmployeeQuery = "UPDATE Employees SET Username = @Username WHERE Username =@Username;";

            SqlCommand UpadteUserCmd = new SqlCommand(UpdateUserQuery,con);
            SqlCommand UpadteEmployeeCmd = new SqlCommand(UpdateEmployeeQuery,con);
            // Binding Parameters For 
            UpadteUserCmd.Parameters.AddWithValue("@Password", Encrypt.GetSHA1(UserObj.User.Password));
            UpadteUserCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
            UpadteUserCmd.Parameters.AddWithValue("@FirstName", UserObj.User.First_Name);
            UpadteUserCmd.Parameters.AddWithValue("@MiddleName", UserObj.User.Middle_Name);
            UpadteUserCmd.Parameters.AddWithValue("@LastName", UserObj.User.Last_Name);
            UpadteUserCmd.Parameters.AddWithValue("@Email", UserObj.User.Email);
            UpadteUserCmd.Parameters.AddWithValue("@Street", UserObj.User.Street);
            UpadteUserCmd.Parameters.AddWithValue("@District", UserObj.User.District);
            UpadteUserCmd.Parameters.AddWithValue("@City", UserObj.User.City);
            UpadteUserCmd.Parameters.AddWithValue("@Nationality", UserObj.User.Nationality);
            UpadteUserCmd.Parameters.AddWithValue("@NationalId", UserObj.User.National_ID);
            UpadteUserCmd.Parameters.AddWithValue("@Phone", UserObj.User.Phone);
            // Q2 Params
            UpadteEmployeeCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);

            if (UserObj.User.ProfilePictureFile != null) {
                stringFileName = UploadFile(UserObj.User.ProfilePictureFile);
                UpadteUserCmd.Parameters.AddWithValue("@Picture", stringFileName);
            }
            if (Gender == "F")
                UpadteUserCmd.Parameters.AddWithValue("@Sex", 1);
            else
                UpadteUserCmd.Parameters.AddWithValue("@Sex", 0);

            try{
                UpadteUserCmd.ExecuteNonQuery();
                UpadteEmployeeCmd.ExecuteNonQuery();
            }catch(Exception){
                con.Close();
                TempData["DangerMessage"] = "Failed To Update Your Info !";
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    return RedirectToAction("EmployeeProfile","Home");
                }else return RedirectToAction("PatientProfile","Home");                 
            }


            HttpContext.Session.SetString("Username",UserObj.User.Username);
            TempData["SuccessMessage"] = "Your Info Was Updated Succesfully !";
            if(HttpContext.Session.GetString("UserType") == "Employee"){
                return RedirectToAction("EmployeeProfile","Home");
            }else return RedirectToAction("PatientProfile","Home"); 
        }else{
            TempData["DangerMessage"] = "Failed To Update Your Info !";
            if(HttpContext.Session.GetString("UserType") == "Employee"){
                return RedirectToAction("EmployeeProfile","Home");
            }else return RedirectToAction("PatientProfile","Home");
        }
    }  

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(PatientVM UserObj)
    {
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        // Check User Info Matches A Record In The DB
        var getUserQuery = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
        SqlCommand getUserCmd = new SqlCommand(getUserQuery, con);

        // Check Wether The User Is A Patient Or An Employee
        var getPatientQuery = "SELECT * FROM Patients WHERE Username = @Username";
        SqlCommand getPatientCmd = new SqlCommand(getPatientQuery, con);

        // Bind Parameter Value For getUserQuery
        getUserCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
        getUserCmd.Parameters.AddWithValue("@Password",Encrypt.GetSHA1(UserObj.User.Password));

        // Bind Parameter Value For getPatientQuery
        getPatientCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);

        con.Open();
        SqlDataReader Users = getUserCmd.ExecuteReader();
        DataTable UserTable = new DataTable();
        if (Users.HasRows)
        {   
            UserTable.Load(Users);
            HttpContext.Session.SetString("Username", UserObj.User.Username);
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

    private String UploadFile(IFormFile profilePictureFile)
    {
        String fileName = null;

        if(profilePictureFile != null){
            String Dir = Path.Combine(_webHostEnvironment.WebRootPath,"Images");
            fileName = Guid.NewGuid().ToString() + "-" +  profilePictureFile.FileName;
            String filePath = Path.Combine(Dir,fileName);
            using(var fileStream = new FileStream(filePath,FileMode.Create)){
                profilePictureFile.CopyTo(fileStream);
            }
        }

        return fileName;

    }


        public IActionResult Hospital_income()
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using var report = new LocalReport();
            var dt1 = new DataTable();
            dt1 = Check_TypeName_Costs();
            var dt2 = new DataTable();
            dt2 = MConsult_TypeName_Costs();
            var dt3 = new DataTable();
            dt3 = Surgery_TypeName_Costs();
            var dt4 = new DataTable();
            dt4 = Test_TypeName_Costs();
            report.DataSources.Add(new ReportDataSource("DataSet1", dt1));
            report.DataSources.Add(new ReportDataSource("DataSet2", dt2));
            report.DataSources.Add(new ReportDataSource("DataSet3", dt3));
            report.DataSources.Add(new ReportDataSource("DataSet4", dt4));
            report.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\Reports\\rptCosts.rdlc";
            var pdf = report.Render(renderFormat);
            return File(pdf, mimetype, "report." + extension);

        }

        public DataTable Check_TypeName_Costs()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Costs = new DataTable();
            var query = "Select Type_Name,Check_Cost From Checks;";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Costs.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Costs;
        }
        public DataTable MConsult_TypeName_Costs()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Costs = new DataTable();
            var query = "Select Type_Name,Consultation_Cost From MedicalConsultations;";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Costs.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Costs;
        }
        public DataTable Surgery_TypeName_Costs()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Costs = new DataTable();
            var query = "Select Type_Name,Surgery_Cost From Surgeries;";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Costs.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Costs;
        }
        public DataTable Test_TypeName_Costs()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Costs = new DataTable();
            var query = "Select Type_Name,Test_Cost From Tests;";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Costs.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Costs;
        }
        ////////////////////////////////////////////////////
        public IActionResult Print()
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using var report = new LocalReport();
            var dt = new DataTable();
            dt = Get_Check_Details();
            var dt2 = new DataTable();
            dt2 = Get_MConsult_Details();
            var dt3 = new DataTable();
            dt3 = Get_Surgery_Details();
            var dt4 = new DataTable();
            dt4 = Get_Test_Details();
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));
            report.DataSources.Add(new ReportDataSource("DataSet2", dt2));
            report.DataSources.Add(new ReportDataSource("DataSet3", dt3));
            report.DataSources.Add(new ReportDataSource("DataSet4", dt4));
            report.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\Reports\\rptCheckForPatient.rdlc";
            var pdf = report.Render(renderFormat);
            return File(pdf, mimetype, "report." + extension);

        }
        public DataTable Get_Check_Details()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select First_Name,Last_Name,Check_Date,Check_Cost From Checks,Users where Patient_Username=@Username and Username=Doctor_Username;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Details;
        }
        public DataTable Get_MConsult_Details()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select First_Name,Last_Name,Consultation_Date,Consultation_Cost,Check_Date From MedicalConsultations,Users where Patient_Username=@Username and Username=Doctor_Username;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Details;
        }
        public DataTable Get_Surgery_Details()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select First_Name,Last_Name,Surgery_Date,Surgery_Cost From Surgeries,Users where Patient_Username=@Username and Username=Doctor_Username;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Details;
        }
        public DataTable Get_Test_Details()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select First_Name,Last_Name,Test_Date,Test_Cost,Test_Delivery_Date From Tests,Users where Patient_Username=@Username and Username=Doctor_Username;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            return Details;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

}
