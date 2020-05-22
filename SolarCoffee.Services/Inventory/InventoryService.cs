using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;

namespace SolarCoffee.Services.Inventory {
    public class InventoryService : IInventoryService {

        private readonly SolarDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(SolarDbContext dbContext, ILogger<InventoryService> logger) {
            _db = dbContext;
            _logger = logger;
        }
        
        /// <summary>
        ///     Retrieves a list of Inventory units
        /// </summary>
        /// <returns>ProductInventory</returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<ProductInventory> GetCurrentInventory() {
            return _db.ProductInventories
                .Include(inventory => inventory.Product)
                .Where(inventory => !inventory.Product.IsArchived)
                .ToList();
        }
        
        /// <summary>
        ///     Updates available units of the provided product id
        ///     Adjusts the QuantityOnHand by adjustment value
        /// </summary>
        /// <param name="id">productId</param>
        /// <param name="adjustment">number of units added or removed</param>
        /// <returns></returns>
        public ServiceResponse<ProductInventory> UpdateAvailableUnits(int id, int adjustment) {
            try {
                var inventory = _db.ProductInventories
                    .Include(inv => inv.Product)
                    .First(inv => inv.Product.Id == id);

                inventory.QuantityOnHand += adjustment;

                try {
                    CreateSnapshot(inventory);
                }
                catch (Exception e) {
                    _logger.LogError("Error creating inventory snapshot");
                    _logger.LogError(e.StackTrace);
                }
                
                _db.SaveChanges();

                return new ServiceResponse<ProductInventory> {
                    Data = inventory,
                    Time = DateTime.Now,
                    Message = "Inventory record successfully updated",
                    IsSuccess = true
                };
            }
            catch (Exception e) {
                return new ServiceResponse<ProductInventory> {
                    Data = null,
                    Time = DateTime.Now,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }
        
        /// <summary>
        ///     Retrieves an inventory record by Product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductInventory GetInventoryByProductId(int productId) {
            return _db.ProductInventories
                .Include(pi => pi.Product)
                .FirstOrDefault(pi => pi.Product.Id == productId);
        }

        /// <summary>
        ///     Retrieves snapshot history from the database for the last 6 hours
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<ProductInventorySnapshot> GetSnapshotHistory() {
            var earliest = DateTime.Now - TimeSpan.FromHours(6);

            return _db.ProductInventorySnapshots
                .Include(snap => snap.Product)
                .Where(snap 
                    => snap.SnapshotTime > earliest
                       && !snap.Product.IsArchived)
                .ToList();
        }
        
        /// <summary>
        ///     Creates a snapshot of the Inventory object's QuantityOnHand
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateSnapshot(ProductInventory inventory) {
            var snapshot = new ProductInventorySnapshot {
                SnapshotTime = DateTime.Now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };

            _db.Add(snapshot);
        }
    }
}