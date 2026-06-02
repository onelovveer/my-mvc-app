using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Account;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public LoginModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

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



        var user = _context.Users.FirstOrDefault(u =>
            u.Username == Input.Username && u.Password == Input.Password && u.Role == Input.Role);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин, пароль или роль.");
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role);

        if (string.Equals(user.Role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Admin/Services");

        return RedirectToPage("/Client/Services");
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Введите логин.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = Roles.Client;
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

