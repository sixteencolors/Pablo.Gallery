namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveErrantRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("gallery.Group", "Artist_Id", "gallery.Artist");
            DropIndex("gallery.Group", new[] { "Artist_Id" });
            DropColumn("gallery.Group", "Artist_Id");
        }
        
        public override void Down()
        {
            AddColumn("gallery.Group", "Artist_Id", c => c.Int());
            CreateIndex("gallery.Group", "Artist_Id");
            AddForeignKey("gallery.Group", "Artist_Id", "gallery.Artist", "Id");
        }
    }
}
