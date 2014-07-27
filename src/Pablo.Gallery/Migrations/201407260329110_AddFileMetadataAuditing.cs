namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileMetadataAuditing : DbMigration
    {
        public override void Up()
        {
			
			DropForeignKey("gallery.File_Artist", "File_Id", "gallery.File");
            DropForeignKey("gallery.File_Artist", "Artist_Id", "gallery.Artist");
            DropForeignKey("gallery.File_Tag", "File_Id", "gallery.File");
            DropForeignKey("gallery.File_Tag", "Tag_Id", "gallery.Tag");
            DropIndex("gallery.File_Artist", new[] { "File_Id" });
            DropIndex("gallery.File_Artist", new[] { "Artist_Id" });
            DropIndex("gallery.File_Tag", new[] { "File_Id" });
            DropIndex("gallery.File_Tag", new[] { "Tag_Id" });
			DropTable("gallery.File_Artist");
			DropTable("gallery.File_Tag");
			CreateTable(
                "gallery.File_Artist",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        ArtistId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        CreatedById = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        EditedById = c.Int(nullable: false),
                        EditedDateTime = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.ArtistId })
                .ForeignKey("gallery.Artist", t => t.ArtistId, cascadeDelete: true)
                .ForeignKey("gallery.File", t => t.FileId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.ArtistId);
            
            CreateTable(
                "gallery.File_Tag",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        CreatedById = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        EditedById = c.Int(nullable: false),
                        EditedDateTime = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.TagId })
                .ForeignKey("gallery.File", t => t.FileId, cascadeDelete: true)
                .ForeignKey("gallery.Tag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            
            DropForeignKey("gallery.File_Tag", "TagId", "gallery.Tag");
            DropForeignKey("gallery.File_Tag", "FileId", "gallery.File");
            DropForeignKey("gallery.File_Artist", "FileId", "gallery.File");
            DropForeignKey("gallery.File_Artist", "ArtistId", "gallery.Artist");
            DropIndex("gallery.File_Tag", new[] { "TagId" });
            DropIndex("gallery.File_Tag", new[] { "FileId" });
            DropIndex("gallery.File_Artist", new[] { "ArtistId" });
            DropIndex("gallery.File_Artist", new[] { "FileId" });
            DropTable("gallery.File_Tag");
            DropTable("gallery.File_Artist");
			CreateTable(
				"gallery.File_Tag",
				c => new {
					File_Id = c.Int(nullable: false),
					Tag_Id = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.File_Id, t.Tag_Id });

			CreateTable(
				"gallery.File_Artist",
				c => new {
					File_Id = c.Int(nullable: false),
					Artist_Id = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.File_Id, t.Artist_Id });
			CreateIndex("gallery.File_Tag", "Tag_Id");
            CreateIndex("gallery.File_Tag", "File_Id");
            CreateIndex("gallery.File_Artist", "Artist_Id");
            CreateIndex("gallery.File_Artist", "File_Id");
            AddForeignKey("gallery.File_Tag", "Tag_Id", "gallery.Tag", "Id", cascadeDelete: true);
            AddForeignKey("gallery.File_Tag", "File_Id", "gallery.File", "Id", cascadeDelete: true);
            AddForeignKey("gallery.File_Artist", "Artist_Id", "gallery.Artist", "Id", cascadeDelete: true);
            AddForeignKey("gallery.File_Artist", "File_Id", "gallery.File", "Id", cascadeDelete: true);
		
		}
    }
}
