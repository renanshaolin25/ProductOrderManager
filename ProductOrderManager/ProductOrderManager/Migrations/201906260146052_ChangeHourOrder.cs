namespace ProductOrderManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeHourOrder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "orderDate", c => c.DateTime());
            AlterColumn("dbo.Orders", "deliveryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "deliveryDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "orderDate", c => c.DateTime(nullable: false));
        }
    }
}
