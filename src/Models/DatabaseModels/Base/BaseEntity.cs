using System;
using System.ComponentModel.DataAnnotations;

namespace FunnyExperience.Server.Models.DatabaseModels.Base;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}