using FunnyExperience.Server.Data.Repositories.Interfaces;
using FunnyExperience.Server.Models.DatabaseModels;
using FunnyExperience.Server.Models.DTOs.Player;
using FunnyExperience.Server.Services.Interfaces;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task PostStats(UpdateLevelRequest request)
    {
        var player = await _playerRepository.GetPlayerByName(request.PlayerName);
        if (player == null)
        {
            var newPlayer = new Player {
                Name = request.PlayerName, 
                Stats = new PlayerStats {
                    Level = request.Level
                }
            };
            await _playerRepository.AddAsync(newPlayer);
            return;
        }

        player.Stats.Level = request.Level;
    }
}