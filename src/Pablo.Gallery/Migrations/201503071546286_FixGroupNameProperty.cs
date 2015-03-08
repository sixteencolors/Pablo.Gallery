namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixGroupNameProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("gallery.Group", "Name", c => c.String(maxLength: 256, fixedLength: true));
            AddColumn("gallery.Group", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            CreateIndex("gallery.Group", "Name", unique: true, name: "ux_Group_Name");
            CreateIndex("gallery.Group", "Slug", unique: true, name: "ux_Alias_Slug");
        }
        
        public override void Down()
        {
            DropIndex("gallery.Group", "ux_Alias_Slug");
            DropIndex("gallery.Group", "ux_Group_Name");
            DropColumn("gallery.Group", "Slug");
            DropColumn("gallery.Group", "Name");
        }
    }
}
