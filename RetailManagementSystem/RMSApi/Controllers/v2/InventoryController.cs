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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryData _inventoryData;

        public InventoryController(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryModel>>> Get()
        {
            try
            {
                var output = await _inventoryData.GetInventory();
                return Ok(output);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]InventoryModel item)
        {
            try
            {
                await _inventoryData.SaveInventoryRecord(item);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}
