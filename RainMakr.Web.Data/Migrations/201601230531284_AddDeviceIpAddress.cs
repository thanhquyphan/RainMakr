namespace RainMakr.Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceIpAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Device", "IpAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Device", "IpAddress");
        }
    }
}
