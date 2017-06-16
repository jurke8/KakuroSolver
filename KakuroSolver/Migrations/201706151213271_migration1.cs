namespace KakuroSolver.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KakuroStatistics", "Rows", c => c.Int(nullable: false));
            AddColumn("dbo.KakuroStatistics", "Columns", c => c.Int(nullable: false));
            AddColumn("dbo.KakuroStatistics", "LoadTime", c => c.Long(nullable: false));
            AddColumn("dbo.KakuroStatistics", "Loaded", c => c.Boolean(nullable: false));
            AddColumn("dbo.KakuroStatistics", "Solved", c => c.Boolean(nullable: false));
            DropColumn("dbo.KakuroStatistics", "NumberOfFields");
            DropColumn("dbo.KakuroStatistics", "ReadTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KakuroStatistics", "ReadTime", c => c.Long(nullable: false));
            AddColumn("dbo.KakuroStatistics", "NumberOfFields", c => c.Int(nullable: false));
            DropColumn("dbo.KakuroStatistics", "Solved");
            DropColumn("dbo.KakuroStatistics", "Loaded");
            DropColumn("dbo.KakuroStatistics", "LoadTime");
            DropColumn("dbo.KakuroStatistics", "Columns");
            DropColumn("dbo.KakuroStatistics", "Rows");
        }
    }
}
