using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        
        //Método responsável por criptografar as senhas
        private void CriarPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //Método responsável em verificar se usuário existe no banco de dados
        public async Task<bool> UsuarioExistente(string username)
        {
            if(await _context.Usuarios.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        //Método responsável por registrar usuário
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuario user)
        {
            if(await UsuarioExistente(user.Username))
                return BadRequest("Nome de usuário já existe");
            
            CriarPasswordHash(user.PasswordString, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordString = string.Empty;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Usuarios.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }


        private readonly DataContext _context;
        public UsuariosController(DataContext context)
        {
            _context = context;
        }
    }
}