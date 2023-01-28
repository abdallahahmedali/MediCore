using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.Data.SqlClient;
namespace HospitalAppl.Models{
    public class EmployeeVM{
        public EmployeeVM(){}

        public User User { get; set;}
        public Employee Employee { get; set;}

        public DataTable Jobs { get; set;}

        public DataTable Departments { get; set;}

        public EmployeeVM(DataTable Jobs,DataTable Departments)
        {
            this.Jobs = Jobs;
            this.Departments = Departments;
        }
        public EmployeeVM(DataRow UserRow){
            User = new User();
            Employee = new Employee();
            User.Username = UserRow["Username"].ToString();
            User.Password = UserRow["Password"].ToString();
            User.First_Name = UserRow["First_Name"].ToString();
            User.Middle_Name = UserRow["Middle_Name"].ToString();
            User.Last_Name = UserRow["Last_Name"].ToString();
            User.Start_Date = Convert.ToDateTime(UserRow["Start_Date"]);
            User.Street = UserRow["Street"].ToString();
            User.District = UserRow["District"].ToString();
            User.City = UserRow["City"].ToString();
            User.Sex = Convert.ToBoolean(UserRow["Sex"]);
            User.Nationality = UserRow["Nationality"].ToString();
            User.National_ID = UserRow["National_ID"].ToString();
            User.Phone = UserRow["Phone"].ToString();
            User.Email = UserRow["Email"].ToString();
            User.Profile_Pic = UserRow["Profile_Pic"].ToString();
            Employee.Username = UserRow["Username"].ToString();
            Employee.Job_Name = UserRow["Job_Name"].ToString();
            Employee.Department_Name = UserRow["Department_Name"].ToString();
            Employee.Employee_Salary = Convert.ToDouble(UserRow["Employee_Salary"]);
        }
        public EmployeeVM(DataRow UserRow,DataTable Jobs,DataTable Departments){
            User = new User();
            Employee = new Employee();
            this.Jobs = Jobs;
            this.Departments = Departments;
            User.Username = UserRow["Username"].ToString();
            User.Password = UserRow["Password"].ToString();
            User.First_Name = UserRow["First_Name"].ToString();
            User.Middle_Name = UserRow["Middle_Name"].ToString();
            User.Last_Name = UserRow["Last_Name"].ToString();
            User.Start_Date = Convert.ToDateTime(UserRow["Start_Date"]);
            User.Street = UserRow["Street"].ToString();
            User.District = UserRow["District"].ToString();
            User.City = UserRow["City"].ToString();
            User.Sex = Convert.ToBoolean(UserRow["Sex"]);
            User.Nationality = UserRow["Nationality"].ToString();
            User.National_ID = UserRow["National_ID"].ToString();
            User.Phone = UserRow["Phone"].ToString();
            User.Email = UserRow["Email"].ToString();
            User.Profile_Pic = UserRow["Profile_Pic"].ToString();
            Employee.Username = UserRow["Username"].ToString();
            Employee.Job_Name = UserRow["Job_Name"].ToString();
            Employee.Department_Name = UserRow["Department_Name"].ToString();
            Employee.Employee_Salary = Convert.ToDouble(UserRow["Employee_Salary"]);
        }
        public static bool CheckValidUserPk(IConfiguration config,string PK)
        {
            DataTable Jobs = new DataTable();
            SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
            var SelectUserQuery = "SELECT * FROM Users WHERE Username = @Name";
            SqlCommand SelectUserCmd = new SqlCommand(SelectUserQuery, con);
            SelectUserCmd.Parameters.AddWithValue("@Name", PK);

            con.Open();
            SqlDataReader sdr = SelectUserCmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Close();
                con.Close();
                return false;
            }
            return true;
        }

    }
}