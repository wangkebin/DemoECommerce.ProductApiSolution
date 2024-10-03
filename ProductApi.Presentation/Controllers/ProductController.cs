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

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            var prod = product.ToProduct();
            var response = await productInterface.CreateAsync(prod);
            if (response is null) return BadRequest("failed to created product");
            return response.Flag? Ok(response) : BadRequest("failed to create product");
            
            
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

        
        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var prod = product.ToProduct();
            var response = await productInterface.UpdateAsync(prod);
            return response.Flag ? Ok(response) : BadRequest($"No matching product to update");
        }
        
        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            var prod = product.ToProduct();
            var response = await productInterface.DeleteAsync(prod);
            return response.Flag ? Ok(response) : NotFound($"No product found with id {product.Id}");
        }

    }


}
