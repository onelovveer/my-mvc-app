using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.FitnessClub;

public class Service
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [Column(TypeName = "boolean")]
    public bool IsSpecialOffer { get; set; }

    [MaxLength(500)]
    public string? SpecialDescription { get; set; }
}

