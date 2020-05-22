using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Product;

namespace SolarCoffee.Services.Order {
    public class OrderService : IOrderService {
        
        private readonly SolarDbContext _db;
        private readonly ILogger<OrderService> _logger;
        private readonly ProductService _productService;
        private readonly InventoryService _inventoryService;
        
        public OrderService(SolarDbContext dbContext, ILogger<OrderService> logger, 
                            ProductService product, InventoryService inventory) {
            _db = dbContext;
            _logger = logger;
            _productService = product;
            _inventoryService = inventory;
        }

        public List<SalesOrder> GetOrders() {
            return _db.SalesOrders
                .Include(so => so.Customer)
                    .ThenInclude((customer => customer.PrimaryAddress))
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(item => item.Product)
                .ToList();
        }

        /// <summary>
        ///     Creates an open sales order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResponse<bool> GenerateInvoiceForOrder(SalesOrder order) {
            foreach (var item in order.SalesOrderItems) {
                item.Product = _productService
                    .GetProductById(item.Product.Id);
                var inventoryId = _inventoryService
                    .GetInventoryByProductId(item.Product.Id).Id;

                _inventoryService.UpdateAvailableUnits(inventoryId, -item.Quantity);
            }

            try {
                _db.SalesOrders.Add(order);
                _db.SaveChanges();
                
                _logger.LogInformation($"Successfully generated an invoice order for order {order.Id}");

                return new ServiceResponse<bool> {
                    Data = true,
                    Time = DateTime.Now,
                    Message = "Successfully generated a new sales order",
                    IsSuccess = true
                };
            }
            catch (Exception e) {
                return new ServiceResponse<bool> {
                    Data = false,
                    Time = DateTime.Now,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        ///     Mark an order as paid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<bool> MarkFulfilled(int id) {
            try {
                var order = _db.SalesOrders.Find(id);
                order.UpdatedOn = DateTime.Now;
                order.IsPaid = true;
                _db.SalesOrders.Update(order);
                _db.SaveChanges();

                return new ServiceResponse<bool> {
                    Data = true,
                    Time = DateTime.Now,
                    Message = "Inventory record successfully updated",
                    IsSuccess = true
                };
            }
            catch (Exception e) {
                return new ServiceResponse<bool> {
                    Data = true,
                    Time = DateTime.Now,
                    Message = e.StackTrace,
                    IsSuccess = true
                };
            }
        }
    }
}