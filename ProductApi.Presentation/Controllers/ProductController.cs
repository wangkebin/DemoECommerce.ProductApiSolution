using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (products == null || !products.Any())
            {
                return NotFound("No product detected");
            }
            var (_, list) = ProductConversion.FromProduct(null!, products);
            return list!.Any() ? Ok(list) : NotFound("");

        }

        [HttpGet("{id int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.GetByIdAsync(id);
            if (product == null) {
                return NotFound($"No product found with id {id} in data");
            }
            var (_prod, _) = ProductConversion.FromProduct(product, null!);
            return _prod is not null ? Ok(_prod) : NotFound($"No product found with id {id}");
        }

        [HttpDelete("{id int}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {

            var response = await productInterface.DeleteAsync(new Product { Id=id});
            if (response is null) {
                return NotFound($"No product found with id {id} in data");
            }
            return response.Flag ? Ok(response) : NotFound($"No product found with id {id}");

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            var prod = ProductConversion.ToProduct(product);
            var response = await productInterface.UpdateAsync(prod);
            if (response is null) return NotFound($"No product found with id {product.Id} in data");
            return response.Flag ? Ok(response) : BadRequest($"No matching product to update");
        }
    }


}
