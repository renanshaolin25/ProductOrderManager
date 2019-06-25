namespace ProductOrderManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrder : DbMigration
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
                        totalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        totalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        freightPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        qttProduct = c.Int(nullable: false),
                        ProductId = c.Long(nullable: false),
                        OrderId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
        }
    }
}
