using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Mubank.Models;
using Mubank.Services;
using Mubank.Services.IServices;

namespace Mubank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class OperationsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OperationsController> _logger;
        private readonly IConfiguration _config;
        private readonly List<TransationsModel> _transations = new List<TransationsModel>();

        public OperationsController(DataContext context, IMapper mapper, ITokenService tokenService, ILogger<OperationsController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpGet]
        [Route("GetIdUsingPass")]
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
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter operações: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor.");
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


        [HttpGet]
        [Route("GetOperationsRealized")]
        public async Task<IActionResult> GetOperationsRealized()
        {
            try
            {
                var transationsRealized = await _context.Transations.ToListAsync();

                if (transationsRealized == null || !transationsRealized.Any())
                {
                    _logger.LogInformation("Nenhuma transação realizada encontrada.");
                    return NotFound("Nenhuma transação realizada encontrada.");
                } else
                {
                    return Ok(transationsRealized);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter operações: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor.");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendPostValue(TransitionsDTO transitions)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (transitions.Value <= 0)
            {
                _logger.LogError("Valor inválido fornecido para a transação.");
                return BadRequest("Valor inválido.");
            }

            if (transitions.IDDequemfez == Guid.Empty || transitions.IDDequemrecebeu == Guid.Empty)
            {
                _logger.LogError("IDs inválidos fornecidos para a transação.");
                return BadRequest("IDs inválidos.");
            }

            try
            {
                var user1 = await _context.Users.FindAsync(transitions.IDDequemfez);
                var user2 = await _context.Users.FindAsync(transitions.IDDequemrecebeu);

                if (user1 == null || user2 == null)
                {
                    _logger.LogError("Usuário não encontrado para os IDs fornecidos.");
                    return NotFound("Usuário não encontrado.");
                }

                if (user1.Value < transitions.Value)
                {
                    _logger.LogError("Saldo insuficiente para a transação.");
                    return BadRequest("Saldo insuficiente.");
                }

                // Use transação para garantir atomicidade
                using var transaction = await _context.Database.BeginTransactionAsync();

                user1.Value -= transitions.Value;
                user2.Value += transitions.Value;

                var transacao = new TransationsModel
                {
                    Id = Guid.NewGuid(),
                    IDDequemfez = transitions.IDDequemfez,
                    IDDequemrecebeu = transitions.IDDequemrecebeu,
                    Value = transitions.Value,
                    Title = transitions.title,
                    Description = transitions.description,
                    TransationData = DateTime.Now.ToString("dd:MM:yyyy"), // mudar propriedade para DateTime
                };

                _context.Users.Update(user1);
                _context.Users.Update(user2);
                await _context.Transations.AddAsync(transacao);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _transations.Add(transacao);

                return Ok("Transação realizada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a transação.");

                var error = new ErrorModel()
                {
                    IdError = Guid.NewGuid(),
                    MessageError = ex.Message,
                    HttpStatusCode = 500,
                    Date = DateTime.Now
                };

                _context.Errors.Add(error);
                await _context.SaveChangesAsync();

                return StatusCode(500, error);
            }
        }


    }
}