using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mubank.Models;
using Mubank.Models.DTOS;
using Mubank.Services;
using Mubank.Services.IServices;
using System.Net.Http;

namespace Mubank.Controllers
{       
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admins,Owner")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;   
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;        
        private readonly IConfiguration _config;

        public UserController(DataContext context, IMapper mapper, ITokenService tokenService, ILogger<UserController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _config = config;
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

            if (id == null)
            {
                List<UserDTO> dtos = new List<UserDTO>();

                foreach (var user in _context.Users.ToList())
                {
                    var userdto = _mapper.Map<UserModel, UserDTO>(user);
                                        
                    dtos.Add(userdto);
                }

                return Ok(dtos);
            }
            else
            {
                var a = await _context.Users.Where(x => x.Id == id).Select(x => x).FirstOrDefaultAsync();

                if (a == null)
                {
                    return NotFound(Error_Throwed("Usuario não encontrado"));
                } else
                {                    

                    var ReturnUserFULLdata = new UserFullDataDTO()
                    {
                        UserID = a.Id,
                        Name = a.Name,
                        Email = a.Email,
                        DateCreated = DateTime.Now,
                        RoleName = _context.Accounts.Find(a.Id).RoleName,
                        Transactions = _context.Transations.Where(X => X.IDDequemfez == a.Id).ToList() ?? null,                        
                    };

                    return Ok(ReturnUserFULLdata);
                }

                    
            }
             
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
                    Account = new Models.ModelsHaveShip.AccountModel
                    {
                        Id = Id,
                        RoleName = "NormalUser",
                        Value = 0,
                        TransationsMade = new List<TransationsModel>(),
                        TransationsRecived = new List<TransationsModel>()
                    },
                };

                var UserDto = _mapper.Map<UserModel, UserDTO>(model);                                                

                await _context.Users.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(UserDto);
            }
                
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(Guid idForSearch, UserCreateDTO? userupdate)
        {            
            if (idForSearch == null)
            {
                return BadRequest(Error_Throwed("Id nulo, impossivel atualizar usuario sem o ID para authenticação"));                
            } else if (userupdate == null) 
            {


                var str = new GeminiService();

                return Conflict(new
                {
                    Message = "Não tem oq atualizar, bruh",
                    DataDeHoje = DateTime.Now,
                    MensagemDoDiaDoPastor = await str.SendHttpPost("Gere uma frase biblica para alegrar o dia de uma pessoa tudo em uma linha só sem conter a fonte dela,sem conter aspas, apenas a frase", _config["ApiKeys:ApiGeminiKey"])
                });
            } else
            {
                var user = await _context.Users.Where(x => x.Id == idForSearch).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(Error_Throwed("Usuario não encontrado"));
                }
                user.Name = userupdate.Name;
                user.Email = userupdate.Email;
                user.Password = BCrypt.Net.BCrypt.HashPassword(userupdate.Password);                
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var UserDto = _mapper.Map<UserModel, UserDTO>(user);
                
                return Ok(UserDto);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Guid idForSearch)
        {            
            if (idForSearch == null)
            {
                return BadRequest(Error_Throwed("Id nulo, impossivel deletar usuario sem o ID para authenticação"));
            }
            else
            {
                var user = await _context.Users.Where(x => x.Id == idForSearch).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(Error_Throwed("Usuario não encontrado"));
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Usuario deletado com sucesso" });
            }
        }
    }
}
