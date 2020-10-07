using Microsoft.AspNetCore.Mvc;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArmaController : ControllerBase
    {
        private Arma a = new Arma();
                
        public IActionResult Get()
        { 
            return Ok(a);
        }

    }
    
}