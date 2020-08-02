using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models
{
    public partial class bcareContext : DbContext
    {
        public bcareContext()
        {
        }

        public bcareContext(DbContextOptions<bcareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apointment> Apointment { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonEntityType> PersonEntityType { get; set; }
        public virtual DbSet<PersonMedia> PersonMedia { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }

        public virtual DbSet<ApointmentUpdate> ApointmentUpdate { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=DESKTOP-DFP17VJ;user=sa;password=demo123123;Database=bcare;Trusted_Connection=True;MultipleActiveResultSets=true;");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apointment>(entity =>
            {
                entity.HasKey(e => e.PkApointmentId);

                entity.ToTable("Apointment", "Trans");

                entity.Property(e => e.PkApointmentId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ApointmentDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DoctorsNotes).IsUnicode(false);

                entity.Property(e => e.PatientSymptoms)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.FkApprover)
                    .WithMany(p => p.ApointmentFkApprover)
                    .HasForeignKey(d => d.FkApproverId)
                    .HasConstraintName("FK_Apointment_Approver");

                entity.HasOne(d => d.FkCloser)
                    .WithMany(p => p.ApointmentFkCloser)
                    .HasForeignKey(d => d.FkCloserId)
                    .HasConstraintName("FK_Apointment_Closer");

                entity.HasOne(d => d.FkCreator)
                    .WithMany(p => p.ApointmentFkCreator)
                    .HasForeignKey(d => d.FkCreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Apointment_Creator");

                entity.HasOne(d => d.FkPatient)
                    .WithMany(p => p.ApointmentFkPatient)
                    .HasForeignKey(d => d.FkPatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Apointment_Patient");
            });

            modelBuilder.Entity<Diagnosis>(entity =>
            {
                entity.HasKey(e => e.PkDiagnosis);

                entity.ToTable("Diagnosis", "Trans");

                entity.Property(e => e.PkDiagnosis).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.FkApointment)
                    .WithMany(p => p.Diagnosis)
                    .HasForeignKey(d => d.FkApointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Diagnosis_Apointment");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.PkPersonId);

                entity.ToTable("Person", "Person");

                entity.Property(e => e.PkPersonId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.ContactNo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DoB).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsUnicode(false);

                entity.HasOne(d => d.FkPersonEntityType)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.FkPersonEntityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_PersonEntityType");
            });

            modelBuilder.Entity<PersonEntityType>(entity =>
            {
                entity.HasKey(e => e.PkPersonEntityTypeId);

                entity.ToTable("PersonEntityType", "Person");

                entity.Property(e => e.PkPersonEntityTypeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Flag)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PersonMedia>(entity =>
            {
                entity.HasKey(e => e.PkPersonMediaId);

                entity.ToTable("PersonMedia", "Person");

                entity.Property(e => e.PkPersonMediaId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.PkUserId);

                entity.ToTable("User", "Organization");

                entity.Property(e => e.PkUserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkPerson)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.FkPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Person");

                entity.HasOne(d => d.FkUserType)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.FkUserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserType");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.PkUserTypeId);

                entity.ToTable("UserType", "Organization");

                entity.Property(e => e.PkUserTypeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Flag)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ApointmentUpdate>(entity =>
            {
                entity.HasKey(e => e.PkApointmentUpdateId);

                entity.ToTable("ApointmentUpdate", "Trans");

                entity.Property(e => e.PkApointmentUpdateId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.UpdateNote)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.FkApointment)
                    .WithMany(p => p.ApointmentUpdates)
                    .HasForeignKey(d => d.FkApointmentId)                    
                    .HasConstraintName("FK_ApointmentUpdate_Apointment");
            });
        }
    }
}
