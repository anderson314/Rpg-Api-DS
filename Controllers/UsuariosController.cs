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


        //Método para autenticar o usuário
        private bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {   
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        //Método responsável por Autenticar o login
        [HttpPost("Autenticar")]
        public async Task<IActionResult> AutenticarUsuario(Usuario credenciaisUsuario)
        {

            Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(x =>
                x.Username.ToLower().Equals(credenciaisUsuario.Username.ToLower()));

            if(usuario ==null)
            {
                return BadRequest("Usuário não encontrado.");
            }
            else if (!VerificarPasswordHash(credenciaisUsuario.PasswordString,
                                usuario.PasswordHash, usuario.PasswordSalt))
            {
                return BadRequest("Senha incorreta.");
            }
            else
            {
                return Ok(usuario.Id);
            }
        }



        private readonly DataContext _context;
        public UsuariosController(DataContext context)
        {
            _context = context;
        }
    }
}