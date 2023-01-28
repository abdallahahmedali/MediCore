using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HospitalAppl.Models;

public partial class Employee
{
    [Key]
    public string Username { get; set; } = null!;

    public string Job_Name { get; set; } = null!;

    public string Department_Name { get; set; } = null!;

    public double Employee_Salary { get; set; }

    public DateTime? Employee_End_Date { get; set; }

    //public virtual Department DepartmentNameNavigation { get; set; } = null!;

    //public virtual Job JobNameNavigation { get; set; } = null!;

    //public virtual User UsernameNavigation { get; set; } = null!;

    public static DataTable SelectDoctors(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Employees " +
            "INNER JOIN Users ON Users.Username = Employees.Username " +
            "WHERE Users.Is_Admin = 0";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }
    public static IEnumerable<SelectListItem> GetEmployeesFromDepartment(string conStr,string DepartmentName)
    {
        if (!String.IsNullOrWhiteSpace(DepartmentName))
        {
            List<SelectListItem> EmployeesList = new List<SelectListItem>();
            DataTable Employees = new DataTable();
            SqlConnection con = new SqlConnection(conStr);
            string SelectEmployeesQuery = "SELECT * FROM Users AS U,Employees AS E WHERE U.Username = E.Username AND E.Department_Name=@DepName";
            SqlCommand SelectEmployeesCmd = new SqlCommand(SelectEmployeesQuery, con);

            con.Open();
            SelectEmployeesCmd.Parameters.AddWithValue("@DepName", DepartmentName);
            SqlDataReader sdr = SelectEmployeesCmd.ExecuteReader();


            if (sdr.HasRows)
            {
                Employees.Load(sdr);
                sdr.Close();
            }
            con.Close();

            foreach(DataRow Employee in Employees.Rows){
                EmployeesList.Add(new SelectListItem{ Value = Employee["Username"].ToString(), Text = Employee["Username"].ToString() });
            }
            return new SelectList(EmployeesList, "Value", "Text");
        }
        return null;
    }

    public static DataTable SelectEmployees(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Employees " +
            "INNER JOIN Users ON Users.Username = Employees.Username";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }

    public static int Promote(IConfiguration config, string Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = @"Update Users SET Is_Admin = 1 WHERE Username = @Username";
        SqlCommand Command = new SqlCommand(query, DBCon);
        //Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Username", Username);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
}
