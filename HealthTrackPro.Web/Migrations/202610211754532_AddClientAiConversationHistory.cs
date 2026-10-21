namespace HealthTrackPro.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddClientAiConversationHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AiConversations",
                c => new
                    {
                        AiConversationId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 120),
                        CreatedAtUtc = c.DateTime(nullable: false),
                        UpdatedAtUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AiConversationId)
                .ForeignKey("dbo.AspNetUsers", t => t.ClientUserId)
                .Index(t => t.ClientUserId);

            CreateTable(
                "dbo.AiMessages",
                c => new
                    {
                        AiMessageId = c.Int(nullable: false, identity: true),
                        AiConversationId = c.Int(nullable: false),
                        Role = c.String(nullable: false, maxLength: 20),
                        Content = c.String(nullable: false),
                        CreatedAtUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AiMessageId)
                .ForeignKey("dbo.AiConversations", t => t.AiConversationId, cascadeDelete: true)
                .Index(t => t.AiConversationId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AiMessages", "AiConversationId", "dbo.AiConversations");
            DropForeignKey("dbo.AiConversations", "ClientUserId", "dbo.AspNetUsers");
            DropIndex("dbo.AiMessages", new[] { "AiConversationId" });
            DropIndex("dbo.AiConversations", new[] { "ClientUserId" });
            DropTable("dbo.AiMessages");
            DropTable("dbo.AiConversations");
        }
    }
}
