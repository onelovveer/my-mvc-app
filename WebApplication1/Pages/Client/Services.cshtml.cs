using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Client;

public class ServicesModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ServicesModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Service> Services { get; set; } = new();

    [BindProperty]
    public PurchaseInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        var gate = EnsureClient();
        if (gate != null) return gate;

        LoadServices();
        return Page();
    }

    public IActionResult OnPost()
    {
        var gate = EnsureClient();
        if (gate != null) return gate;

        LoadServices();

        if (!ModelState.IsValid)
            return Page();

        int userId = HttpContext.Session.GetInt32("UserId")!.Value;
        int? clientId = _db.Clients.Where(c => c.UserId == userId).Select(c => (int?)c.Id).FirstOrDefault();
        if (!clientId.HasValue)
        {
            ErrorMessage = "Профиль клиента не найден.";
            return Page();
        }

        var service = _db.Services.FirstOrDefault(s => s.Id == Input.ServiceId);
        if (service == null)
        {
            ErrorMessage = "Услуга не найдена.";
            return Page();
        }

        decimal originalPrice = service.Price * Input.Months;
        decimal discountPercent = GetDiscountPercent(clientId.Value, Input.IsProlongation);
        decimal discountAmount = Math.Round(originalPrice * (discountPercent / 100m), 2);
        decimal finalPrice = originalPrice - discountAmount;

        DateTime startDate = DateTime.Today;
        DateTime endDate = startDate.AddMonths(Input.Months);

        _db.Subscriptions.Add(new Subscription
        {
            ClientId = clientId.Value,
            ServiceId = service.Id,
            StartDate = startDate,
            EndDate = endDate,
            OriginalPrice = originalPrice,
            FinalPrice = finalPrice,
            DiscountPercent = discountPercent
        });

        if (Input.IsProlongation)
        {
            var client = _db.Clients.First(c => c.Id == clientId.Value);
            client.CompletedSubscriptions += 1;
        }

        _db.SaveChanges();

        SuccessMessage =
            $"Абонемент оформлен. Сумма: {originalPrice:N2} руб. Скидка: {discountPercent:N0}%. К оплате: {finalPrice:N2} руб.";

        return Page();
    }

    private void LoadServices()
    {
        Services = _db.Services.OrderBy(s => s.Id).ToList();
        if (Services.Count > 0 && Input.ServiceId == 0)
            Input.ServiceId = Services[0].Id;
    }

    private IActionResult? EnsureClient()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.Equals(role, Roles.Client, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Account/Login", new { role = Roles.Client });

        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login", new { role = Roles.Client });

        return null;
    }

    // Копируем логику WinForms (Database.GetDiscountPercent)
    private decimal GetDiscountPercent(int clientId, bool isProlongation)
    {
        if (!isProlongation) return 0m;
        var completed = _db.Clients.Where(c => c.Id == clientId).Select(c => c.CompletedSubscriptions).FirstOrDefault();
        if (completed > 0) return 10m;
        return 5m;
    }

    public class PurchaseInput
    {
        [Required]
        public int ServiceId { get; set; }

        [Range(1, 36, ErrorMessage = "Укажите срок от 1 до 36 месяцев.")]
        public int Months { get; set; } = 1;

        public bool IsProlongation { get; set; }
    }
}

