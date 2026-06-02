using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public RegisterModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public void OnGet(string? role)
    {
        Input.Role = NormalizeRole(role) ?? Roles.Client;
    }

    public IActionResult OnPost()
    {
        Input.Role = NormalizeRole(Input.Role) ?? Roles.Client;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_context.Users.Any(u => u.Username == Input.Username))
        {
            ModelState.AddModelError(string.Empty, "Пользователь с таким логином уже существует.");
            return Page();
        }

        var user = new User
        {
            Username = Input.Username.Trim(),
            Password = Input.Password,
            Role = Input.Role
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        if (string.Equals(user.Role, Roles.Client, StringComparison.OrdinalIgnoreCase))
        {
            var client = new WebApplication1.Models.FitnessClub.Client
            {
                UserId = user.Id,
                FullName = string.IsNullOrWhiteSpace(Input.FullName) ? user.Username : Input.FullName.Trim(),
                CompletedSubscriptions = 0
            };
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role);

        if (string.Equals(user.Role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Admin/Services");

        return RedirectToPage("/Client/Services");
    }

    public class RegisterInputModel : IValidatableObject
    {
        [Required(ErrorMessage = "Введите логин.")]
        [StringLength(100, ErrorMessage = "Максимальная длина логина — 100 символов.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Длина пароля должна быть от 4 до 100 символов.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = Roles.Client;

        public string? FullName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.Equals(Role, Roles.Client, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("Некорректная роль.", new[] { nameof(Role) });
            }
        }
    }

    private static string? NormalizeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return null;
        role = role.Trim();
        if (string.Equals(role, Roles.Client, StringComparison.OrdinalIgnoreCase)) return Roles.Client;
        if (string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase)) return Roles.Admin;
        return null;
    }
}

