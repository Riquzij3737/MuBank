using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mubank.Models;
using Mubank.Services;
using Mubank.Services.IServices;

namespace Mubank.Controllers
{       
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;   
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;        

        public UserController(DataContext context, IMapper mapper, ITokenService tokenService, ILogger<UserController> logger)
        {            
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        private async void Client_Connected(string Message)
        {
            var Connect = new HostConnectLogModel()
            {
                ConnectID = Guid.NewGuid(),
                IpAddress = HttpContext.Connection.LocalIpAddress.ToString(),
                Message = Message,
                Port = HttpContext.Connection.LocalPort,
                date = DateTime.Now
            };

            await _context.HostConnectLog.AddAsync(Connect);

            await _context.SaveChangesAsync();
        }

        private async Task<object> Error_Throwed(string Message)
        {

            var error = new ErrorModel()
            {
                IdError = Guid.NewGuid(),
                MessageError = Message,
                HttpStatusCode = 400,
                Date = DateTime.Now
            };

            var dto = _mapper.Map<ErrorModel, ErrorDTO>(error);
            await _context.Errors.AddAsync(error);
            await _context.SaveChangesAsync();

            return dto;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAsync(Guid? id)
        {
            Client_Connected("Cliente conectado na rota /api/user para usar o seu metodo GET para receber os usuarios presentes no banco de dados");

            if (id == null)
                return Ok(await _context.Users.ToListAsync());
            else
                return Ok(await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDTO user)
        {          
            Guid Id = Guid.NewGuid();

            if (user == null)
            {
                return BadRequest(Error_Throwed("Error, Pois o modelo recebido foi recebido  como nulo"));
            } else
            {
                var model = new UserModel()
                {
                    Id = Id,
                    Name = user.Name,
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    RoleName = "NormalUser"
                };

                var UserDto = _mapper.Map<UserModel, UserDTO>(model);
                
                UserDto.password = string.Empty;                

                for (int i = 0; i < user.Password.Length;i++)
                {
                    UserDto.password += "*";
                }

                await _context.Users.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(UserDto);
            }
                
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(Guid idForSearch, UserCreateDTO? userupdate)
        {
            Client_Connected("Usuario conectado para atualizar algum dado de usuarios do banco de dados da api");

            if (userupdate == null)
            {

            }
        }
    }
}
