using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SmsGateway.Models
{
    public partial class SMSServerContext : DbContext
    {
        public virtual DbSet<MessageIn> MessageIn { get; set; }
        public virtual DbSet<MessageLog> MessageLog { get; set; }
        public virtual DbSet<MessageOut> MessageOut { get; set; }

        public SMSServerContext(DbContextOptions<SMSServerContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageIn>(entity =>
            {
                entity.Property(e => e.Gateway).HasMaxLength(80);

                entity.Property(e => e.MessageFrom).HasMaxLength(80);

                entity.Property(e => e.MessagePdu).HasColumnName("MessagePDU");

                entity.Property(e => e.MessageTo).HasMaxLength(80);

                entity.Property(e => e.MessageType).HasMaxLength(80);

                entity.Property(e => e.ReceiveTime).HasColumnType("datetime");

                entity.Property(e => e.SendTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Smsc)
                    .HasColumnName("SMSC")
                    .HasMaxLength(80);

                entity.Property(e => e.UserId).HasMaxLength(80);
            });

            modelBuilder.Entity<MessageLog>(entity =>
            {
                entity.HasIndex(e => new { e.MessageId, e.SendTime })
                    .HasName("IDX_MessageId");

                entity.Property(e => e.ErrorCode).HasMaxLength(80);

                entity.Property(e => e.ErrorText).HasMaxLength(80);

                entity.Property(e => e.Gateway).HasMaxLength(80);

                entity.Property(e => e.MessageFrom).HasMaxLength(80);

                entity.Property(e => e.MessageId).HasMaxLength(80);

                entity.Property(e => e.MessagePdu).HasColumnName("MessagePDU");

                entity.Property(e => e.MessageTo).HasMaxLength(80);

                entity.Property(e => e.MessageType).HasMaxLength(80);

                entity.Property(e => e.ReceiveTime).HasColumnType("datetime");

                entity.Property(e => e.SendTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StatusText).HasMaxLength(80);

                entity.Property(e => e.UserId).HasMaxLength(80);
            });

            modelBuilder.Entity<MessageOut>(entity =>
            {
                entity.HasIndex(e => e.IsRead)
                    .HasName("IDX_IsRead");

                entity.Property(e => e.Gateway).HasMaxLength(80);

                entity.Property(e => e.MessageFrom).HasMaxLength(80);

                entity.Property(e => e.MessageTo).HasMaxLength(80);

                entity.Property(e => e.MessageType).HasMaxLength(80);

                entity.Property(e => e.Scheduled).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(80);
            });
        }
    }
}
