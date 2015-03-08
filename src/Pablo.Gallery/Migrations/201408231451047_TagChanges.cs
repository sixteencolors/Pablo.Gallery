namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("gallery.Tag", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            CreateIndex("gallery.Tag", "Slug", unique: true, name: "ux_Tag_Slug");
        }
        
        public override void Down()
        {
            DropIndex("gallery.Tag", "ux_Tag_Slug");
            DropColumn("gallery.Tag", "Slug");
        }
    }
}
