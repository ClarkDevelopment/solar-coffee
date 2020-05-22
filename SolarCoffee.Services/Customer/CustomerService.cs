using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SolarCoffee.Data;

namespace SolarCoffee.Services.Customer {
    public class CustomerService : ICustomerService {
        
        private readonly SolarDbContext _db;

        public CustomerService(SolarDbContext dbContext ) {
            _db = dbContext;
        }
        
        /// <summary>
        ///     Returns a list of customers and their corresponding CustomerAddress records
        /// </summary>
        /// <returns>Customer</returns>
        public List<Data.Models.Customer> GetAllCustomers() {
            return _db.Customers
                .Include(customer => customer.PrimaryAddress)
                .OrderBy(customer => customer.LastName)
                .ToList();
        }

        /// <summary>
        ///     Adds a new Customer record
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>ServiceResponse<Customer></returns>
        public ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer) {
            try {
                _db.Customers.Add(customer);
                _db.SaveChanges();
                
                return new ServiceResponse<Data.Models.Customer> {
                    Data = customer,
                    Time = DateTime.Now,
                    Message = "Successfully added a new customer",
                    IsSuccess = true
                };

            }
            catch (Exception e) {
                return new ServiceResponse<Data.Models.Customer> {
                    Data = null,
                    Time = DateTime.Now,
                    Message = "Failed to create new customer",
                    IsSuccess = false
                };
            }
        }
        
        /// <summary>
        ///     Deletes a Customer record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ServiceResponse<bool></returns>
        public ServiceResponse<bool> DeleteCustomer(int id) {
            try {
                var customer = _db.Customers.Find(id);

                if (customer == null) {
                    return new ServiceResponse<bool> {
                        Data = false,
                        Time = DateTime.Now,
                        Message = "Unable to locate Customer for deletion",
                        IsSuccess = false
                    };
                }
                _db.Customers.Remove(customer);
                _db.SaveChanges();

                return new ServiceResponse<bool> {
                    Data = true,
                    Time = DateTime.Now,
                    Message = "Successfully deleted the customer",
                    IsSuccess = true
                };
            }
            catch (Exception e) {
                return new ServiceResponse<bool> {
                    Data = false,
                    Time = DateTime.Now,
                    Message = e.StackTrace,
                    IsSuccess = true
                };
            }
        }

        /// <summary>
        ///     Gets a Customer record by primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Customer</returns>
        public Data.Models.Customer GetCustomerById(int id) {
            return _db.Customers.Find(id);
        }
    }
}