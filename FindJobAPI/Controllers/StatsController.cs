using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/admin/stats")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly FindJobContext _db;

    public StatsController(FindJobContext db) => _db = db;

    [HttpGet("monthly")]
    public async Task<ActionResult> GetMonthly()
    {
        var data = await _db.JobPosts
            .GroupBy(j => j.DateTime.Month)
            .Select(g => new { month = g.Key, count = g.Count() })
            .OrderBy(x => x.month)
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("cities")]
    public async Task<ActionResult> GetCities()
    {
        var data = await _db.JobPosts
            .Where(j => j.Address != null)
            .GroupBy(j => j.Address)
            .Select(g => new { city = g.Key, count = g.Count() })
            .OrderByDescending(x => x.count)
            .Take(20)
            .ToListAsync();
        return Ok(data);
    }
}
