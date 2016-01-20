namespace RainMakr.Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceAndScheduleTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Device",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 200),
                        PersonId = c.String(maxLength: 128),
                        MacAddress = c.String(nullable: false, maxLength: 17),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            this.Sql("create unique nonclustered index IX_Device_PersonIdMacAddress on dbo.Device(PersonId, MacAddress)");
            this.Sql("create unique nonclustered index IX_Device_PersonIdName on dbo.Device(PersonId, Name)");

            CreateTable(
                "dbo.Schedule",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CheckForRain = c.Boolean(nullable: false),
                        Recurrence = c.Boolean(nullable: false),
                        StartDate = c.DateTime(),
                        Offset = c.Time(nullable: false, precision: 7),
                        Duration = c.Int(nullable: false),
                        Days = c.Int(),
                        DeviceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Device", t => t.DeviceId)
                .Index(t => t.DeviceId);
            this.Sql("create unique nonclustered index IX_Schedule_UniqueScheduleTrigger on dbo.Schedule(DeviceId, StartDate, Offset, Days)");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedule", "DeviceId", "dbo.Device");
            DropForeignKey("dbo.Device", "PersonId", "dbo.Person");
            DropIndex("dbo.Schedule", new[] { "DeviceId" });
            DropIndex("dbo.Schedule", "IX_Schedule_UniqueScheduleTrigger");
            DropIndex("dbo.Device", new[] { "PersonId" });
            DropIndex("dbo.Device", "IX_Device_PersonIdMacAddress");
            DropIndex("dbo.Device", "IX_Device_PersonIdName");
            DropTable("dbo.Schedule");
            DropTable("dbo.Device");
        }
    }
}
