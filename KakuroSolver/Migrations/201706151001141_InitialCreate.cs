namespace KakuroSolver.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KakuroStatistics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        NumberOfFields = c.Int(nullable: false),
                        ReadTime = c.Long(nullable: false),
                        SolveTime = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.KakuroStatistics");
        }
    }
}
