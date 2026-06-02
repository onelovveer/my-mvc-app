using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.FitnessClub;

public class Client
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public int CompletedSubscriptions { get; set; }
}

