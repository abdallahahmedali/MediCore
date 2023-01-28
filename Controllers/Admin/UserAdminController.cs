using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using static HospitalAppl.Models.User;
using HospitalAppl.Data;    
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HospitalAppl.Libraies;
using System.Data;

namespace HospitalAppl.Controllers.Admin {
public class UserAdminController : Controller
{
    private readonly ILogger<UserAdminController> _logger;

    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _webHostEnvironment;
     
    public UserAdminController(ILogger<UserAdminController> logger,IConfiguration config,IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _config = config;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index(){
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
        DataTable Employees = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var SelectJobQuery = "SELECT * FROM Users AS U,Employees AS E WHERE U.Username = E.Username";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectJobQuery,con);

        con.Open();
        SqlDataReader sdr = SelectJobsCmd.ExecuteReader();

        if (sdr.HasRows)
        {
            Employees.Load(sdr);
            sdr.Close();
        }
        con.Close();

        return View("/Views/Admin/User/Index.cshtml", Employees); 
    }

    [HttpGet]
    public IActionResult CreateAdminUser()
    {
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
        DataTable Jobs = Job.GetAllJobs(_config);
        DataTable Departments = Department.SelectDepartments(_config);
        EmployeeVM Employee = new EmployeeVM(Jobs,Departments);
        return View("/Views/Admin/User/CreateAdminUser.cshtml", Employee);
    }

    [HttpGet]
    public IActionResult EditAdminUser(string? editUserame)
    {
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
        if(editUserame != null){
            DataTable Jobs = Job.GetAllJobs(_config);
            DataTable Departments = Department.SelectDepartments(_config);

            DataTable Users = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var query = "SELECT * FROM Users AS U,Employees AS E WHERE U.Username = E.Username AND E.Username = @Username";
            SqlCommand cmd = new SqlCommand(query,con);
            cmd.Parameters.AddWithValue("@Username", editUserame);

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Users.Load(sdr);
                sdr.Close();
            }
            con.Close();

            DataRow User = Users.Rows[0];
            EmployeeVM EmployeeObj = new EmployeeVM(User,Jobs,Departments);

            return View("/Views/Admin/User/EditAdminUser.cshtml", EmployeeObj);
        }
        TempData["State"] = "Failure";
        return RedirectToAction("Index","Home");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAdminUser(EmployeeVM UserObj,String Gender)
    {           
            if(EmployeeVM.CheckValidUserPk(_config,UserObj.User.Username)){
               SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();
                var InsertUserQuery = "INSERT INTO Users (Username,Password,First_Name,Middle_Name,Last_Name,Street,District,City,Sex,Nationality,National_ID,Phone,Email,Profile_Pic) VALUES (@Username,@Password,@FirstName,@MiddleName,@LastName,@Street,@District,@City,@Sex,@Nationality,@NationalId,@Phone,@Email,@Picture)";
                var InsertEmployeeQuery = "INSERT INTO Employees (Username,Job_Name,Department_Name,Employee_Salary) VALUES (@Username,@JobName,@DepartmentName,@Salary)";
                SqlCommand InsertUserCmd = new SqlCommand(InsertUserQuery,con);
                SqlCommand InsertEmployeeCmd = new SqlCommand(InsertEmployeeQuery,con);
                // Binding Parameters For InsertUserQuery
                var stringFileName = UploadFile(UserObj.User.ProfilePictureFile);
                InsertUserCmd.Parameters.AddWithValue("@Password", Encrypt.GetSHA1(UserObj.User.Password));
                InsertUserCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
                InsertUserCmd.Parameters.AddWithValue("@FirstName", UserObj.User.First_Name);
                InsertUserCmd.Parameters.AddWithValue("@MiddleName", UserObj.User.Middle_Name);
                InsertUserCmd.Parameters.AddWithValue("@LastName", UserObj.User.Last_Name);
                InsertUserCmd.Parameters.AddWithValue("@Email", UserObj.User.Email);
                InsertUserCmd.Parameters.AddWithValue("@Street", UserObj.User.Street);
                InsertUserCmd.Parameters.AddWithValue("@District", UserObj.User.District);
                InsertUserCmd.Parameters.AddWithValue("@City", UserObj.User.City);
                InsertUserCmd.Parameters.AddWithValue("@Nationality", UserObj.User.Nationality);
                InsertUserCmd.Parameters.AddWithValue("@NationalId", UserObj.User.National_ID);
                InsertUserCmd.Parameters.AddWithValue("@Phone", UserObj.User.Phone);
                InsertUserCmd.Parameters.AddWithValue("@Picture", stringFileName);
                // Binding Parameters For InsertEmployeeQuery
                InsertEmployeeCmd.Parameters.AddWithValue("@Username", UserObj.User.Username);
                InsertEmployeeCmd.Parameters.AddWithValue("@JobName", UserObj.Employee.Job_Name);
                InsertEmployeeCmd.Parameters.AddWithValue("@DepartmentName", UserObj.Employee.Department_Name);
                InsertEmployeeCmd.Parameters.AddWithValue("@Salary", UserObj.Employee.Employee_Salary);

                if (Gender == "F")
                    InsertUserCmd.Parameters.AddWithValue("@Sex", 1);
                else
                    InsertUserCmd.Parameters.AddWithValue("@Sex", 0);

                InsertUserCmd.ExecuteNonQuery();
                InsertEmployeeCmd.ExecuteNonQuery();
                con.Close();
                


                TempData["SuccessMessage"] = "Employee Added Successfully";
                return RedirectToAction("Index","Home");

            }

            TempData["DangerMessage"] = "Failed To Add Employee";
            return RedirectToAction("Index","Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditAdminUser(EmployeeVM UserObj,String Gender)
    {
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();
                string UpdateUserQuery,stringFileName;
                if(UserObj.User.ProfilePictureFile != null){
                    stringFileName = UploadFile(UserObj.User.ProfilePictureFile);
                    UpdateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email,Profile_Pic=@Picture WHERE Username ='"+HttpContext.Session.GetString("Username")+"';";
                }else
                    UpdateUserQuery = "UPDATE Users SET Username=@Username,Password=@Password,First_Name=@FirstName,Middle_Name=@MiddleName,Last_Name=@LastName,Street=@Street,District=@District,City=@City,Sex=@Sex,Nationality=@Nationality,National_ID=@NationalId,Phone=@Phone,Email=@Email WHERE Username =@Username;";
                
                string UpdateEmployeeQuery = "UPDATE Employees SET Job_Name = @JobName,Department_Name = @DepartmentName,Employee_Salary = @Salary WHERE Username =@Username;";

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
                UpadteEmployeeCmd.Parameters.AddWithValue("@JobName", UserObj.Employee.Job_Name);
                UpadteEmployeeCmd.Parameters.AddWithValue("@DepartmentName", UserObj.Employee.Department_Name);
                UpadteEmployeeCmd.Parameters.AddWithValue("@Salary", UserObj.Employee.Employee_Salary);

                if (UserObj.User.ProfilePictureFile != null) UpadteUserCmd.Parameters.AddWithValue("@Picture", UserObj.User.ProfilePictureFile);

                if (Gender == "F")
                    UpadteUserCmd.Parameters.AddWithValue("@Sex", 1);
                else
                    UpadteUserCmd.Parameters.AddWithValue("@Sex", 0);

                UpadteUserCmd.ExecuteNonQuery();
                UpadteEmployeeCmd.ExecuteNonQuery();
                con.Close();
                

                TempData["State"] = "Success";
                return RedirectToAction("Index");
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

}
