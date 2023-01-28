using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HospitalAppl.Models;

public partial class Job
{
    [Key]
    public string Job_Name { get; set; } = null!;

    //public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
    public Job()
    {

    }
    public Job(DataRow JobRow)
    {
        Job_Name = JobRow["Job_Name"].ToString();
    }

    public static bool CheckValidJobPk(IConfiguration config, string PK)
    {
        DataTable Jobs = new DataTable();
        SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        var SelectJobQuery = "SELECT * FROM Jobs WHERE Job_Name = @Name";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectJobQuery, con);
        SelectJobsCmd.Parameters.AddWithValue("@Name", PK);

        con.Open();
        SqlDataReader sdr = SelectJobsCmd.ExecuteReader();

        if (sdr.HasRows)
        {
            sdr.Close();
            con.Close();
            return false;
        }
        return true;
    }


    public static int GetNumWorkers(IConfiguration config, string jobName)
    {
        DataTable Jobs = new DataTable();
        SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        string SelectCountWorkersQuery = "SELECT COUNT(*) FROM Employees WHERE Job_Name = @Name";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectCountWorkersQuery, con);
        SelectJobsCmd.Parameters.AddWithValue("@Name", jobName);

        con.Open();
        con.Close();
        return Convert.ToInt32(SelectJobsCmd.ExecuteScalar());
    }

    public static DataTable GetAllJobs(IConfiguration config)
    {
        DataTable Jobs = new DataTable();
        SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        string SelectCountWorkersQuery = "SELECT * FROM Jobs";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectCountWorkersQuery, con);

        con.Open();
        SqlDataReader sdr = SelectJobsCmd.ExecuteReader();


        if (sdr.HasRows)
        {
            Jobs.Load(sdr);
            sdr.Close();
        }
        con.Close();
        return Jobs;
    }
}
