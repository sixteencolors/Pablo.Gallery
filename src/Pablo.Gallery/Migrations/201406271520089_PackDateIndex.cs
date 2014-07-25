namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackDateIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("gallery.Pack", "Date", name: "Date");
        }
        
        public override void Down()
        {
            DropIndex("gallery.Pack", "Date");
        }
    }
}
