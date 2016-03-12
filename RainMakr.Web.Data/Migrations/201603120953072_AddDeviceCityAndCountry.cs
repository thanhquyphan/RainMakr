namespace RainMakr.Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceCityAndCountry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Device", "City", c => c.String(maxLength: 200));
            AddColumn("dbo.Device", "CountryCode", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Device", "CountryCode");
            DropColumn("dbo.Device", "City");
        }
    }
}
