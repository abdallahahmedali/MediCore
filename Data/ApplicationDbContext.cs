using HospitalAppl.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppl.Data{
    public class ApplicationDbContext: DbContext
    {
        static string DB_Connection_String = @"Data Source=localhost;Initial Catalog=Medicore;Integrated Security=True";

        // Empty constructor needed for dependency injection
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }


        public virtual DbSet<Check> Checks { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<FinancialTransaction> FinancialTransactions { get; set; }

        public virtual DbSet<Job> Jobs { get; set; }

        public virtual DbSet<MedicalConsultation> MedicalConsultations { get; set; }

        public virtual DbSet<Patient> Patients { get; set; }

        public virtual DbSet<Room> Rooms { get; set; }

        public virtual DbSet<RoomReservation> RoomReservations { get; set; }

        public virtual DbSet<RoomType> RoomTypes { get; set; }

        public virtual DbSet<ServiceType> ServiceTypes { get; set; }

        public virtual DbSet<Surgery> Surgeries { get; set; }

        public virtual DbSet<Test> Tests { get; set; }


        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Visit> Visits { get; set; }


    }
}