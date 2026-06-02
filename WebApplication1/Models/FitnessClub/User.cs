using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.FitnessClub;

public class User
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Role { get; set; } = Roles.Client;
}

