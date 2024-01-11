//using Azure;
using EventScheduler.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EventScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly UserContext _dbContext;
        public AuthController(UserContext dbContext, IConfiguration configurtion)
        {
            _dbContext = dbContext;
            _configuration = configurtion;

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponsedto>> Register(UserDto request)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return a BadRequest response with the validation errors.
                return BadRequest(ModelState);
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                Passwordhash = passwordHash,
                Passwordsalt = passwordSalt,
                Email = request.Email,
                Phone = request.PhoneNo,
                role= request.role
                
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userResponseDto = new UserResponsedto
            {
                Username = user.Username
            };

            return Ok(userResponseDto);

        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            if (!VerifyPasswordHash(request.Password, user.Passwordhash, user.Passwordsalt))
            {
                return BadRequest("Wrong Password");
            }
            string token = Createtoken(user);
            return Ok(new { token });
        }

        [HttpGet("Test_Endpoint")]//, Authorize(Roles ="Admin")]
        public ActionResult test()
        {
            return Ok();

        }
        private string Createtoken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.Username),
               // new Claim(ClaimTypes.Role,user.role)
            };
            //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            //    _configuration.GetSection("AppSettings:Token").Value));
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("my secret top key"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(null,null,claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}