using BestStore.Models;
using BestStore.Services;
using BestStore_Application.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BestStore.Tests.Controller
{
    public class ProductControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly ProductsController _controller;

        public ProductControllerTests()
        {
            // Setup In-Memory DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed products
            _context.Products.AddRange(
                new Product { Id = 1, Name = "Laptop", Brand = "Dell", Category = "Electronics", Price = 50000, CreateAt = DateTime.Now },
                new Product { Id = 2, Name = "Mouse", Brand = "HP", Category = "Accessories", Price = 500, CreateAt = DateTime.Now },
                new Product { Id = 3, Name = "Keyboard", Brand = "Logitech", Category = "Accessories", Price = 1500, CreateAt = DateTime.Now },
                new Product { Id = 4, Name = "Monitor", Brand = "Samsung", Category = "Electronics", Price = 10000, CreateAt = DateTime.Now }
            );
            _context.SaveChanges();

            _mockEnv = new Mock<IWebHostEnvironment>();
            _controller = new ProductsController(_context, _mockEnv.Object);
        }

        [Fact]
        public void Index_Returns_ViewResult_With_ProductList()
        {
            // Act
            var result = _controller.Index(1, null, "Name", "asc");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);

            Assert.Equal(3, model.Count); // PageSize = 3
            Assert.Equal("Keyboard", model[0].Name); // Sorted by name ascending
            Assert.Equal("Laptop", model[1].Name);
            Assert.Equal("Monitor", model[2].Name);
        }
    }
}
