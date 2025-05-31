using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Context;

public class OuroborosContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
	public OuroborosContext(DbContextOptions<OuroborosContext> options) : base(options) { }

	public DbSet<Cart> Carts { get; set; }
	public DbSet<CartItem> CartItems { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Charm> Charms { get; set; }
	public DbSet<Collection> Collections { get; set; }
	public DbSet<CustomBracelet> CustomBracelets { get; set; }
	public DbSet<CustomBraceletCharm> CustomBraceletCharms { get; set; }
	public DbSet<Design> Designs { get; set; }
	public DbSet<DesignImage> DesignImages { get; set; }
	public DbSet<Model> Models { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<Payment> Payments { get; set; }
	public DbSet<Topic> Topics { get; set; }
	public DbSet<SystemTracking> SystemTrackings { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }


    // Cấu hình mối quan hệ giữa các bảng trong phương thức OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Cấu hình khóa chính cho các bảng cần thiết
		modelBuilder.Entity<ApplicationUser>(entity =>
		{
			entity.HasKey(u => u.Id);  // Khóa chính là Id của ApplicationUser
		});

		modelBuilder.Entity<Cart>(entity =>
		{
			entity.HasKey(c => c.CartId);
			entity.HasOne(c => c.User)
				  .WithOne(u => u.Cart)
				  .HasForeignKey<Cart>(c => c.UserId);
		});

		modelBuilder.Entity<CartItem>(entity =>
		{
			entity.HasKey(ci => ci.CartItemId);
			entity.HasOne(ci => ci.Cart)
				  .WithMany(c => c.CartItems)
				  .HasForeignKey(ci => ci.CartId);
			entity.HasOne(ci => ci.CustomBracelet)
				  .WithMany(cb => cb.CartItems)
				  .HasForeignKey(ci => ci.CustomBraceletId);
			entity.HasOne(ci => ci.Design)
				  .WithMany(d => d.CartItems)
				  .HasForeignKey(ci => ci.DesignId);
		});

		modelBuilder.Entity<Category>(entity =>
		{
			entity.HasKey(c => c.CategoryId);
		});

		modelBuilder.Entity<Charm>(entity =>
		{
			entity.HasKey(ch => ch.CharmId);
		});

		modelBuilder.Entity<Collection>(entity =>
		{
			entity.HasKey(c => c.CollectionId);
		});

		modelBuilder.Entity<CustomBracelet>(entity =>
		{
			entity.HasKey(cb => cb.CustomBraceletId);
			entity.HasOne(cb => cb.User)
				  .WithMany(u => u.CustomBracelets)
				  .HasForeignKey(cb => cb.UserId);
			entity.HasOne(cb => cb.Category)
				  .WithMany(c => c.CustomBracelets)
				  .HasForeignKey(cb => cb.CategoryId);
		});

		modelBuilder.Entity<CustomBraceletCharm>(entity =>
		{
			entity.HasKey(cbch => new { cbch.CustomBraceletId, cbch.CharmId });
			entity.HasOne(cbch => cbch.CustomBracelet)
				  .WithMany(cb => cb.CustomBraceletCharms)
				  .HasForeignKey(cbch => cbch.CustomBraceletId);
			entity.HasOne(cbch => cbch.Charm)
				  .WithMany(ch => ch.CustomBraceletCharms)
				  .HasForeignKey(cbch => cbch.CharmId);
		});

		modelBuilder.Entity<Design>(entity =>
		{
			entity.HasKey(d => d.DesignId);
			entity.HasOne(d => d.Category)
				  .WithMany(c => c.Designs)
				  .HasForeignKey(d => d.CategoryId);
			entity.HasOne(d => d.Model)
				  .WithMany(m => m.Designs)
				  .HasForeignKey(d => d.ModelId);
		});

		modelBuilder.Entity<DesignImage>(entity =>
		{
			entity.HasKey(di => di.ImageId);
			entity.HasOne(di => di.Design)
				  .WithMany(d => d.DesignImages)
				  .HasForeignKey(di => di.DesignId);
		});

		modelBuilder.Entity<Model>(entity =>
		{
			entity.HasKey(m => m.ModelId);
			entity.HasOne(m => m.Topic)
				  .WithMany(t => t.Models)
				  .HasForeignKey(m => m.TopicId);
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.HasKey(o => o.OrderId);
			entity.HasOne(o => o.User)
				  .WithMany(u => u.Orders)
				  .HasForeignKey(o => o.UserId);
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.HasKey(oi => oi.OrderItemId);
			entity.HasOne(oi => oi.Order)
				  .WithMany(o => o.OrderItems)
				  .HasForeignKey(oi => oi.OrderId);
			entity.HasOne(oi => oi.CustomBracelet)
				  .WithMany(cb => cb.OrderItems)
				  .HasForeignKey(oi => oi.CustomBraceletId);
			entity.HasOne(oi => oi.Design)
				  .WithMany(d => d.OrderItems)
				  .HasForeignKey(oi => oi.DesignId);
		});

		modelBuilder.Entity<Payment>(entity =>
		{
			entity.HasKey(p => p.PaymentId);
			entity.HasOne(p => p.Order)
				  .WithMany(o => o.Payments)
				  .HasForeignKey(p => p.OrderId);
		});

		modelBuilder.Entity<Topic>(entity =>
		{
			entity.HasKey(t => t.TopicId);
			entity.HasOne(t => t.Collection)
				  .WithMany(c => c.Topics)
				  .HasForeignKey(t => t.CollectionId);
		});

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(cm => cm.MessageId);
        });
    }
}
