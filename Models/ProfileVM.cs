using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.Data.SqlClient;
namespace HospitalAppl.Models{
    public class ProfileVM{
        public ProfileVM() {}
        public DataRow Employee { get; set;}
        public DataRow Patient { get; set;}

        public DataTable Patients { get; set;}
        public DataTable Checks { get; set;}
        public DataTable Surgeries { get; set;}
        public DataTable Tests { get; set;}

        public DataTable TotalAmount { get; set;}
        public DataTable PaidAmount { get; set;}
        public ProfileVM(DataRow Employee,DataTable Patients,DataTable Checks,DataTable Surgeries,DataTable Tests)
        {
            this.Employee = Employee;
            this.Patient = Employee;
            this.Patients = Patients;
            this.Checks = Checks;
            this.Surgeries = Surgeries;
            this.Tests = Tests;
        }

        public ProfileVM(DataRow Employee,DataTable Patients,DataTable Checks,DataTable Surgeries,DataTable Tests,DataTable TotalTable)
        {
            this.Employee = Employee;
            this.Patient = Employee;
            this.Patients = Patients;
            this.Checks = Checks;
            this.Surgeries = Surgeries;
            this.Tests = Tests;
            this.TotalAmount = TotalTable;
        }


    }
}