namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArtistSlug : DbMigration
    {
        public override void Up()
        {
            AddColumn("gallery.Artist", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            CreateIndex("gallery.Artist", "Slug", unique: true, name: "ux_Alias_Slug");
        }
        
        public override void Down()
        {
            DropIndex("gallery.Artist", "ux_Alias_Slug");
            DropColumn("gallery.Artist", "Slug");
        }
    }
}
