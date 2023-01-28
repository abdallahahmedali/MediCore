using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class MedicalConsultation
{
     [Key]
    public int Consultation_ID { get; set; }
    public DateTime Visit_Start_Date { get; set; }
    public string Patient_Username { get; set; } = null!;
    public string Type_Name { get; set; } = null!;
    public string Room_Type_Name { get; set; } = null!;
    public int Room_Number { get; set; }
    public string Doctor_Username { get; set; } = null!;

    public DateTime Check_Date { get; set; }

    public double Consultation_Cost { get; set; }

    public bool Consultation_IsDone { get; set; }

    public DateTime? Consultation_Date { get; set; }

    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;

    public static DataTable SelectConsultations(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM MedicalConsultations " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = MedicalConsultations.Room_Type_Name";
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


    public static DataTable SelectConsultations(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT Consultation_ID, Consultation_Cost, Consultation_Date, Patient_Username, Visit_Start_Date, Type_Name, MedicalConsultations.Room_Type_Name, MedicalConsultations.Room_Number " +
            "From MedicalConsultations " +
            "LEFT JOIN Rooms ON Rooms.Room_Type_Name = MedicalConsultations.Room_Type_Name AND Rooms.Room_Number = MedicalConsultations.Room_Number " +
            "WHERE MedicalConsultations.Consultation_ID = @Consultation_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Consultation_ID", _id);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }

    public static DataTable SelectConsultations(IConfiguration config, DataRow CheckObj)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT Consultation_ID, Consultation_Cost, Consultation_Date, Patient_Username, Visit_Start_Date, Type_Name, MedicalConsultations.Room_Type_Name, MedicalConsultations.Room_Number " +
            "From MedicalConsultations " +
            "LEFT JOIN Rooms ON Rooms.Room_Type_Name = MedicalConsultations.Room_Type_Name AND Rooms.Room_Number = MedicalConsultations.Room_Number " +
            "WHERE Check_Date = @Check_Date AND Patient_Username = @Patient_Username " +
            "AND Visit_Start_Date = @Visit_Start_Date AND Type_Name = @Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Check_Date", CheckObj["Check_Date"]);
        Command.Parameters.AddWithValue("@Patient_Username", CheckObj["Patient_Username"]);
        Command.Parameters.AddWithValue("@Visit_Start_Date", CheckObj["Visit_Start_Date"]);
        Command.Parameters.AddWithValue("@Type_Name", CheckObj["Type_Name"]);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public int InsertConsultation(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = @"INSERT INTO MedicalConsultations 
                        (Type_Name, Consultation_Date, Check_Date, Room_Number, Room_Type_Name, 
                        Visit_Start_Date, Patient_Username, Doctor_Username, Consultation_Cost)
                        values(@Type_Name, @Consultation_Date, @Check_Date, @Room_Number, 
                        @Room_Type_Name, @Visit_Start_Date, @Patient_Username, 
                        @Doctor_Username, @Consultation_Cost)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Consultation_Date", this.Consultation_Date);
        Command.Parameters.AddWithValue("@Check_Date", this.Check_Date);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Consultation_Cost", this.Consultation_Cost);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public int UpdateConsultation(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update MedicalConsultations SET Room_Number = @Room_Number, Consultation_Date = @Consultation_Date " +
            "WHERE Check_ID = @Check_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        //Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Consultation_Date", this.Consultation_Date);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Consultation_ID", this.Consultation_ID);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;
}
