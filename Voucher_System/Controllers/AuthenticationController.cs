using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Voucher_System.Models;
using Voucher_System.Services.Interfaces;
using Voucher_System.ViewModels;

namespace Voucher_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ITokenService _tokenService;

        public AuthenticationController(ApplicationContext applicationContext, ITokenService tokenService)
        {
            _applicationContext = applicationContext;
            _tokenService = tokenService;
        }

        [HttpPost("customer/register")]

        // To receive data from the body it should be an object
        public async Task<ActionResult<SignInAndSignUpResponseDTO>> Register([FromBody] CustomerRegisterDTO registerDTO)
        {

            if (await UserExists(registerDTO.Email)) return BadRequest("Name is Taken");

            using var hmac = new HMACSHA512();

            var customer = new Customer()
            {
                Id = Guid.NewGuid(),
                dateOfBirth = registerDTO.dateOfBirth,
                Created = DateTime.UtcNow,
                Email = registerDTO.Email,
                lastActive = DateTime.UtcNow,
                Gender = registerDTO.gender,
                userName = registerDTO.userName.ToLower(),
                passwordHash = hmac.ComputeHash(Encoding.UTF32.GetBytes(registerDTO.password)),
                passwordSalt = hmac.Key
            };

            _applicationContext.Customers.Add(customer);
            await _applicationContext.SaveChangesAsync();

            return new SignInAndSignUpResponseDTO
            {
                userName = customer.userName,
                token = _tokenService.creatToken(customer.userName)
            };
        }

        [HttpPost("customer/login")]
        public async Task<ActionResult<SignInAndSignUpResponseDTO>> Login([FromBody] CustomerLogin loginDTO)
        {

            var customer = await _applicationContext.Customers
                     .SingleOrDefaultAsync(u => u.Email == loginDTO.email);

            if (customer == null) return Unauthorized("Invalid user credentials");

            using var hmc = new HMACSHA512(customer.passwordSalt);

            var ComputedHash = hmc.ComputeHash(Encoding.UTF32.GetBytes(loginDTO.password));

            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != customer.passwordHash[i]) return Unauthorized("Invalid password!");
            }

            return new SignInAndSignUpResponseDTO
            {
                userName = customer.userName,
                token = _tokenService.creatToken(customer.userName)
            };

        }
        private async Task<bool> UserExists(string email)
        {
            return await _applicationContext.Customers.AnyAsync(u => u.Email == email.ToLower());
        }

    }
}
