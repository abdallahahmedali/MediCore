using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalAppl.Models;

public partial class Department
{
    [Required]
    public string Department_Name { get; set; } = null!;

    //public virtual ICollection<Employee> Employees { get; } = new List<Employee>();

    public static DataTable SelectDepartments(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomType = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Departments";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomType.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomType;
    }
    public static DataTable SelectDepartments(IConfiguration config, string Department_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Departments = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Departments WHERE Department_Name = @Department_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Department_Name", Department_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Departments.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Departments;
    }
    public int InsertDepartment(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO Departments (Department_Name) values(@Department_Name)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Department_Name", this.Department_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateDepartment(IConfiguration config, string Old_Department_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Departments SET Department_Name = @Department_Name WHERE Department_Name = @Old_Department_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Department_Name", this.Department_Name);
        Command.Parameters.AddWithValue("@Old_Department_Name", Old_Department_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
}
