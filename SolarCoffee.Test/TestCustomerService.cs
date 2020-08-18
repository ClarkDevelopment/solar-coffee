using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Customer;
using Xunit;

namespace SolarCoffee.Test
{
    public class TestCustomerService
    {
        [Fact]
        public void CustomerService_CreateCustomer_GivenNewCustomerObject()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("Create_customer").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);

            sut.CreateCustomer(new Customer {Id = 65465});
            context.Customers.Single().Id.Should().Be(65465);
        }
        
        [Fact]
        public void CustomerService_GetAllCustomers_GivenTheyExist()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("gets_all").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);
            
            //We've already tested the CreateCustomer method, but in the event it fails, that could
            //be ambiguous here. Probably a better option to just seed the database with mock data
            sut.CreateCustomer(new Customer {Id = 65465});
            sut.CreateCustomer(new Customer {Id = 67887});

            var allCustomers = sut.GetAllCustomers();

            allCustomers.Count.Should().Be(2);
        }

        [Fact]
        public void CustomerService_DeleteCustomer_GivenValidCustomerId()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("Delete_customer").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);
            sut.CreateCustomer(new Customer {Id = 666});
            //Likely redundant to double check CreateCustomers, but to ensure accuracy, I will.
            context.Customers.Single().Id.Should().Be(666);
            sut.DeleteCustomer(666);
            var allCustomers = sut.GetAllCustomers();
            allCustomers.Count.Should().Be(0);
        }
        
        [Fact]
        public void CustomerService_OrdersByLastName_WhenGetAllCustomersInvoked()
        {
            //Assign
            var data = new List<Customer>
            {
                new Customer {Id = 123, LastName = "Clark"},
                new Customer {Id = 231, LastName = "Adkins"},
                new Customer {Id = 456, LastName = "Smith"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Customer>>();
            
            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.Provider)
                .Returns(data.Provider);
            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator);

            var mockContext = new Mock<SolarDbContext>();
            
            mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
            
            // Act
            
            var sut = new CustomerService(mockContext.Object);
            var customers = sut.GetAllCustomers();
            
            // Assert
            customers.Count.Should().Be(3);
            customers[0].Id.Should().Be(231);
            customers[1].Id.Should().Be(123);
            customers[2].Id.Should().Be(456);

        }
    }
}