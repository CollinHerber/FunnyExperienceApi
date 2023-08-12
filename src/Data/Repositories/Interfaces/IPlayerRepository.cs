using FunnyExperience.Server.Models.DatabaseModels;

namespace FunnyExperience.Server.Data.Repositories.Interfaces;

public interface IPlayerRepository : IBaseRepository<Player>
{
    Task<Player?> GetPlayerByName(string name);
}