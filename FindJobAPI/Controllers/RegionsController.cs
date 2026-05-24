using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly FindJobContext _db;

    public RegionsController(FindJobContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var regions = await _db.Regions
            .Select(r => new { r.IdRegion, region = r.RegionName })
            .ToListAsync();
        return Ok(regions);
    }

    [HttpGet("{id}/cities")]
    public async Task<ActionResult> GetCities(int id)
    {
        var cities = await _db.Cities
            .Where(c => c.RegionId == id)
            .Select(c => new { c.IdCity, city = c.CityName })
            .ToListAsync();
        return Ok(cities);
    }
}
