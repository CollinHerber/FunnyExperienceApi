using AutoMapper;
using AutoMapper.QueryableExtensions;
using FunnyExperience.Server.Data.Repositories.Base;
using FunnyExperience.Server.Data.Repositories.Interfaces;
using FunnyExperience.Server.Models;
using FunnyExperience.Server.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using FunnyExperience.Server.Lib;
using FunnyExperience.Server.Lib.Extensions;

namespace FunnyExperience.Server.Data.Repositories;

public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    public PlayerRepository(FunnyExperienceDbContext db, IMapper mapper) : base(db, mapper)
    {
        EntityName = "Player";
    }

    public async Task<Player?> GetPlayerByName(string name)
    {
        return await Table.Where(x => x.Name == name).FirstOrDefaultAsync();
    }
}