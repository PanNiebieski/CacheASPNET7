using CacheASPNET7.Model;
using CacheASPNET7.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CacheASPNET7WithController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {

        private readonly ILogger<GamesController> _logger;
        private readonly IGameRepository _repository;

        public GamesController(ILogger<GamesController> logger
            , IGameRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        [OutputCache()]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _repository.GetAsync(id);

            if (game is null)
                return NotFound();

            return Ok(game);
        }
    }
}