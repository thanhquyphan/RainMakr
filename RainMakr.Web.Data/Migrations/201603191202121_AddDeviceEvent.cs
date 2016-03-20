namespace RainMakr.Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceEvent",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DeviceId = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        ScheduleId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DeviceEvent");
        }
    }
}
