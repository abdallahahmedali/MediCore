using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class Patient
{
     [Key]
    public string Username { get; set; } = null!;

    public DateTime Patient_Birthdate { get; set; }

    public string? Patient_History_Details { get; set; }

    //public virtual User UsernameNavigation { get; set; } = null!;

    //public virtual ICollection<Visit> Visits { get; } = new List<Visit>();

    public static int PatientsCount(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable TypeDetails = new DataTable();
        DBCon.Open();
        string query = "SELECT COUNT(*) FROM Patients";
        SqlCommand Command = new SqlCommand(query, DBCon);
        int PCount = (int)Command.ExecuteScalar();
        DBCon.Close();

        return PCount;
    }

    public static DataTable SelectPatient(IConfiguration config, string Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Patients = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Patients AS p " +
            "INNER JOIN Users AS u ON u.Username = p.Username " +
            "WHERE p.Username = @Username";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.Add("@Username", SqlDbType.VarChar).Value = Username;
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Patients.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Patients;
    }

    public static DataTable SelectPatients(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Patients = new DataTable();
        DBCon.Open();
        string query = @"
                         SELECT * FROM Patients AS p
                         INNER JOIN Users AS u ON u.Username = p.Username 
                        ";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Patients.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Patients;
    }

}
