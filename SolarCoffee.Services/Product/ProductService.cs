using System;
using System.Collections.Generic;
using System.Linq;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;

namespace SolarCoffee.Services.Product {
    public class ProductService : IProductService {
        
        private readonly SolarDbContext _db;

        public ProductService(SolarDbContext dbContext) {
            _db = dbContext;
        }
        
        /*
         * Returns a list of all products
         */
        public List<Data.Models.Product> GetAllProducts() {
            return _db.Products.ToList();
        }
        
        /*
         * Returns a Product object by id
         *
         * @param int id 
         */
        public Data.Models.Product GetProductById(int id) {
            return _db.Products.Find(id);
        }
        
        /*
         * Adds a new product to the database
         *
         * @param Product product 
         */
        public ServiceResponse<Data.Models.Product> CreateProduct(Data.Models.Product product) {
            try {
                _db.Products.Add(product);

                var newInventory = new ProductInventory {
                    Product = product,
                    QuantityOnHand = 0,
                    IdealQuantity = 10,
                    CreatedOn = DateTime.Now

                };

                _db.ProductInventories.Add(newInventory);

                _db.SaveChanges();
                
                return new ServiceResponse<Data.Models.Product> {
                    Data = product,
                    Time = DateTime.Now,
                    Message = "Product successfully added",
                    IsSuccess = true
                };
            }
            catch (Exception) {
                return new ServiceResponse<Data.Models.Product> {
                    Data = product,
                    Time = DateTime.Now,
                    Message = "Errors found adding new product",
                    IsSuccess = false
                };
            }
        }

        /*
         * Archives a product by setting the boolean IsArchived flag to true
         *
         * @param int id 
         */
        public ServiceResponse<Data.Models.Product> ArchiveProduct(int id) {
            try {
                var product = _db.Products.Find(id);
                product.IsArchived = true;
                _db.SaveChanges();

                return new ServiceResponse<Data.Models.Product> {
                    Data = product,
                    Time = DateTime.UtcNow,
                    Message = "Successfully archived",
                    IsSuccess = true
                };
            }
            catch (Exception) {
                return new ServiceResponse<Data.Models.Product> {
                    Data = null,
                    Time = DateTime.UtcNow,
                    Message = "Unable to archive product",
                    IsSuccess = false
                };
            }

        }
    }
}