using FunnyExperience.Server.Models.DTOs.Player;
using FunnyExperience.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost("Stats")]
    [AllowAnonymous]
    public async Task<IActionResult> PostStats([FromBody]UpdateLevelRequest request)
    {
        await _playerService.PostStats(request);
        return Ok();
    }
}