using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Admin;

public class ServicesModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ServicesModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Service> Services { get; set; } = new();

    [BindProperty]
    public ServiceInput Input { get; set; } = new();

    public string FormTitle => Input.Id == 0 ? "Добавить услугу" : $"Редактирование услуги #{Input.Id}";

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var gate = EnsureAdmin();
        if (gate != null) return gate;

        Load();
        return Page();
    }

    public IActionResult OnPostEdit(int serviceId)
    {
        var gate = EnsureAdmin();
        if (gate != null) return gate;

        Load();
        var s = _db.Services.FirstOrDefault(x => x.Id == serviceId);
        if (s == null) return RedirectToPage();

        Input = new ServiceInput
        {
            Id = s.Id,
            Name = s.Name,
            Type = s.Type,
            Price = s.Price,
            IsSpecialOffer = s.IsSpecialOffer,
            SpecialDescription = s.SpecialDescription
        };

        return Page();
    }

    public IActionResult OnPostCancel()
    {
        var gate = EnsureAdmin();
        if (gate != null) return gate;

        return RedirectToPage();
    }

    public IActionResult OnPostSave()
    {
        var gate = EnsureAdmin();
        if (gate != null) return gate;

        Load();

        if (!ModelState.IsValid)
            return Page();

        if (Input.Id == 0)
        {
            _db.Services.Add(new Service
            {
                Name = Input.Name.Trim(),
                Type = Input.Type.Trim(),
                Price = Input.Price,
                IsSpecialOffer = Input.IsSpecialOffer,
                SpecialDescription = string.IsNullOrWhiteSpace(Input.SpecialDescription) ? null : Input.SpecialDescription.Trim()
            });
            _db.SaveChanges();
            SuccessMessage = "Услуга добавлена.";
        }
        else
        {
            var s = _db.Services.FirstOrDefault(x => x.Id == Input.Id);
            if (s == null)
            {
                ErrorMessage = "Услуга не найдена.";
                return Page();
            }

            s.Name = Input.Name.Trim();
            s.Type = Input.Type.Trim();
            s.Price = Input.Price;
            s.IsSpecialOffer = Input.IsSpecialOffer;
            s.SpecialDescription = string.IsNullOrWhiteSpace(Input.SpecialDescription) ? null : Input.SpecialDescription.Trim();
            _db.SaveChanges();
            SuccessMessage = "Услуга обновлена.";
        }

        Load();
        return Page();
    }

    public IActionResult OnPostDelete(int serviceId)
    {
        var gate = EnsureAdmin();
        if (gate != null) return gate;

        Load();

        int hasSubscriptions = _db.Subscriptions.Count(s => s.ServiceId == serviceId);
        if (hasSubscriptions > 0)
        {
            ErrorMessage = "Невозможно удалить услугу: по ней оформлены абонементы.";
            return Page();
        }

        var s = _db.Services.FirstOrDefault(x => x.Id == serviceId);
        if (s != null)
        {
            _db.Services.Remove(s);
            _db.SaveChanges();
            SuccessMessage = "Услуга удалена.";
        }

        Load();
        return Page();
    }

    private IActionResult? EnsureAdmin()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Account/Login", new { role = Roles.Admin });

        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login", new { role = Roles.Admin });

        return null;
    }

    private void Load()
    {
        Services = _db.Services.OrderBy(s => s.Id).ToList();
    }

    public class ServiceInput
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите тип.")]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [Range(0, 100000000, ErrorMessage = "Введите корректную цену.")]
        public decimal Price { get; set; }

        public bool IsSpecialOffer { get; set; }

        [StringLength(500)]
        public string? SpecialDescription { get; set; }
    }
}

