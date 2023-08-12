using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunnyExperience.Server.Models.DatabaseModels;

[Table("User")]
public class User : IdentityUser<Guid>
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(50)]
    public string LastName { get; set; }
    [MaxLength(50)]
    public string DiscordId { get; set; }
}