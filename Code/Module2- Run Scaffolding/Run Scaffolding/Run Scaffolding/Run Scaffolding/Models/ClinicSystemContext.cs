using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Run_Scaffolding.Models;

public partial class ClinicSystemContext : DbContext
{
    public ClinicSystemContext()
    {
    }

    public ClinicSystemContext(DbContextOptions<ClinicSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentType> AppointmentTypes { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<DaysOfWeek> DaysOfWeeks { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorWorkingDay> DoctorWorkingDays { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionItem> PrescriptionItems { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    public virtual DbSet<VisitService> VisitServices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=ClinicSystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("smalldatetime");
            entity.Property(e => e.AppointmentFees).HasColumnType("smallmoney");
            entity.Property(e => e.AppointmentTypeId)
                .HasComment("")
                .HasColumnName("AppointmentTypeID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Status).HasComment("1-Scheduled 2-Cancelled 3-Completed");

            entity.HasOne(d => d.AppointmentType).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AppointmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_AppointmentType");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Users");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Patients");
        });

        modelBuilder.Entity<AppointmentType>(entity =>
        {
            entity.ToTable("AppointmentType");

            entity.Property(e => e.AppointmentTypeId)
                .ValueGeneratedNever()
                .HasColumnName("AppointmentTypeID");
            entity.Property(e => e.AppointmentTypeFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AppointmentTypeName).HasMaxLength(20);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasIndex(e => e.VisitId, "UQ_Bills_VisitID").IsUnique();

            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.Discount).HasColumnType("smallmoney");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasComment("1-Cash 2-CreditCard 3-Insurance");
            entity.Property(e => e.PaymentStatus).HasComment("1-Paid 2-Unpaid");
            entity.Property(e => e.TaxAmount).HasColumnType("smallmoney");
            entity.Property(e => e.TotalAmount).HasColumnType("smallmoney");
            entity.Property(e => e.VisitId).HasColumnName("VisitID");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Bills)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bills_Users");

            entity.HasOne(d => d.Visit).WithOne(p => p.Bill)
                .HasForeignKey<Bill>(d => d.VisitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bills_Visits");
        });

        modelBuilder.Entity<DaysOfWeek>(entity =>
        {
            entity.HasKey(e => e.DayId).HasName("PK__DaysOfWe__BF3DD8255E9072E9");

            entity.ToTable("DaysOfWeek");

            entity.Property(e => e.DayId).HasColumnName("DayID");
            entity.Property(e => e.DayName).HasMaxLength(20);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.ConsultationFees).HasColumnType("smallmoney");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Specialization).HasMaxLength(50);

            entity.HasOne(d => d.Person).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_People");
        });

        modelBuilder.Entity<DoctorWorkingDay>(entity =>
        {
            entity.HasKey(e => e.DoctorWorkingDayId).HasName("PK__DoctorWo__A90029396456392D");

            entity.HasIndex(e => new { e.DoctorId, e.DayId }, "UQ_Doctor_Day").IsUnique();

            entity.Property(e => e.DoctorWorkingDayId).HasColumnName("DoctorWorkingDayID");
            entity.Property(e => e.DayId).HasColumnName("DayID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

            entity.HasOne(d => d.Day).WithMany(p => p.DoctorWorkingDays)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorWorkingDays_Days");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorWorkingDays)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorWorkingDays_Doctors");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.MedicineName).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("smallmoney");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.BloodType).HasComment("0-Unknown 1-A+ 2-A- 3-B+ 4-B+ 5-AB+ 6-AB- 7-O+ 8-O- ");
            entity.Property(e => e.EmergencyContact).HasMaxLength(100);
            entity.Property(e => e.MedicalHistory).HasMaxLength(500);
            entity.Property(e => e.PersonId).HasColumnName("PersonID");

            entity.HasOne(d => d.Person).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_People");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.ImagePath).HasMaxLength(250);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasIndex(e => e.VisitId, "UQ_Prescriptions_VisitID").IsUnique();

            entity.HasIndex(e => e.VisitId, "UQ_VisitID").IsUnique();

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PrescriptionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VisitId).HasColumnName("VisitID");

            entity.HasOne(d => d.Visit).WithOne(p => p.Prescription)
                .HasForeignKey<Prescription>(d => d.VisitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescriptions_Visits");
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Prescrip__727E83EBE1DC05FE");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Dosage).HasMaxLength(255);
            entity.Property(e => e.Instructions).HasMaxLength(500);
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Medicine).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionItems_Medicines");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionItems_Prescriptions");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.ServiceDescription).HasMaxLength(500);
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.ServicePrice).HasColumnType("smallmoney");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.PersonId, "UQ_Users_PersonID").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Role).HasComment("0-Admin 1-Doctor 2-Receptionist");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Person).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_People");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasIndex(e => e.AppointmentId, "UQ_Visits_AppointmentID").IsUnique();

            entity.Property(e => e.VisitId).HasColumnName("VisitID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Diagnosis).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.VisitDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.Appointment).WithOne(p => p.Visit)
                .HasForeignKey<Visit>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visits_Appointments");
        });

        modelBuilder.Entity<VisitService>(entity =>
        {
            entity.Property(e => e.VisitServiceId).HasColumnName("VisitServiceID");
            entity.Property(e => e.ServiceFees).HasColumnType("smallmoney");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.VisitId).HasColumnName("VisitID");

            entity.HasOne(d => d.Service).WithMany(p => p.VisitServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitServices_Services");

            entity.HasOne(d => d.Visit).WithMany(p => p.VisitServices)
                .HasForeignKey(d => d.VisitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitServices_Visits");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
