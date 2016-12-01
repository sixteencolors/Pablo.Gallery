namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oddkeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("gallery.Artist", "ux_Alias_Name");
            DropIndex("gallery.Artist", "ux_Alias_Slug");
            DropIndex("gallery.Pack", "ux_Pack_Name");
            DropIndex("gallery.Pack", "Date");
            DropIndex("gallery.Group", "ux_Group_Name");
            DropIndex("gallery.Group", "ux_Alias_Slug");
            DropIndex("gallery.Tag", "ux_Tag_Name");
            DropIndex("gallery.Tag", "ux_Tag_Slug");
            DropIndex("gallery.User", "ux_User_UserName");
            DropPrimaryKey("gallery.User_OAuthMembership");
            AlterColumn("gallery.Artist", "Alias", c => c.String(maxLength: 256));
            AlterColumn("gallery.Artist", "Slug", c => c.String(maxLength: 256));
            AlterColumn("gallery.File_Artist", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.File_Artist", "EditedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.File", "Name", c => c.String());
            AlterColumn("gallery.File", "FileName", c => c.String());
            AlterColumn("gallery.File", "Format", c => c.String(maxLength: 20));
            AlterColumn("gallery.File", "Type", c => c.String(maxLength: 20));
            AlterColumn("gallery.File", "Title", c => c.String());
            AlterColumn("gallery.File_Content", "Text", c => c.String());
            AlterColumn("gallery.Pack", "Name", c => c.String(maxLength: 256));
            AlterColumn("gallery.Pack", "FileName", c => c.String());
            AlterColumn("gallery.Pack", "Date", c => c.DateTime());
            AlterColumn("gallery.Category", "Name", c => c.String(maxLength: 256));
            AlterColumn("gallery.Group", "Name", c => c.String(maxLength: 256));
            AlterColumn("gallery.Group", "Slug", c => c.String(maxLength: 256));
            AlterColumn("gallery.File_Tag", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.File_Tag", "EditedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.Tag", "Name", c => c.String(maxLength: 256));
            AlterColumn("gallery.Tag", "Slug", c => c.String(maxLength: 256));
            AlterColumn("gallery.Role", "Name", c => c.String(maxLength: 30));
            AlterColumn("gallery.User", "CreateDate", c => c.DateTime());
            AlterColumn("gallery.User", "LastLoginDate", c => c.DateTime());
            AlterColumn("gallery.User", "UserName", c => c.String(maxLength: 256));
            AlterColumn("gallery.User", "Email", c => c.String());
            AlterColumn("gallery.User", "Alias", c => c.String());
            AlterColumn("gallery.User", "ConfirmationToken", c => c.String(maxLength: 128));
            AlterColumn("gallery.User", "LastPasswordFailureDate", c => c.DateTime());
            AlterColumn("gallery.User", "Password", c => c.String(maxLength: 128));
            AlterColumn("gallery.User", "PasswordChangedDate", c => c.DateTime());
            AlterColumn("gallery.User", "PasswordSalt", c => c.String());
            AlterColumn("gallery.User", "PasswordVerificationToken", c => c.String(maxLength: 128));
            AlterColumn("gallery.User", "PasswordVerificationExpiryDate", c => c.DateTime());
            AlterColumn("gallery.User", "PasswordQuestion", c => c.String());
            AlterColumn("gallery.User", "PasswordAnswer", c => c.String());
            AlterColumn("gallery.User_OAuthMembership", "Provider", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("gallery.User_OAuthMembership", "ProviderUserId", c => c.String(nullable: false, maxLength: 100));
            AddPrimaryKey("gallery.User_OAuthMembership", new[] { "Provider", "ProviderUserId" });
            CreateIndex("gallery.Artist", "Alias", unique: true, name: "ux_Alias_Name");
            CreateIndex("gallery.Artist", "Slug", unique: true, name: "ux_Alias_Slug");
            CreateIndex("gallery.Pack", "Name", unique: true, name: "ux_Pack_Name");
            CreateIndex("gallery.Pack", "Date", name: "Date");
            CreateIndex("gallery.Group", "Name", unique: true, name: "ux_Group_Name");
            CreateIndex("gallery.Group", "Slug", unique: true, name: "ux_Alias_Slug");
            CreateIndex("gallery.Tag", "Name", unique: true, name: "ux_Tag_Name");
            CreateIndex("gallery.Tag", "Slug", unique: true, name: "ux_Tag_Slug");
            CreateIndex("gallery.User", "UserName", unique: true, name: "ux_User_UserName");
        }
        
        public override void Down()
        {
            DropIndex("gallery.User", "ux_User_UserName");
            DropIndex("gallery.Tag", "ux_Tag_Slug");
            DropIndex("gallery.Tag", "ux_Tag_Name");
            DropIndex("gallery.Group", "ux_Alias_Slug");
            DropIndex("gallery.Group", "ux_Group_Name");
            DropIndex("gallery.Pack", "Date");
            DropIndex("gallery.Pack", "ux_Pack_Name");
            DropIndex("gallery.Artist", "ux_Alias_Slug");
            DropIndex("gallery.Artist", "ux_Alias_Name");
            DropPrimaryKey("gallery.User_OAuthMembership");
            AlterColumn("gallery.User_OAuthMembership", "ProviderUserId", c => c.String(nullable: false, maxLength: 100, fixedLength: true));
            AlterColumn("gallery.User_OAuthMembership", "Provider", c => c.String(nullable: false, maxLength: 30, fixedLength: true));
            AlterColumn("gallery.User", "PasswordAnswer", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "PasswordQuestion", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "PasswordVerificationExpiryDate", c => c.DateTime());
            AlterColumn("gallery.User", "PasswordVerificationToken", c => c.String(maxLength: 128, fixedLength: true));
            AlterColumn("gallery.User", "PasswordSalt", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "PasswordChangedDate", c => c.DateTime());
            AlterColumn("gallery.User", "Password", c => c.String(maxLength: 128, fixedLength: true));
            AlterColumn("gallery.User", "LastPasswordFailureDate", c => c.DateTime());
            AlterColumn("gallery.User", "ConfirmationToken", c => c.String(maxLength: 128, fixedLength: true));
            AlterColumn("gallery.User", "Alias", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "Email", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "UserName", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.User", "LastLoginDate", c => c.DateTime());
            AlterColumn("gallery.User", "CreateDate", c => c.DateTime());
            AlterColumn("gallery.Role", "Name", c => c.String(maxLength: 30, fixedLength: true));
            AlterColumn("gallery.Tag", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.Tag", "Name", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.File_Tag", "EditedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.File_Tag", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.Group", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.Group", "Name", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.Category", "Name", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.Pack", "Date", c => c.DateTime());
            AlterColumn("gallery.Pack", "FileName", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.Pack", "Name", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.File_Content", "Text", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.File", "Title", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.File", "Type", c => c.String(maxLength: 20, fixedLength: true));
            AlterColumn("gallery.File", "Format", c => c.String(maxLength: 20, fixedLength: true));
            AlterColumn("gallery.File", "FileName", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.File", "Name", c => c.String(maxLength: 1073741823, fixedLength: true));
            AlterColumn("gallery.File_Artist", "EditedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.File_Artist", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("gallery.Artist", "Slug", c => c.String(maxLength: 256, fixedLength: true));
            AlterColumn("gallery.Artist", "Alias", c => c.String(maxLength: 256, fixedLength: true));
            AddPrimaryKey("gallery.User_OAuthMembership", new[] { "Provider", "ProviderUserId" });
            CreateIndex("gallery.User", "UserName", unique: true, name: "ux_User_UserName");
            CreateIndex("gallery.Tag", "Slug", unique: true, name: "ux_Tag_Slug");
            CreateIndex("gallery.Tag", "Name", unique: true, name: "ux_Tag_Name");
            CreateIndex("gallery.Group", "Slug", unique: true, name: "ux_Alias_Slug");
            CreateIndex("gallery.Group", "Name", unique: true, name: "ux_Group_Name");
            CreateIndex("gallery.Pack", "Date", name: "Date");
            CreateIndex("gallery.Pack", "Name", unique: true, name: "ux_Pack_Name");
            CreateIndex("gallery.Artist", "Slug", unique: true, name: "ux_Alias_Slug");
            CreateIndex("gallery.Artist", "Alias", unique: true, name: "ux_Alias_Name");
        }
    }
}
