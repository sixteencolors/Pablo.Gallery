namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlias : DbMigration
    {
        public override void Up()
        {
            AddColumn("gallery.Artist", "Alias", c => c.String(maxLength: 256, fixedLength: true));
            CreateIndex("gallery.Artist", "Alias", unique: true, name: "ux_Alias_Name");
        }
        
        public override void Down()
        {
            DropIndex("gallery.Artist", "ux_Alias_Name");
            DropColumn("gallery.Artist", "Alias");
        }
    }
}
