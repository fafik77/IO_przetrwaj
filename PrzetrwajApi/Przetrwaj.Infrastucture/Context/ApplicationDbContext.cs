using Microsoft.AspNetCore.Identity;
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
	public DbSet<RegionWoj> RegionWoj { get; set; }
	public DbSet<RegionPow> RegionPow { get; set; }
	public DbSet<RegionGmi> RegionGmi { get; set; }
	public DbSet<UserComment> Comments { get; set; }
	public DbSet<Vote> Votes { get; set; }
	public DbSet<Impediment> Impediments { get; set; }
	public DbSet<UserJwtRefresh> UserJwtRefresh { get; set; }

	#region Views and TPH mappings
	/// <summary>
	/// Returns only Active Danger Posts
	/// </summary>
	public IQueryable<Post> PostsDangerRO => Posts.AsNoTracking().Where(p => p.Active == true && p.CategoryType == CategoryType.Danger);
	/// <summary>
	/// Returns only Active Resource Posts
	/// </summary>
	public IQueryable<Post> PostsResourcesRO => Posts.AsNoTracking().Where(p => p.Active == true && p.CategoryType == CategoryType.Resource);
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
		builder.Entity<UserJwtRefresh>().ToTable("UserJwtRefresh", "auth");

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


		// --- 2. UserComment Entity Configuration ---

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


		// --- 2. Region___ Entity Configuration ---

		// Relationship: Region_up (Principal) -> Region_down (Dependent)
		builder.Entity<RegionGmi>()
			.HasOne(p => p.Pow)                     // Gmi is in Pow
			.WithMany(c => c.Gminy)                 // Pow has many Gmi
			.HasForeignKey(p => p.PowId)            // Foreign Key is PowId (short)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<RegionPow>()
			.HasOne(p => p.Woj)                     // Pow is in Woj
			.WithMany(c => c.Powiaty)               // Woj has many Pow
			.HasForeignKey(p => p.WojId)            // Foreign Key is WojId (short)
			.OnDelete(DeleteBehavior.Restrict);

		// --- 2. Post Entity Configuration ---

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

		// Relationship: Region___ (Principal) -> Post (Dependent)
		builder.Entity<Post>()
			.HasOne(p => p.RegionWojNavigation)     // Post has one Region
			.WithMany(r => r.Posts)                 // Region has many Posts
			.HasForeignKey(p => p.IdWojOnly)        // Foreign Key is IdWojOnly (short?) in Post
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Post>()
			.HasOne(p => p.RegionPowNavigation)     // Post has one Region
			.WithMany(r => r.Posts)                 // Region has many Posts
			.HasForeignKey(p => p.IdPowOnly)        // Foreign Key is IdPowOnly (short?) in Post
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Post>()
			.HasOne(p => p.RegionGmiNavigation)     // Post has one Region
			.WithMany(r => r.Posts)                 // Region has many Posts
			.HasForeignKey(p => p.IdGmiOnly)        // Foreign Key is IdGmiOnly (int?) in Post
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Post>()
			.Property(p => p.CategoryType)
			.IsRequired();

		// Add a composite index for Category and Active status (used for: PostsDangerROm, PostsResourcesRO, Statistics)
		builder.Entity<Post>()
			.HasIndex(p => new { p.CategoryType, p.Active })    // 2 * 2 = only 4 branching paths (Da, Dn, Ra, Rn)
			.HasDatabaseName("IX_Post_Category_Active");


		// --- 2. Attachment Entity Configuration ---

		// Relationship: Post (Principal) -> Attachment (Dependent)
		builder.Entity<Attachment>()
			.HasOne(a => a.IdPostNavigation)        // Attachment has one Post
			.WithMany(p => p.Attachments)           // Post has many Attachments
			.HasForeignKey(a => a.IdPost)           // Foreign Key is IdPost in Attachment
			.OnDelete(DeleteBehavior.SetNull);      // If a Post is deleted, its Attachments are invalidated (to clean up)


		// --- 2. AppUser Entity Configuration ---

		// Relationship: Region___ (Principal) -> AppUser (Dependent)
		builder.Entity<AppUser>()
			.HasOne(u => u.RegionPowNavigation)     // AppUser has one Region
			.WithMany(r => r.Users)                 // Region has many Users
			.HasForeignKey(u => u.PowiatId)         // Foreign Key is PowiatId in AppUser
			.OnDelete(DeleteBehavior.SetNull);      // Prevent deleting a Region if users are linked

		builder.Entity<AppUser>()
			.HasOne(u => u.RegionGmiNavigation)     // AppUser has one Region
			.WithMany()
			.HasForeignKey(u => u.GminaId)          // Foreign Key is PowiatId in AppUser
			.OnDelete(DeleteBehavior.SetNull);      // Prevent deleting a Region if users are linked


		// --- 2. UserJwtRefresh Entity Configuration ---

		builder.Entity<UserJwtRefresh>()
			.HasKey(k => new { k.UserId, k.Jwi });

		// one AppUser has many UserJwtRefresh
		builder.Entity<UserJwtRefresh>().HasOne<AppUser>()  // UserJwtRefresh has one AppUser
			.WithMany()                                     // AppUser has many UserJwtRefresh
			.HasForeignKey(e => e.UserId)                   // FK is UserId in UserJwtRefresh
			.OnDelete(DeleteBehavior.Cascade);              // Delete all users tokens when the user is deleted



		// --- 3. Seed data ---
		builder.Entity<IdentityRole>().HasData(
			new IdentityRole
			{
				Id = "c395bc61-a75a-44ea-a8b6-d136bb4e032e",
				Name = "User",
				NormalizedName = "USER"
			},
			new IdentityRole
			{
				Id = "aabf5428-e94c-418a-939a-8004bd1fe63c",
				Name = "Moderator",
				NormalizedName = "MODERATOR"
			},
			new IdentityRole
			{
				Id = "8411b424-3e32-4ea3-8dbc-b5d786b62e40",
				Name = "Admin",
				NormalizedName = "ADMIN"
			}
		);

		builder.Entity<RegionWoj>().HasData(
			new RegionWoj { Id = 0, Name = "Polska" }
		);

	}
}
