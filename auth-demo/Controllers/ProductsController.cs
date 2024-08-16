using auth_demo.Models;
using auth_demo.Services;
using Microsoft.AspNetCore.Mvc;

namespace auth_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController()
        {
            _service = new ProductService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Products>> GetProducts()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Products> GetProduct(int id)
        {
            var product = _service.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Products> PostProduct(Products product)
        {
            _service.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Products updatedProduct)
        {
            // Retrieve the product from your data store
            var product = _service.GetById(id);
            if (product == null)
            {
                // Return 404 Not Found if the product does not exist
                return NotFound();
            }

            // Update product properties
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;

            // Save changes to your data store
            _service.Update(product);

            // Return the updated product
            return Ok(product);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _service.Delete(id);
            return Ok(new { message = "Product successfully deleted." });
        }
    }
}

