using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer;
using Mubank.Models;
using Mubank.Services;
using Mubank.Services.IServices;
using System.Dynamic;
using System.Net;

namespace Mubank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _token;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly ILogger<AccountsController> _logger;
        private readonly DataContext _context;

        public AccountsController(IMapper mapper, IHttpContextAccessor httpContext, ILogger<AccountsController> logger, DataContext context,ITokenService token)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _HttpContext = httpContext;
            _token = token;
        }

        [HttpPost]
        [Route("Subscribe")]
        public async Task<IActionResult> SubscribePost(UserCreateDTO userDto)
        {
            _logger.LogInformation($"Novo registro de usuário - IP: {HttpContext.Connection.RemoteIpAddress}");

            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                _logger.LogError("Dados de registro inválidos");

                var error = new ErrorModel
                {
                    IdError = Guid.NewGuid(),
                    MessageError = "Dados de entrada inválidos",
                    HttpStatusCode = 400,
                    Date = DateTime.Now
                };

                await _context.Errors.AddAsync(error);
                await _context.SaveChangesAsync();

                var errorDto = _mapper.Map<ErrorDTO>(error);
                errorDto.IdError = Guid.Empty;

                return BadRequest(errorDto);
            }

            // Verifica se o email já existe
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return Conflict(new { message = "Email já está em uso." });
            }

            // Cria novo usuário
            var newUser = new UserModel
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                RoleName = "NormalUser"
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // Gera token e retorna o UserResponseDTO
            var response = new UserResponseDTO
            {
                Name = newUser.Name,
                Email = newUser.Email,
                RoleName = newUser.RoleName,
                JwtToken = _token.GenerationToken(newUser)
            };

            return Ok(response);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginPost(UserCreateDTO userDto)
        {
            if (userDto == null)
            {
                var error = new ErrorModel
                {
                    IdError = Guid.NewGuid(),
                    MessageError = "Argumentos nulos ou inválidos",
                    HttpStatusCode = 400,
                    Date = DateTime.Now
                };

                await _context.Errors.AddAsync(error);
                await _context.SaveChangesAsync();

                var dto = _mapper.Map<ErrorDTO>(error);
                dto.IdError = Guid.Empty;

                return BadRequest(dto);
            }

            var user = await _context.Users
            .Where(x => x.Name == userDto.Name && x.Email == userDto.Email)
            .ToListAsync();

            var verifiedUser = user.FirstOrDefault(x => BCrypt.Net.BCrypt.Verify(userDto.Password, x.Password, false, HashType.SHA384));

            
            if (verifiedUser == null)
            {
                var error = new ErrorModel
                {
                    IdError = Guid.NewGuid(),
                    MessageError = "Login negado ou não encontrado",
                    HttpStatusCode = 401,
                    Date = DateTime.Now
                };

                await _context.Errors.AddAsync(error);
                await _context.SaveChangesAsync();

                var dto = _mapper.Map<ErrorDTO>(error);
                dto.IdError = Guid.Empty;

                return NotFound(dto);
            }

            return Ok(new
            {
                NameUser = verifiedUser.Name,
                JwtToken = _token.GenerationToken(verifiedUser)
            });
        }

    }
}
