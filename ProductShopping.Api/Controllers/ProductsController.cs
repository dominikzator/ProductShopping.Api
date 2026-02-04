using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs;
using ProductShopping.Api.Results;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService productsService) : BaseApiController
    {
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetProductsDto>>> Get()
        {
            var result = await productsService.GetCountriesAsync();

            return ToActionResult(result);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
