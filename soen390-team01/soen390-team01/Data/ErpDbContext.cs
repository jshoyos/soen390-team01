using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using soen390_team01.Data.Entities;

#nullable disable

namespace soen390_team01.Data
{
    public partial class ErpDbContext : DbContext
    {
        public ErpDbContext()
        {
        }

        public ErpDbContext(DbContextOptions<ErpDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Bike> Bikes { get; set; }
        public virtual DbSet<BikePart> BikeParts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PartMaterial> PartMaterials { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Procurement> Procurements { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<Bike>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("bike_pkey");

                entity.ToTable("bike");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasDefaultValueSql("nextval('bike_id_seq'::regclass)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("grade");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(4)
                    .HasColumnName("size");
            });

            modelBuilder.Entity<BikePart>(entity =>
            {
                entity.HasKey(e => new { e.BikeId, e.PartId })
                    .HasName("bike_part_pkey");

                entity.ToTable("bike_part");

                entity.Property(e => e.BikeId).HasColumnName("bike_id");

                entity.Property(e => e.PartId).HasColumnName("part_id");

                entity.Property(e => e.PartQuantity).HasColumnName("part_quantity");

                entity.HasOne(d => d.Bike)
                    .WithMany(p => p.BikeParts)
                    .HasForeignKey(d => d.BikeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bike_part_bike_id_fkey");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.BikeParts)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bike_part_part_id_fkey");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("address");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventory");

                entity.HasIndex(e => new { e.ItemId, e.Type }, "inventory_item_id_type_key")
                    .IsUnique();

                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("type");

                entity.Property(e => e.Warehouse)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("warehouse");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("material_pkey");

                entity.ToTable("material");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasDefaultValueSql("nextval('material_id_seq'::regclass)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("grade");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("state");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_customer_id_fkey");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_payment_id_fkey");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.OrderId, e.ItemId })
                    .HasName("order_item_pkey");

                entity.ToTable("order_item");

                entity.Property(e => e.Type)
                    .HasMaxLength(8)
                    .HasColumnName("type");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.ItemQuantity).HasColumnName("item_quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_item_order_id_fkey");
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("part_pkey");

                entity.ToTable("part");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasDefaultValueSql("nextval('part_id_seq'::regclass)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("grade");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(4)
                    .HasColumnName("size");
            });

            modelBuilder.Entity<PartMaterial>(entity =>
            {
                entity.HasKey(e => new { e.PartId, e.MaterialId })
                    .HasName("part_material_pkey");

                entity.ToTable("part_material");

                entity.Property(e => e.PartId).HasColumnName("part_id");

                entity.Property(e => e.MaterialId).HasColumnName("material_id");

                entity.Property(e => e.MaterialQuantity).HasColumnName("material_quantity");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.PartMaterials)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("part_material_material_id_fkey");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.PartMaterials)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("part_material_part_id_fkey");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<Procurement>(entity =>
            {
                entity.ToTable("procurement");

                entity.Property(e => e.ProcurementId).HasColumnName("procurement_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.ItemQuantity).HasColumnName("item_quantity");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("state");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("type");

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Procurements)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("procurement_payment_id_fkey");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Procurements)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("procurement_vendor_id_fkey");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("vendor");

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("address");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnName("user_id")
                    .HasDefaultValueSql("nextval('user_user_id_seq'::regclass)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("first_name");

                entity.Property(e => e.Iv)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("iv");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("last_name");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("phone_number");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("user_role");
            });

            modelBuilder.HasSequence("bike_id_seq");

            modelBuilder.HasSequence("customer_customer_id_seq");

            modelBuilder.HasSequence("material_id_seq");

            modelBuilder.HasSequence("order_order_id_seq");

            modelBuilder.HasSequence("part_id_seq");

            modelBuilder.HasSequence("payment_payment_id_seq");

            modelBuilder.HasSequence("procurement_procurement_id_seq");

            modelBuilder.HasSequence("vendor_vendor_id_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
