namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetadata : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "gallery.Artist",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "gallery.Group",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Artist_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gallery.Artist", t => t.Artist_Id)
                .Index(t => t.Artist_Id);
            
            CreateTable(
                "gallery.Tag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256, fixedLength: true),
                        Pack_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gallery.Pack", t => t.Pack_Id)
                .Index(t => t.Name, unique: true, name: "ux_Tag_Name")
                .Index(t => t.Pack_Id);
            
            CreateTable(
                "gallery.File_Artist",
                c => new
                    {
                        File_Id = c.Int(nullable: false),
                        Artist_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Artist_Id })
                .ForeignKey("gallery.File", t => t.File_Id, cascadeDelete: true)
                .ForeignKey("gallery.Artist", t => t.Artist_Id, cascadeDelete: true)
                .Index(t => t.File_Id)
                .Index(t => t.Artist_Id);
            
            CreateTable(
                "gallery.File_Tag",
                c => new
                    {
                        File_Id = c.Int(nullable: false),
                        Tag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Tag_Id })
                .ForeignKey("gallery.File", t => t.File_Id, cascadeDelete: true)
                .ForeignKey("gallery.Tag", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.File_Id)
                .Index(t => t.Tag_Id);
            
            AddColumn("gallery.Pack", "Group_Id", c => c.Int());
            AddColumn("gallery.File", "Title", c => c.String(maxLength: 1073741823, fixedLength: true));
            CreateIndex("gallery.Pack", "Group_Id");
            AddForeignKey("gallery.Pack", "Group_Id", "gallery.Group", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("gallery.Tag", "Pack_Id", "gallery.Pack");
            DropForeignKey("gallery.Pack", "Group_Id", "gallery.Group");
            DropForeignKey("gallery.File_Tag", "Tag_Id", "gallery.Tag");
            DropForeignKey("gallery.File_Tag", "File_Id", "gallery.File");
            DropForeignKey("gallery.File_Artist", "Artist_Id", "gallery.Artist");
            DropForeignKey("gallery.File_Artist", "File_Id", "gallery.File");
            DropForeignKey("gallery.Group", "Artist_Id", "gallery.Artist");
            DropIndex("gallery.File_Tag", new[] { "Tag_Id" });
            DropIndex("gallery.File_Tag", new[] { "File_Id" });
            DropIndex("gallery.File_Artist", new[] { "Artist_Id" });
            DropIndex("gallery.File_Artist", new[] { "File_Id" });
            DropIndex("gallery.Tag", new[] { "Pack_Id" });
            DropIndex("gallery.Tag", "ux_Tag_Name");
            DropIndex("gallery.Group", new[] { "Artist_Id" });
            DropIndex("gallery.Pack", new[] { "Group_Id" });
            DropColumn("gallery.File", "Title");
            DropColumn("gallery.Pack", "Group_Id");
            DropTable("gallery.File_Tag");
            DropTable("gallery.File_Artist");
            DropTable("gallery.Tag");
            DropTable("gallery.Group");
            DropTable("gallery.Artist");
        }
    }
}
