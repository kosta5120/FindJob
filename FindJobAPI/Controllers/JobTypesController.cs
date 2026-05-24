using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/job-types")]
public class JobTypesController : ControllerBase
{
    private readonly FindJobContext _db;

    public JobTypesController(FindJobContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var types = await _db.JobTypes
            .Select(t => new { t.IdTypePosition, t.JobTypeName })
            .ToListAsync();
        return Ok(types);
    }
}
