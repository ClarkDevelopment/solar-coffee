using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Customer;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger,
                ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpPost("/api/customer")]
        public ActionResult CreateCustomer([FromBody] CustomerModel customer)
        {
            _logger.LogInformation("Creating a new customer");
            customer.CreatedOn = DateTime.Now;
            customer.UpdatedOn = DateTime.Now;
            var customerData = CustomerMapper.SerializeCustomer(customer);
            var newCustomer = _customerService.CreateCustomer(customerData);
            return Ok(newCustomer);
        }
        
        [HttpGet("/api/customer")]
        public ActionResult GetCustomerById(int id)
        { 
            _logger.LogInformation("Retrieving customers list");
            var customer = _customerService.GetCustomerById(id);
            return Ok(customer);
        }

        [HttpGet("/api/customer")]
        public ActionResult GetCustomers()
        { 
            _logger.LogInformation("Retrieving customers list");
            var customers = _customerService.GetAllCustomers();
            var customerModels = customers.Select(customer => new CustomerModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PrimaryAddress = CustomerMapper
                    .MapCustomerAddress(customer.PrimaryAddress),
                CreatedOn = customer.CreatedOn,
                UpdatedOn = customer.UpdatedOn
            })
            .OrderByDescending(customer =>customer.CreatedOn)
            .ToList();

            return Ok(customerModels);
        }

        [HttpDelete("/api/customer/{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            _logger.LogInformation($"Deleting customer {id}");
            var customerResponse = _customerService.DeleteCustomer(id);
            return Ok(customerResponse);
        }
    }
}