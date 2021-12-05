namespace OfficeFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserTask", "TaskTypeId", c => c.Int());
            CreateIndex("dbo.UserTask", "TaskTypeId");
            AddForeignKey("dbo.UserTask", "TaskTypeId", "dbo.TaskTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTask", "TaskTypeId", "dbo.TaskTypes");
            DropIndex("dbo.UserTask", new[] { "TaskTypeId" });
            DropColumn("dbo.UserTask", "TaskTypeId");
            DropTable("dbo.TaskTypes");
        }
    }
}
