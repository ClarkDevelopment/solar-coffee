using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers {
    [ApiController]
    public class ProductController : ControllerBase {

        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService) {
            _logger = logger;
            _productService = productService;
        }
        
        [HttpGet("/api/product")]
        public ActionResult GetProduct() {
            _logger.LogInformation("Getting all products");
            var products = _productService.GetAllProducts();
            
            //Shorthand instead of using a lambda in the Select
            var productViewModels = products.Select(ProductMapper.SerializeProductModel);
            return Ok(productViewModels);
        }

        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("/api/product")]
        public ActionResult AddProduct([FromBody] ProductModel product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _logger.LogInformation("Adding new product");
            var newProduct = ProductMapper.SerializeProductModel(product);
            var addResult= _productService.CreateProduct(newProduct);
            return Ok(addResult);
        }

        /// <summary>
        /// Flip the archive flag for a particular product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("/api/product/{id}")]
        public ActionResult ArchiveProduct(int id)
        {
            _logger.LogInformation($"Archiving product {id}");
            var archiveResult = _productService.ArchiveProduct(id);
            return Ok(archiveResult);
        }
    }
}