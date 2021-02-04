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
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PartMaterial> PartMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<Bike>(entity =>
            {
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
                entity.HasKey(e => e.ItemId);
            });

            modelBuilder.Entity<BikePart>(entity =>
            {
                entity.ToTable("bike_part");

                entity.Property(e => e.BikePartId).HasColumnName("bike_part_id");

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
                entity.HasKey(e => e.ItemId);
            });

            modelBuilder.Entity<Part>(entity =>
            {
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
                entity.HasKey(e => e.ItemId);
            });

            modelBuilder.Entity<PartMaterial>(entity =>
            {
                entity.HasKey(e => e.PartId)
                    .HasName("part_material_pkey");

                entity.ToTable("part_material");

                entity.HasIndex(e => e.PartId, "unique_part_material")
                    .IsUnique();

                entity.Property(e => e.PartId)
                    .ValueGeneratedNever()
                    .HasColumnName("part_id");

                entity.Property(e => e.MaterialId).HasColumnName("material_id");

                entity.Property(e => e.MaterialQuantity).HasColumnName("material_quantity");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.PartMaterials)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("part_material_material_id_fkey");

                entity.HasOne(d => d.Part)
                    .WithOne(p => p.PartMaterial)
                    .HasForeignKey<PartMaterial>(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("part_material_part_id_fkey");
            });

            modelBuilder.HasSequence("bike_id_seq");

            modelBuilder.HasSequence("material_id_seq");

            modelBuilder.HasSequence("part_id_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
