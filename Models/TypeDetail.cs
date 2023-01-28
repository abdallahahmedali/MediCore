using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HospitalAppl.Models;

public partial class TypeDetail
{
   [Key]
    public string Type_Name { get; set; } = null!;

    public string Service_Type_Name { get; set; } = null!;

    public double Type_Cost { get; set; }
    public static DataTable SelectTypeDetails(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable TypeDetails = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM TypeDetails AS T " +
            "INNER JOIN ServiceTypes AS S ON T.Service_Type_Name = S.Service_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            TypeDetails.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return TypeDetails;
    }

    public static DataTable SelectTypeDetails(IConfiguration config, string Service_Type_Name, string Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable TypeDetails = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT * FROM TypeDetails " +
            "WHERE Service_Type_Name = @Service_Type_Name AND Type_Name = @Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", Service_Type_Name);
        Command.Parameters.AddWithValue("@Type_Name", Type_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            TypeDetails.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return TypeDetails;
    }

    public static DataTable SelectTypeDetails(IConfiguration config, string Service_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable TypeDetails = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT * FROM TypeDetails " +
            "WHERE Service_Type_Name = @Service_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", Service_Type_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            TypeDetails.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return TypeDetails;
    }

    public int InsertTypeDetails(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO TypeDetails (Service_Type_Name, Type_Name, Type_Cost) values(@Service_Type_Name, @Type_Name, @Type_Cost)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", this.Service_Type_Name);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Type_Cost", this.Type_Cost);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateTypeDetail(IConfiguration config, string Old_Service_Type_Name, string Old_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update TypeDetails SET Service_Type_Name = @Service_Type_Name, Type_Name = @Type_Name, Type_Cost = @Type_Cost " +
            "WHERE Service_Type_Name = @Old_Service_Type_Name AND Type_Name = @Old_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", this.Service_Type_Name);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Type_Cost", this.Type_Cost);
        Command.Parameters.AddWithValue("@Old_Type_Name", Old_Type_Name);
        Command.Parameters.AddWithValue("@Old_Service_Type_Name", Old_Service_Type_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    //public virtual ICollection<TypeDetail> TypeDetails { get; } = new List<TypeDetail>();
    public  bool CheckValidServiceTypeName(IConfiguration config, string ST)
    {

        //SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        //var SelectServiceTypeQuery = "SELECT * FROM TypeDetails WHERE Service_Type_Name =@Name";
        //SqlCommand SelectCmd = new SqlCommand(SelectServiceTypeQuery, con);
        //SelectCmd.Parameters.AddWithValue("@Name", ST);

        //con.Open();
        //SqlDataReader sdr = SelectCmd.ExecuteReader();

        //if (sdr.HasRows)
        //{
        //    sdr.Close();
        //    con.Close();
        //    return false;
        //}
        return true;
    }

}
