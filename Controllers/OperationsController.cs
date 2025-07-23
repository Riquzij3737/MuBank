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
    [Authorize("Admin,Owner,NormalUser")]
    public class OperationsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OperationsController> _logger;
        private readonly IConfiguration _config;

        public OperationsController(DataContext context, IMapper mapper, ITokenService tokenService, ILogger<OperationsController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpGet("{pass")]
        public async Task<IActionResult> GetIdUsingPass(UserCreateDTO Crendentiais)
        {
            if (Crendentiais == null || string.IsNullOrEmpty(Crendentiais.Password))
            {
                _logger.LogError("Credenciais inválidas fornecidas.");
                return BadRequest("Credenciais inválidas.");
            }
            else
            {
                var id = await _context.Users
                    .Where(x => BCrypt.Net.BCrypt.Verify(Crendentiais.Password, x.Password, false, HashType.SHA384) && x.Email == Crendentiais.Email && x.Name == Crendentiais.Name)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                if (id == Guid.Empty)
                {
                    return NotFound("Usuário não encontrado com as credenciais fornecidas.");
                }
                else
                {
                    return Ok(new { UserId = id });
                }


            }
        }

        [HttpGet]
        [Route("GetOperationsRealized/{id}")]
        public async Task<IActionResult> GetOperationsRealized([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("ID inválido fornecido para obter operações.");
                return BadRequest("ID inválido.");
            }
            try
            {
                var Operations = await _context.Transations
                    .Where(x => x.IDDequemfez == id)
                    .ToListAsync();

                if (Operations == null || !Operations.Any())
                {
                    return NotFound("Nenhuma operação feita encontrada neste ID");
                } else
                {
                    return Ok(Operations);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        [HttpGet]
        [Route("GetOperationsReceived/{id}")]
        public async Task<IActionResult> GetOperationsReceived([FromRoute] Guid id)
        {

            if (id == Guid.Empty)
            {
                _logger.LogError("ID inválido fornecido para obter operações.");                
                return BadRequest("ID inválido.");
            }
            try
            {
                var Operations = await _context.Transations
                    .Where(x => x.IDDequemfez == id)
                    .ToListAsync();

                if (Operations == null || !Operations.Any())
                {
                    _logger.LogInformation($"Nenhuma operação encontrada para o usuário com ID: {id}");
                    return NotFound("Nenhuma operação encontrada neste ID");
                }
                else
                {
                    _logger.LogInformation($"Operações encontradas para o usuário com ID: {id}");
                    return Ok(Operations);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter operações: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendPostValue(Guid QuemVaifazer, Guid QuemVaireceber, decimal Valor)
        {
            if (Valor <= 0)
            {
                _logger.LogError("Valor inválido fornecido para a transação.");
                return BadRequest("Valor inválido.");
            }

            try
            {
                if (QuemVaifazer == Guid.Empty || QuemVaireceber == Guid.Empty)
                {
                    _logger.LogError("IDs inválidos fornecidos para a transação.");
                    return BadRequest("IDs inválidos.");
                } else
                {
                    var user1 = await _context.Users.FindAsync(QuemVaifazer);
                    var user2 = await _context.Users.FindAsync(QuemVaireceber);

                    if (user1 == null || user2 == null)
                    {
                        _logger.LogError("Usuário não encontrado para os IDs fornecidos.");
                        return NotFound("Usuário não encontrado.");
                    } else
                    {
                        if (user1.Value < Valor)
                        {
                            _logger.LogError("Saldo insuficiente para a transação.");
                            return BadRequest("Saldo insuficiente.");
                        }
                      
                        user1.Value -= Valor;
                        user2.Value += Valor;
                        var transacao = new TransationsModel
                        {
                            IDDequemfez = QuemVaifazer,
                            IDDequemrecebeu = QuemVaireceber,
                            Value = Valor,
                            TransationData = DateTime.Now.ToString("dd:MM:yyyy"),
                        };

                        await _context.Transations.AddAsync(transacao);

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}