using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Migrations;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data.Entity.Migrations.History;
using System.Text;
using System.Web;
using Pablo.Gallery.Logic.Interceptors;
using Pablo.Gallery.Logic.Interfaces;

namespace Pablo.Gallery.Models
{
	sealed class Configuration : DbConfiguration
	{
		public Configuration()
		{
			// general configuration
		}
	}

	public class GalleryContext : DbContext
	{
		public const string Schema = "gallery";

		public GalleryContext()
			: base("Gallery")
		{
			DbInterception.Add(new FullTextSearchInterceptor());
			//var entity = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace)
			//	.BaseEntitySets.First(r => r.Name == "File");
		}

		public ObjectContext ObjectContext { get { return ((IObjectContextAdapter)this).ObjectContext; } }

		public string EscapeContains(string search)
		{
			// hack, npgsql doesn't escape strings for contains search
			return IsPostgres ? search.Replace(@"\", @"\\") : search;
		}

		public bool IsPostgres
		{
			get { return Database.Connection is Npgsql.NpgsqlConnection; }
		}

		public bool IsSqlServer
		{
			get { return Database.Connection is System.Data.SqlClient.SqlConnection; }
		}

		public DbSet<Pack> Packs { get; set; }

		public DbSet<File> Files { get; set; }
		public DbSet<Artist> Artists { get; set; }
		public DbSet<FileArtist> FileArtists { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<FileTag> FileTags { get; set; }
		public DbSet<FileContent> FileContents { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<UserOAuthMembership> UserOAuthMemberships { get; set; }

		public override int SaveChanges() {
			//Do soft deletes
			foreach (var deletableEntity in ChangeTracker.Entries<IDeletable>()) {
				if (deletableEntity.State == EntityState.Deleted) {
					//Deleted - set the deleted flag
					deletableEntity.State = EntityState.Unchanged; //We need to set this to unchanged here, because setting it to modified seems to set ALL of its fields to modified
					deletableEntity.Entity.IsDeleted = true; //This will set the entity's state to modified for the next time we query the ChangeTracker
				}
			}

			//Do concurrency
			foreach (var versionableEntity in ChangeTracker.Entries<IVersionable>()) {
				switch (versionableEntity.State) {
					case EntityState.Added:
						//Added - Start with version 1
						versionableEntity.Entity.Version = 1;
						break;
					case EntityState.Modified:
						//Modified (or deleted from above) - Increment the version number
						versionableEntity.Entity.Version++;
						break;
				}
			}

			//Do audit trails
			int currentApplicationUserId = HttpContext.Current != null && HttpContext.Current.User != null &&
			                               HttpContext.Current.User.CurrentUser() != null
				? HttpContext.Current.User.CurrentUser().Id : -1;
			DateTime currentDateTime = DateTime.Now;
			foreach (var auditableEntity in ChangeTracker.Entries<IAuditable>()) {
				if (currentApplicationUserId < 0)
					throw new DbEntityValidationException(string.Format("Attempt to save audited entity without known user"));
				if (auditableEntity.State == EntityState.Added || auditableEntity.State == EntityState.Modified) {
					//Adding or modifying - update the edited audit trails
					auditableEntity.Entity.EditedDateTime = currentDateTime;
					auditableEntity.Entity.EditedById = currentApplicationUserId;

					switch (auditableEntity.State) {
						case EntityState.Added:
							//Adding - set the created audit trails
							auditableEntity.Entity.CreatedDateTime = currentDateTime;
							auditableEntity.Entity.CreatedById = currentApplicationUserId;
							break;
						case EntityState.Modified:
							//Modified (or deleted from above) - ensure that the created fields are not being modified
							if (auditableEntity.Property(p => p.CreatedDateTime).IsModified || auditableEntity.Property(p => p.CreatedById).IsModified) {
								throw new DbEntityValidationException(string.Format("Attempt to change created audit trails on a modified {0}", auditableEntity.Entity.GetType().FullName));
							}
							break;
					}
				}
			}
			return base.SaveChanges();
		}
		public IQueryable<File> QueryFiles(string format = null, string type = null, string query = null)
		{
			return from f in Files
				where
				(format == null || f.Format.ToLower() == format.ToLower())
				&& (type == null || f.Type.ToLower() == type.ToLower())
				&& (query == null || f.FileName.ToLower().Contains(query.ToLower()))
				orderby f.Pack.Date descending, f.FileName
				select f;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Pack>().
				HasMany(c => c.Categories).
				WithMany(p => p.Packs).
				Map(m =>
			{
				m.MapLeftKey("Pack_Id");
				m.MapRightKey("Category_Id");
				m.ToTable("Pack_Category", Schema);
			});

			modelBuilder.Entity<User>().
				HasMany(c => c.Roles).
				WithMany(p => p.Users).
				Map(m =>
			{
				m.MapLeftKey("User_Id");
				m.MapRightKey("Role_Id");
				m.ToTable("User_Role", Schema);
			});

			modelBuilder.Entity<FileArtist>()
				.HasKey(fa => new {fa.FileId, fa.ArtistId});

			modelBuilder.Entity<File>()
				.HasMany(f => f.Artists)
				.WithRequired()
				.HasForeignKey(fa => fa.FileId);

			modelBuilder.Entity<Artist>()
				.HasMany(a => a.Files)
				.WithRequired()
				.HasForeignKey(fa => fa.ArtistId);

			modelBuilder.Entity<FileTag>()
				.HasKey(ft => new { ft.FileId, ft.TagId });

			modelBuilder.Entity<File>()
				.HasMany(f => f.Tags)
				.WithRequired()
				.HasForeignKey(ft => ft.FileId);

			modelBuilder.Entity<Tag>()
				.HasMany(t => t.Files)
				.WithRequired()
				.HasForeignKey(ft => ft.TagId);

			//modelBuilder.Entity<File>().
			//	HasMany(c => c.Artists).
			//	WithMany(f => f.Files).
			//	Map(m =>
			//{
			//	m.MapLeftKey("File_Id");
			//	m.MapRightKey("Artist_Id");
			//	m.ToTable("File_Artist", Schema);
			//});

			//modelBuilder.Entity<File>().
			//	HasMany(c => c.Tags).
			//	WithMany(p => p.Files).
			//	Map(m =>
			//{
			//	m.MapLeftKey("File_Id");
			//	m.MapRightKey("Tag_Id");
			//	m.ToTable("File_Tag", Schema);
			//});

		}
	}
}