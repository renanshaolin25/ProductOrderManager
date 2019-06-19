namespace ProductOrderManager.Migrations.ProductOrderManagerContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabaseCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        email = c.String(),
                        orderDate = c.DateTime(nullable: false),
                        deliveryDate = c.DateTime(nullable: false),
                        orderStatus = c.String(),
                        totalPrice = c.String(),
                        totalWeight = c.String(),
                        freightPrice = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        qttProduct = c.Int(nullable: false),
                        productId = c.Long(nullable: false),
                        orderId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .Index(t => t.productId)
                .Index(t => t.orderId);
            
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
            DropForeignKey("dbo.OrderItems", "orderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "productId", "dbo.Products");
            DropIndex("dbo.Products", new[] { "code" });
            DropIndex("dbo.Products", new[] { "model" });
            DropIndex("dbo.OrderItems", new[] { "orderId" });
            DropIndex("dbo.OrderItems", new[] { "productId" });
            DropTable("dbo.Products");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
        }
    }
}
