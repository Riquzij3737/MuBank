using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }


}
