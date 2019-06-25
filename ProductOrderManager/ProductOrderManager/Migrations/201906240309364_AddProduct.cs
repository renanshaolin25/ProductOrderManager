namespace ProductOrderManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        model = c.String(nullable: false, maxLength: 200),
                        code = c.String(nullable: false, maxLength: 200),
                        description = c.String(),
                        color = c.String(),
                        imageURL = c.String(),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        diameter = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.model, unique: true)
                .Index(t => t.code, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", new[] { "code" });
            DropIndex("dbo.Products", new[] { "model" });
            DropTable("dbo.Products");
        }
    }
}
