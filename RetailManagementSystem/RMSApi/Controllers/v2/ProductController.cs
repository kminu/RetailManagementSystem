using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMS.Library.Data;
using RMS.Library.Model;

namespace RMSApi.Controllers.v2
{
    [Route("api/v{verion:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    public class ProductController : ControllerBase
    {
        private readonly IProductData _productData;

        public ProductController(IProductData productData)
        {
            _productData = productData;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductModel>>> Get()
        {
            try
            {
                var output = await _productData.GetProducts();
                return Ok(output);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
