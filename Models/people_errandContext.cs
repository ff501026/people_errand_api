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
        public virtual DbSet<EmployeeDepartmentType> EmployeeDepartmentTypes { get; set; }
        public virtual DbSet<EmployeeFlexibleWorktime> EmployeeFlexibleWorktimes { get; set; }
        public virtual DbSet<EmployeeGeneralWorktime> EmployeeGeneralWorktimes { get; set; }
        public virtual DbSet<EmployeeInformation> EmployeeInformations { get; set; }
        public virtual DbSet<EmployeeJobtitleType> EmployeeJobtitleTypes { get; set; }
        public virtual DbSet<EmployeeLeaveRecord> EmployeeLeaveRecords { get; set; }
        public virtual DbSet<EmployeeLeaveType> EmployeeLeaveTypes { get; set; }
        public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public virtual DbSet<EmployeeTrip2Record> EmployeeTrip2Records { get; set; }
        public virtual DbSet<EmployeeTrip2Type> EmployeeTrip2Types { get; set; }
        public virtual DbSet<EmployeeTripRecord> EmployeeTripRecords { get; set; }
        public virtual DbSet<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }
        public virtual DbSet<EmployeeWorkType> EmployeeWorkTypes { get; set; }
        public virtual DbSet<ManagerAccount> ManagerAccounts { get; set; }
        public virtual DbSet<ManagerPermission> ManagerPermissions { get; set; }
        public virtual DbSet<ManagerPermissionsCustomization> ManagerPermissionsCustomizations { get; set; }
        public virtual DbSet<ManagerPermissionsType> ManagerPermissionsTypes { get; set; }
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

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("address");

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

                entity.Property(e => e.CoordinateX)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_X");

                entity.Property(e => e.CoordinateY)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_Y");

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

                entity.Property(e => e.ManagerPassword)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_password");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.RestTime)
                    .HasColumnType("time(4)")
                    .HasColumnName("rest_time");

                entity.Property(e => e.WorkTime)
                    .HasColumnType("time(4)")
                    .HasColumnName("work_time");
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

                entity.Property(e => e.Enabled).HasColumnName("enabled");

                entity.Property(e => e.ManagerHash)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_hash");

                entity.Property(e => e.ManagerKey)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("manager_key");

                entity.Property(e => e.ManagerKeyOverDate)
                    .HasColumnType("datetime")
                    .HasColumnName("manager_key_over_date");

                entity.Property(e => e.PhoneCode)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("phone_code")
                    .IsFixedLength(true);

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.WorktimeId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("worktime_id")
                    .IsFixedLength(true);

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.CompanyHash)
                    .HasConstraintName("FK_employee_company");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_role");

                entity.HasOne(d => d.Worktime)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.WorktimeId)
                    .HasConstraintName("FK_employee_employee_flexible_worktime");

                entity.HasOne(d => d.WorktimeNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.WorktimeId)
                    .HasConstraintName("FK_employee_employee_general_worktime");
            });

            modelBuilder.Entity<EmployeeDepartmentType>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);

                entity.ToTable("employee_department_type");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.CompanyHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.EmployeeDepartmentTypes)
                    .HasForeignKey(d => d.CompanyHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_department_type_company");
            });

            modelBuilder.Entity<EmployeeFlexibleWorktime>(entity =>
            {
                entity.HasKey(e => e.FlexibleWorktimeId);

                entity.ToTable("employee_flexible_worktime");

                entity.Property(e => e.FlexibleWorktimeId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("flexible_worktime_id")
                    .HasDefaultValueSql("([dbo].[add_flexible_worktime_id]())")
                    .IsFixedLength(true);

                entity.Property(e => e.BreakTime).HasColumnName("break_time");

                entity.Property(e => e.CompanyHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.RestTimeEnd)
                    .HasColumnType("time(4)")
                    .HasColumnName("rest_time_end");

                entity.Property(e => e.RestTimeStart)
                    .HasColumnType("time(4)")
                    .HasColumnName("rest_time_start");

                entity.Property(e => e.WorkTimeEnd)
                    .HasColumnType("time(4)")
                    .HasColumnName("work_time_end");

                entity.Property(e => e.WorkTimeStart)
                    .HasColumnType("time(4)")
                    .HasColumnName("work_time_start");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.EmployeeFlexibleWorktimes)
                    .HasForeignKey(d => d.CompanyHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_flexible_worktime_company");
            });

            modelBuilder.Entity<EmployeeGeneralWorktime>(entity =>
            {
                entity.HasKey(e => e.GeneralWorktimeId);

                entity.ToTable("employee_general_worktime");

                entity.Property(e => e.GeneralWorktimeId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("general_worktime_id")
                    .HasDefaultValueSql("([dbo].[add_general_worktime_id]())")
                    .IsFixedLength(true);

                entity.Property(e => e.BreakTime).HasColumnName("break_time");

                entity.Property(e => e.CompanyHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.RestTime)
                    .HasColumnType("time(4)")
                    .HasColumnName("rest_time");

                entity.Property(e => e.WorkTime)
                    .HasColumnType("time(4)")
                    .HasColumnName("work_time");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.EmployeeGeneralWorktimes)
                    .HasForeignKey(d => d.CompanyHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_general_worktime_company");
            });

            modelBuilder.Entity<EmployeeInformation>(entity =>
            {
                entity.HasKey(e => e.InformationId);

                entity.ToTable("employee_information");

                entity.Property(e => e.InformationId).HasColumnName("information_id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("email");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.Img)
                    .HasMaxLength(256)
                    .HasColumnName("img");

                entity.Property(e => e.JobtitleId).HasColumnName("jobtitle_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeInformations)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_employee_information_employee_department_type");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeInformations)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_information_employee1");

                entity.HasOne(d => d.Jobtitle)
                    .WithMany(p => p.EmployeeInformations)
                    .HasForeignKey(d => d.JobtitleId)
                    .HasConstraintName("FK_employee_information_employee_jobtitle_type");
            });

            modelBuilder.Entity<EmployeeJobtitleType>(entity =>
            {
                entity.HasKey(e => e.JobtitleId);

                entity.ToTable("employee_jobtitle_type");

                entity.Property(e => e.JobtitleId).HasColumnName("jobtitle_id");

                entity.Property(e => e.CompanyHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.EmployeeJobtitleTypes)
                    .HasForeignKey(d => d.CompanyHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_jobtitle_type_company");
            });

            modelBuilder.Entity<EmployeeLeaveRecord>(entity =>
            {
                entity.HasKey(e => e.LeaveRecordsId);

                entity.ToTable("employee_leaveRecords");

                entity.Property(e => e.LeaveRecordsId).HasColumnName("leaveRecords_id");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("reason");

                entity.Property(e => e.Review).HasColumnName("review");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeLeaveRecords)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_leaveRecords_employee");

                entity.HasOne(d => d.LeaveType)
                    .WithMany(p => p.EmployeeLeaveRecords)
                    .HasForeignKey(d => d.LeaveTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_leaveRecords_employee_leave_type");
            });

            modelBuilder.Entity<EmployeeLeaveType>(entity =>
            {
                entity.HasKey(e => e.LeaveTypeId);

                entity.ToTable("employee_leave_type");

                entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
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

            modelBuilder.Entity<EmployeeTrip2Record>(entity =>
            {
                entity.HasKey(e => e.Trip2RecordsId);

                entity.ToTable("employee_trip2Records");

                entity.Property(e => e.Trip2RecordsId).HasColumnName("trip2Records_id");

                entity.Property(e => e.CoordinateX)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_X");

                entity.Property(e => e.CoordinateY)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_Y");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GroupId).HasColumnName("group_id");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.Trip2TypeId).HasColumnName("trip2_type_id");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeTrip2Records)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_trip2Records_employee1");

                entity.HasOne(d => d.Trip2Type)
                    .WithMany(p => p.EmployeeTrip2Records)
                    .HasForeignKey(d => d.Trip2TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_trip2Records_employee_trip2_type1");
            });

            modelBuilder.Entity<EmployeeTrip2Type>(entity =>
            {
                entity.HasKey(e => e.Trip2TypeId);

                entity.ToTable("employee_trip2_type");

                entity.Property(e => e.Trip2TypeId).HasColumnName("trip2_type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<EmployeeTripRecord>(entity =>
            {
                entity.HasKey(e => e.TripRecordsId)
                    .HasName("PK_employee_trip_records");

                entity.ToTable("employee_tripRecords");

                entity.Property(e => e.TripRecordsId).HasColumnName("tripRecords_id");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

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

                entity.Property(e => e.Review).HasColumnName("review");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.EmployeeTripRecords)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_trip_records_employee");
            });

            modelBuilder.Entity<EmployeeWorkRecord>(entity =>
            {
                entity.HasKey(e => e.WorkRecordsId);

                entity.ToTable("employee_workRecords");

                entity.Property(e => e.WorkRecordsId).HasColumnName("workRecords_id");

                entity.Property(e => e.CoordinateX)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_X");

                entity.Property(e => e.CoordinateY)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("coordinate_Y");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Enabled).HasColumnName("enabled");

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

            modelBuilder.Entity<ManagerAccount>(entity =>
            {
                entity.HasKey(e => e.ManagerId);

                entity.ToTable("manager_account");

                entity.Property(e => e.ManagerId).HasColumnName("manager_id");

                entity.Property(e => e.Enabled).HasColumnName("enabled");

                entity.Property(e => e.HashAccount)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("hash_account");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PermissionsId).HasColumnName("permissions_id");

                entity.HasOne(d => d.HashAccountNavigation)
                    .WithMany(p => p.ManagerAccounts)
                    .HasForeignKey(d => d.HashAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_manager_account_employee");

                entity.HasOne(d => d.Permissions)
                    .WithMany(p => p.ManagerAccounts)
                    .HasForeignKey(d => d.PermissionsId)
                    .HasConstraintName("FK_manager_account_manager_permissions");
            });

            modelBuilder.Entity<ManagerPermission>(entity =>
            {
                entity.HasKey(e => e.PermissionsId);

                entity.ToTable("manager_permissions");

                entity.Property(e => e.PermissionsId).HasColumnName("permissions_id");

                entity.Property(e => e.CompanyHash)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("company_hash");

                entity.Property(e => e.EmployeeDisplay).HasColumnName("employee_display");

                entity.Property(e => e.EmployeeReview).HasColumnName("employee_review");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.SettingDepartmentJobtitle).HasColumnName("setting_department_jobtitle");

                entity.Property(e => e.SettingLocation).HasColumnName("setting_location");

                entity.Property(e => e.SettingWorktime).HasColumnName("setting_worktime");

                entity.HasOne(d => d.CompanyHashNavigation)
                    .WithMany(p => p.ManagerPermissions)
                    .HasForeignKey(d => d.CompanyHash)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_manager_permissions_company");

                entity.HasOne(d => d.EmployeeDisplayNavigation)
                    .WithMany(p => p.ManagerPermissionEmployeeDisplayNavigations)
                    .HasForeignKey(d => d.EmployeeDisplay)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_manager_permissions_manager_permissions_type");

                entity.HasOne(d => d.EmployeeReviewNavigation)
                    .WithMany(p => p.ManagerPermissionEmployeeReviewNavigations)
                    .HasForeignKey(d => d.EmployeeReview)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_manager_permissions_manager_permissions_type1");
            });

            modelBuilder.Entity<ManagerPermissionsCustomization>(entity =>
            {
                entity.HasKey(e => e.CustomizationId)
                    .HasName("PK_manager_customization");

                entity.ToTable("manager_permissions_customization");

                entity.Property(e => e.CustomizationId).HasColumnName("customization_id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.JobtitleId).HasColumnName("jobtitle_id");

                entity.Property(e => e.PermissionsId).HasColumnName("permissions_id");

                entity.HasOne(d => d.Permissions)
                    .WithMany(p => p.ManagerPermissionsCustomizations)
                    .HasForeignKey(d => d.PermissionsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_manager_permissions_customization_manager_permissions");
            });

            modelBuilder.Entity<ManagerPermissionsType>(entity =>
            {
                entity.HasKey(e => e.PermissionsTypeId);

                entity.ToTable("manager_permissions_type");

                entity.Property(e => e.PermissionsTypeId).HasColumnName("permissions_type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
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
