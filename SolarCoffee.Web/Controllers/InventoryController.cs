using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventoryService _inventoryService;

        public InventoryController(ILogger<InventoryController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet("/api/inventory/")]
        public ActionResult GetCurrentInventory()
        {
            _logger.LogInformation("Retrieving all inventory");
            var inventory = _inventoryService.GetCurrentInventory()
                .Select(pi => new ProductInventoryModel
                {
                    Id = pi.Id,
                    CreatedOn = pi.CreatedOn,
                    UpdatedOn = pi.UpdatedOn,
                    QuantityOnHand = pi.QuantityOnHand,
                    IdealQuantity = pi.IdealQuantity,
                    Product = ProductMapper.SerializeProductModel(pi.Product)
                })
                .OrderBy(inv => inv.Product.Name)
                .ToList();

            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                $"Updating inventory for {shipment.ProductId}"
            );
            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService
                .UpdateAvailableUnits(id, adjustment);

            return Ok(inventory);
        }

        [HttpGet("/api/inventory/snapshot")]
        public ActionResult GetSnapshotHistory()
        {
            _logger.LogInformation("Getting snapshot history");

            try
            {
                var snapshotHistory = _inventoryService.GetSnapshotHistory();

                var timelineMarkers = snapshotHistory
                    .Select(t => t.SnapshotTime)
                    .Distinct()
                    .ToList();

                var shapshots = snapshotHistory
                    .GroupBy(history => history.Product
                        , history => history.QuantityOnHand,
                        (key, g) => new ProductInventorySnapshotModel
                        {
                            ProductId = key.Id,
                            QuantityOnHand = g.ToList()
                        })
                    .OrderBy(history => history.ProductId)
                    .ToList();

                var viewModel = new SnapshotResponse()
                {
                    Timeline = timelineMarkers,
                    ProductInventorySnapshots = shapshots
                };

                return Ok(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting snapshot history");
                _logger.LogError(e.StackTrace);
                return BadRequest("Error retrieving snapshot history");
            }
        }
    }
}