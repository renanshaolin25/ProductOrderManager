namespace ProductOrderManager.Migrations
{
    using ProductOrderManager.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProductOrderManager.Models.ProductOrderManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProductOrderManager.Models.ProductOrderManagerContext context)
        {
            context.Products.AddOrUpdate(
            p => p.Id,
            new Product
            {
                Id = 1,
                name = "Product 1",
                model = "Model 1",
                code = "Code 1",
                description = "Description 1",
                color = "color 1",
                imageURL = "URL 1",
                price = 10.5M,
                weight = 10.3M,
                height = 10.4M,
                width = 20.8M,
                length = 20.7M,
                diameter = 30.1M
            },
            new Product
            {
                Id = 2,
                name = "Product 2",
                model = "Model 2",
                code = "Code 2",
                description = "Description 2",
                color = "color 1",
                imageURL = "URL 2",
                price = 10.5M,
                weight = 10.3M,
                height = 10.4M,
                width = 20.8M,
                length = 20.7M,
                diameter = 30.1M
            });
        }
    }
}
