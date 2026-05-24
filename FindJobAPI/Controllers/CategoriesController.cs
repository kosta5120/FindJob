using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly FindJobContext _db;

    public CategoriesController(FindJobContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var cats = await _db.Categories
            .Select(c => new { c.Id, c.CategoryWork })
            .ToListAsync();
        return Ok(cats);
    }

    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult> GetSubcategories(int id)
    {
        var subs = await _db.Subcategories
            .Where(s => s.CategoryId == id)
            .Select(s => new { s.Id, subcategory = s.SubcategoryName })
            .ToListAsync();
        return Ok(subs);
    }
}
