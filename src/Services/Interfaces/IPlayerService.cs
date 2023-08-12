using FunnyExperience.Server.Models.DTOs.Player;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Services.Interfaces;

public interface IPlayerService
{
    Task PostStats(UpdateLevelRequest request);
}