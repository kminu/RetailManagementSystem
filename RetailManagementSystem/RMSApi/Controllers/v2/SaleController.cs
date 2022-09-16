using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMS.Library.Data;
using RMS.Library.Model;
using System.Security.Claims;

namespace RMSApi.Controllers.v2
{
    [Route("api/v{verion:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }

        [HttpPost]
        public async Task<ActionResult> Post(SaleModel sale)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _saleData.SaveSale(sale, userId);
                
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetSalesReport")]
        public async Task<ActionResult<List<SaleReportModel>>> GetSalesReport()
        {
            try
            {
                var output = await _saleData.GetSaleReport();
                return Ok(output);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
