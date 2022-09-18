using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class WelcomeController : ControllerBase
    {
        [Route("/")]
        [HttpGet]
        public async Task<ActionResult> WelcomeMessage()
        {
            string message = "Welcome to RMS API";

            return Ok(message);
        }
    }
}