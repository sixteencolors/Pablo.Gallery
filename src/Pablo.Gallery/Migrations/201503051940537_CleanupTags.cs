namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanupTags : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("gallery.Tag", "Pack_Id", "gallery.Pack");
            DropIndex("gallery.Tag", new[] { "Pack_Id" });
            DropColumn("gallery.Tag", "Pack_Id");
        }
        
        public override void Down()
        {
            AddColumn("gallery.Tag", "Pack_Id", c => c.Int());
            CreateIndex("gallery.Tag", "Pack_Id");
            AddForeignKey("gallery.Tag", "Pack_Id", "gallery.Pack", "Id");
        }
    }
}
