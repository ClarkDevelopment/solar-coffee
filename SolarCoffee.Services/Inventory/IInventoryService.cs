using System.Collections.Generic;
using SolarCoffee.Data.Models;

namespace SolarCoffee.Services.Inventory {
    public interface IInventoryService {
        public List<ProductInventory> GetCurrentInventory();
        public ServiceResponse<ProductInventory> UpdateAvailableUnits(int id, int adjustment);
        public ProductInventory GetInventoryByProductId(int productId);
        public List<ProductInventorySnapshot> GetSnapshotHistory();
    }
}