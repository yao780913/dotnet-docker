using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using myWebApp.Data;

namespace myWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly SchoolContext _context;

    public IndexModel (ILogger<IndexModel> logger, Data.SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    public string StudentName { get; private set; } = "PageModel in C#";

    public void OnGet ()
    {
        var s = _context.Students.FirstOrDefault(d => d.Id == 1);
        if (s != null)
            this.StudentName = $"{s.FirstName} {s.LastName}";
    }
}