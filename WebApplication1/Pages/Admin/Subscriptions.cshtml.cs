using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Pages.Admin;

public class SubscriptionsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public SubscriptionsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Row> Rows { get; set; } = new();

    public List<ServiceItem> Services { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? ServiceId { get; set; }

    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            return RedirectToPage("/Account/Login", new { role = Roles.Admin });

        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login", new { role = Roles.Admin });

        Services = _db.Services
            .OrderBy(s => s.Name)
            .Select(s => new ServiceItem { Id = s.Id, Name = s.Name })
            .ToList();

        var query =
            from sub in _db.Subscriptions
            join c in _db.Clients on sub.ClientId equals c.Id
            join s in _db.Services on sub.ServiceId equals s.Id
            select new { sub, c, s };

        if (ServiceId is > 0)
        {
            query = query.Where(x => x.s.Id == ServiceId.Value);
        }

        Rows = query
            .OrderByDescending(x => x.sub.StartDate)
            .Select(x => new Row
            {
                Id = x.sub.Id,
                ClientName = x.c.FullName,
                ServiceName = x.s.Name,
                StartDate = x.sub.StartDate,
                EndDate = x.sub.EndDate,
                FinalPrice = x.sub.FinalPrice
            })
            .ToList();

        return Page();
    }

    public sealed class Row
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public sealed class ServiceItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

