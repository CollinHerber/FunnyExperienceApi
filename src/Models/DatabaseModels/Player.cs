using FunnyExperience.Server.Models.DatabaseModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunnyExperience.Server.Models.DatabaseModels;

[Table("Player")]
public class Player : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; set; }
    public int Experience { get; set; }
    [MaxLength(50)]
    public string PlayerId { get; set; }
    [Column(TypeName = "json")]
    public PlayerStats Stats { get; set; }
}

public class PlayerStats
{
    public int Level { get; set; }
}