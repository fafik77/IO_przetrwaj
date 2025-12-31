using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Infrastucture.Context;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
	public DbSet<Category> Categories { get; set; }

	public DbSet<Attachment> Attachments { get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<Region> Regions { get; set; }
	public DbSet<UserComment> Comments { get; set; }
	public DbSet<Vote> Votes { get; set; }

	#region Views and TPH mappings
	public DbSet<CategoryResource> CategoryResources { get; set; }
	public DbSet<CategoryDanger> CategoryDangers { get; set; }

	/// <summary>
	/// Returns only Active Danger Posts
	/// </summary>
	public IQueryable<Post> PostsDangerRO => Posts.AsNoTracking().Where(p => p.Active == true && p.Category == CategoryType.Danger);
	/// <summary>
	/// Returns only Active Resource Posts
	/// </summary>
	public IQueryable<Post> PostsResourcesRO => Posts.AsNoTracking().Where(p => p.Active == true && p.Category == CategoryType.Resource);
	public DbSet<PostMinimalCategoryRegion> PostMinimalViews { get; set; }
	#endregion

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		// MUST call the base method first to configure Identity tables
		base.OnModelCreating(builder);
		builder.HasDefaultSchema("przetrwaj");

		// --- 1. Category Inheritance (TPH) Configuration ---
		builder.Entity<Category>()
			.HasDiscriminator<CategoryType>("Type")
			.HasValue<CategoryResource>(CategoryType.Resource)
			.HasValue<CategoryDanger>(CategoryType.Danger);

		builder.Entity<PostMinimalCategoryRegion>(entity =>
		{
			entity.HasNoKey(); // Views usually don't have a PK in EF context
			entity.ToView("View_PostMinimal", "przetrwaj");
			// Match the property names if they differ from SQL columns
			entity.Property(v => v.IdPost).HasColumnName("IdPost");
		});

		// --- 2. Vote Entity Configuration ---
		// Vote has a composite unique key (IdPost, IdUser) for the 'UniquePair'
		// The [Key] on IdVote means it's the primary key, but we need to ensure 
		// the (IdPost, IdUser) pair is unique.
		builder.Entity<Vote>()
			.HasKey(v => new { v.IdPost, v.IdUser }); //Key is unique and indexed

		// Relationship: Post (Principal) -> Vote (Dependent)
		builder.Entity<Vote>()
			.HasOne(v => v.IdPostNavigation)        // Vote has one Post
			.WithMany(p => p.Votes)                 // Post has many Votes
			.HasForeignKey(v => v.IdPost)           // Foreign Key is IdPost in Vote
													// If a Post is deleted, its Votes are deleted
			.OnDelete(DeleteBehavior.Cascade);

		// Relationship: AppUser (Principal) -> Vote (Dependent)
		builder.Entity<Vote>()
			.HasOne(v => v.IdUserNavigation)        // Vote has one AppUser
			.WithMany()                             // AppUser doesn't have a direct Votes collection
			.HasForeignKey(v => v.IdUser)           // Foreign Key is IdUser in Vote
													// Prevent deleting a User from automatically deleting their Votes
			.OnDelete(DeleteBehavior.Restrict);


		// --- 3. UserComment Entity Configuration ---

		// Relationship: Post (Principal) -> UserComment (Dependent)
		builder.Entity<UserComment>()
			.HasOne(c => c.IdPostNavigation)        // Comment has one Post
			.WithMany(p => p.Comments)              // Post has many Comments
			.HasForeignKey(c => c.IdPost)           // Foreign Key is IdPost in UserComment
			.OnDelete(DeleteBehavior.Cascade);      // If a Post is deleted, its Comments are deleted

		// Relationship: AppUser (Principal) -> UserComment (Dependent)
		builder.Entity<UserComment>()
			.HasOne(c => c.IdAutorNavigation)       // Comment has one AppUser (Autor)
			.WithMany(u => u.Comments)              // AppUser has many Comments
			.HasForeignKey(c => c.IdAutor)          // Foreign Key is IdAutor in UserComment
													// Prevent deleting a User from automatically deleting their Comments
			.OnDelete(DeleteBehavior.Restrict);


		// --- 4. Post Entity Configuration ---

		// Relationship: AppUser (Principal) -> Post (Dependent)
		builder.Entity<Post>()
			.HasOne(p => p.IdAutorNavigation)       // Post has one AppUser (Autor)
			.WithMany(u => u.Posts)                 // AppUser has many Posts
			.HasForeignKey(p => p.IdAutor)          // Foreign Key is IdAutor in Post
			.OnDelete(DeleteBehavior.Restrict);

		// Relationship: Category (Principal) -> Post (Dependent)
		builder.Entity<Post>()
			.HasOne(p => p.IdCategoryNavigation)    // Post has one Category
			.WithMany(c => c.Posts)                 // Category has many Posts
			.HasForeignKey(p => p.IdCategory)       // Foreign Key is IdCategory (int) in Post
			.OnDelete(DeleteBehavior.Restrict);

		// Relationship: Region (Principal) -> Post (Dependent)
		builder.Entity<Post>()
			.HasOne(p => p.IdRegionNavigation)      // Post has one Region
			.WithMany(r => r.Posts)                 // Region has many Posts
			.HasForeignKey(p => p.IdRegion)         // Foreign Key is IdRegion (int) in Post
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Post>()
			.Property(p => p.Category)
			.IsRequired();

		// Add a composite index for Category and Active status (used for: PostsDangerROm, PostsResourcesRO, Statistics)
		builder.Entity<Post>()
			.HasIndex(p => new { p.Category, p.Active })	// 2 * 2 = only 4 branching paths (Da, Dn, Ra, Rn)
			.HasDatabaseName("IX_Post_Category_Active");


		// --- 5. Attachment Entity Configuration ---

		// Relationship: Post (Principal) -> Attachment (Dependent)
		builder.Entity<Attachment>()
			.HasOne(a => a.IdPostNavigation)        // Attachment has one Post
			.WithMany(p => p.Attachments)           // Post has many Attachments
			.HasForeignKey(a => a.IdPost)           // Foreign Key is IdPost in Attachment
			.OnDelete(DeleteBehavior.Cascade);      // If a Post is deleted, its Attachments are deleted


		// --- 6. AppUser Entity Configuration ---

		// Relationship: Region (Principal) -> AppUser (Dependent)
		builder.Entity<AppUser>()
			.HasOne(u => u.IdRegionNavigation)      // AppUser has one Region
			.WithMany(r => r.Users)                 // Region has many Users
			.HasForeignKey(u => u.IdRegion)         // Foreign Key is IdRegion in AppUser
			.OnDelete(DeleteBehavior.Restrict);     // Prevent deleting a Region if users are linked
	}
}
