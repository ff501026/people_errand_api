using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace People_errand_api.Models
{
    public partial class people_errandContext : DbContext
    {
        public people_errandContext()
        {
        }

        public people_errandContext(DbContextOptions<people_errandContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeInformation> EmployeeInformations { get; set; }
        public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public virtual DbSet<EmployeeTripRecord> EmployeeTripRecords { get; set; }
        public virtual DbSet<EmployeeTripType> EmployeeTripTypes { get; set; }
        public virtual DbSet<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }
        public virtual DbSet<EmployeeWorkType> EmployeeWorkTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SalaryRecord> SalaryRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=163.18.110.100;Initial Catalog=people_errand;Persist Security Info=True;User ID=e215api;Password=E2152k7au4a83");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CompanyHash);

                entity.ToTable("company");

                entity.Property(e => e.CompanyHash)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.CompanyId)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("company_id")
                    .HasDefaultValueSql("([dbo].[add_company]())")
                    .IsFixedLength(true);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(128)
                    .HasColumnName("email");

                entity.Property(e => e.ManagerHash)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.HasOne(d => d.ManagerHashNavigation)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.ManagerHash)
                    .HasConstraintName("FK_company_employee");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.HashAccount);

                entity.ToTable("employee");

                entity.Property(e => e.HashAccount)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.CompanyHash)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmployeeId)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("employee_id")
                    .HasDefaultValueSql("([dbo].[add_employee]())")
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.PhoneCode)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("phone_code");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.CompanyHash)
                    .HasConstraintName("FK_employee_company");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_role");
            });

            modelBuilder.Entity<EmployeeInformation>(entity =>
            {
                entity.HasKey(e => e.InformationId);

                entity.ToTable("employee_information");

                entity.Property(e => e.InformationId).HasColumnName("information_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("email");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeInformations)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_information_employee");
            });

            modelBuilder.Entity<EmployeeSchedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);

                entity.ToTable("employee_schedule");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmployeeHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("employee_hash");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time");

                entity.Property(e => e.ManagerHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_hash");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");

                entity.HasOne(d => d.EmployeeHashNavigation)
                    .WithMany(p => p.EmployeeScheduleEmployeeHashNavigations)
                    .HasForeignKey(d => d.EmployeeHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_schedule_employee1");

                entity.HasOne(d => d.ManagerHashNavigation)
                    .WithMany(p => p.EmployeeScheduleManagerHashNavigations)
                    .HasForeignKey(d => d.ManagerHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_schedule_employee");
            });

            modelBuilder.Entity<EmployeeTripRecord>(entity =>
            {
                entity.HasKey(e => e.TripRecordsId);

                entity.ToTable("employee_trip_records");

                entity.Property(e => e.TripRecordsId).HasColumnName("tripRecords_id");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("reason");

                entity.Property(e => e.TripTypeId).HasColumnName("trip_type_id");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeTripRecords)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_trip_records_employee");

                entity.HasOne(d => d.TripType)
                    .WithMany(p => p.EmployeeTripRecords)
                    .HasForeignKey(d => d.TripTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_trip_records_employee_trip_type");
            });

            modelBuilder.Entity<EmployeeTripType>(entity =>
            {
                entity.HasKey(e => e.TripTypeId);

                entity.ToTable("employee_trip_type");

                entity.Property(e => e.TripTypeId).HasColumnName("trip_type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<EmployeeWorkRecord>(entity =>
            {
                entity.HasKey(e => e.WorkRecordsId);

                entity.ToTable("employee_workRecords");

                entity.Property(e => e.WorkRecordsId).HasColumnName("workRecords_id");

                entity.Property(e => e.CoordinateX).HasColumnName("coordinate_X");

                entity.Property(e => e.CoordinateY).HasColumnName("coordinate_Y");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeWorkRecords)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_workRecords_employee");

                entity.HasOne(d => d.WorkType)
                    .WithMany(p => p.EmployeeWorkRecords)
                    .HasForeignKey(d => d.WorkTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_workRecords_employee_work_type");
            });

            modelBuilder.Entity<EmployeeWorkType>(entity =>
            {
                entity.HasKey(e => e.WorkTypeId);

                entity.ToTable("employee_work_type");

                entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<SalaryRecord>(entity =>
            {
                entity.HasKey(e => e.SalaryRecordsId);

                entity.ToTable("salary_records");

                entity.Property(e => e.SalaryRecordsId).HasColumnName("salaryRecords_id");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmployeeHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("employee_hash");

                entity.Property(e => e.ManagerHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_hash");

                entity.Property(e => e.Money).HasColumnName("money");

                entity.Property(e => e.PayMonth).HasColumnName("pay_month");

                entity.HasOne(d => d.EmployeeHashNavigation)
                    .WithMany(p => p.SalaryRecordEmployeeHashNavigations)
                    .HasForeignKey(d => d.EmployeeHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_salary_records_employee");

                entity.HasOne(d => d.ManagerHashNavigation)
                    .WithMany(p => p.SalaryRecordManagerHashNavigations)
                    .HasForeignKey(d => d.ManagerHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_salary_records_employee1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
