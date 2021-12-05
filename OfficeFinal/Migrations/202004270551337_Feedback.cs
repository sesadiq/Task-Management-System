namespace OfficeFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feedback : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserFeedback", "AspNetUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserFeedback", "UserComment_Id", "dbo.UserComments");
            DropForeignKey("dbo.UserFeedback", "UserTask_TaskId", "dbo.UserTask");
            DropIndex("dbo.UserFeedback", new[] { "AspNetUser_Id" });
            DropIndex("dbo.UserFeedback", new[] { "UserComment_Id" });
            DropIndex("dbo.UserFeedback", new[] { "UserTask_TaskId" });
            CreateTable(
                "dbo.Feedback",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Feedback = c.String(maxLength: 250),
                        UserCommentId = c.Int(nullable: false),
                        UserTaskId = c.Int(nullable: false),
                        AspNetUserId = c.String(maxLength: 128),
                        UserTask_TaskId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUserId)
                .ForeignKey("dbo.UserComments", t => t.UserCommentId, cascadeDelete: true)
                .ForeignKey("dbo.UserTask", t => t.UserTask_TaskId)
                .Index(t => t.UserCommentId)
                .Index(t => t.AspNetUserId)
                .Index(t => t.UserTask_TaskId);
            
        }
        
        public override void Down()
        {
           
            
            DropForeignKey("dbo.Feedback", "UserTask_TaskId", "dbo.UserTask");
            DropForeignKey("dbo.Feedback", "UserCommentId", "dbo.UserComments");
            DropForeignKey("dbo.Feedback", "AspNetUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Feedback", new[] { "UserTask_TaskId" });
            DropIndex("dbo.Feedback", new[] { "AspNetUserId" });
            DropIndex("dbo.Feedback", new[] { "UserCommentId" });
            DropTable("dbo.Feedback");
            CreateIndex("dbo.UserFeedback", "UserTask_TaskId");
            CreateIndex("dbo.UserFeedback", "UserComment_Id");
            CreateIndex("dbo.UserFeedback", "AspNetUser_Id");
            AddForeignKey("dbo.UserFeedback", "UserTask_TaskId", "dbo.UserTask", "TaskId");
            AddForeignKey("dbo.UserFeedback", "UserComment_Id", "dbo.UserComments", "Id");
            AddForeignKey("dbo.UserFeedback", "AspNetUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
