using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Client;

public class MySubscriptionsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public MySubscriptionsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public WebApplication1.Models.FitnessClub.Client? Client { get; set; }
    public List<Row> Rows { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.Equals(role, Roles.Client, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Account/Login", new { role = Roles.Client });

        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Account/Login", new { role = Roles.Client });

        Client = _db.Clients.FirstOrDefault(c => c.UserId == userId.Value);
        if (Client == null)
        {
            ErrorMessage = "Профиль клиента не найден.";
            return Page();
        }

        Rows = (from sub in _db.Subscriptions
                join s in _db.Services on sub.ServiceId equals s.Id
                where sub.ClientId == Client.Id
                orderby sub.StartDate descending
                select new Row
                {
                    ServiceName = s.Name,
                    Type = s.Type,
                    StartDate = sub.StartDate,
                    EndDate = sub.EndDate,
                    OriginalPrice = sub.OriginalPrice,
                    FinalPrice = sub.FinalPrice,
                    DiscountPercent = sub.DiscountPercent
                }).ToList();

        return Page();
    }

    public sealed class Row
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}

