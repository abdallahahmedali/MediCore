using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class FinancialTransaction
{
    public int Transaction_ID { get; set; }

    public double Transaction_Paid_Amount { get; set; }

    public DateTime Transaction_Date { get; set; }

    public string Patient_Username { get; set; } = null!;

    public DateTime Visit_Start_Date { get; set; }

    public static DataTable SelectOpenTransactions(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = @"
                        SELECT v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost, COALESCE(SUM(Transaction_Paid_Amount), 0) as Total_Paid 
                        FROM Visits as v
                        LEFT JOIN FinancialTransactions as fn ON fn.Visit_Start_Date = v.Visit_Start_Date AND fn.Patient_Username = v.Patient_Username
                        GROUP BY v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost
                        HAVING COALESCE(SUM(Transaction_Paid_Amount), 0) < Visit_Total_Cost
                        ";
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
    public static DataTable SelectOpenTransactions(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = @"
                        SELECT v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost, COALESCE(SUM(Transaction_Paid_Amount), 0) as Total_Paid 
                        FROM Visits as v
                        LEFT JOIN FinancialTransactions as fn ON fn.Visit_Start_Date = v.Visit_Start_Date AND fn.Patient_Username = v.Patient_Username
                        WHERE v.Patient_Username = @Username AND v.Visit_Start_Date = @Visit_Start_Date
                        GROUP BY v.Visit_Start_Date, v.Patient_Username, Visit_Total_Cost
                        HAVING COALESCE(SUM(Transaction_Paid_Amount), 0) < Visit_Total_Cost
                        ";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Username", Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", Visit_Start_Date);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }

    public int InsertTransaction(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = @"INSERT INTO FinancialTransactions (Visit_Start_Date, Patient_Username, Transaction_Paid_Amount, Transaction_Date) 
                        values(@Visit_Start_Date, @Patient_Username, @Transaction_Paid_Amount, CAST(CURRENT_TIMESTAMP AS DATE))";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Transaction_Paid_Amount", this.Transaction_Paid_Amount);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
}
