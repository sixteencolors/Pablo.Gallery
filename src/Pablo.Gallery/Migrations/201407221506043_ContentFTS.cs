namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentFTS : DbMigration
    {
		public override void Up() {
			//this.AddFullTextIndex("gallery.File_Content", "fti_File_Content_Text", "Text", "english");
		}

		public override void Down() {
			// TODO: Write Down()
		}
    }
}
