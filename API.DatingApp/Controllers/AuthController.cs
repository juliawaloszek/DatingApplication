using System.Threading.Tasks;
using API.DatingApp.Data;
using API.DatingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (string username, string password)
        {
            //validate request

            username = username.ToLower();

            if(await _repo.UserExists(username))
                return BadRequest("Username already exist");
            
            var userToCreate = new User
            {
                Username = username
            };

            var createdUser = await _repo.Register(userToCreate, password);

            // return CreatedAtRoute()
            return StatusCode(201);
        }
    }
}