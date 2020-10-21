using Microsoft.AspNetCore.Mvc;
using RpgApi.Data;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        
        private readonly DataContext _context;
        public UsuariosController(DataContext context)
        {
            _context = context;
        }
    }
}