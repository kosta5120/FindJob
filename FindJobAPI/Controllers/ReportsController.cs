using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;
using FindJobAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api")]
public class ReportsController : ControllerBase
{
    private readonly FindJobContext _db;

    public ReportsController(FindJobContext db) => _db = db;

    [HttpPost("reports")]
    public async Task<ActionResult> ReportPost([FromBody] ReportRequest req)
    {
        var existing = await _db.BadReports.FirstOrDefaultAsync(r => r.PostId == req.PostId);
        if (existing != null)
        {
            existing.NumberOfReports++;
        }
        else
        {
            _db.BadReports.Add(new BadReport { PostId = req.PostId, NumberOfReports = 1 });
        }
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("admin/reports")]
    [Authorize]
    public async Task<ActionResult> GetReports()
    {
        var reports = await _db.BadReports
            .Include(r => r.Post)
            .Select(r => new
            {
                r.Id,
                r.PostId,
                r.NumberOfReports,
                post = r.Post == null ? null : new
                {
                    r.Post.Id,
                    r.Post.PostTitle,
                    r.Post.NameOfPublisher,
                    r.Post.Email,
                    r.Post.DateTime,
                    r.Post.CategoryPosition,
                    r.Post.Subcategory,
                    r.Post.TypOfJob,
                    r.Post.Address,
                    r.Post.Region,
                    r.Post.Salary,
                    r.Post.Description,
                    r.Post.Experience,
                }
            })
            .ToListAsync();
        return Ok(reports);
    }

    [HttpDelete("admin/reports/{id}")]
    [Authorize]
    public async Task<ActionResult> DismissReport(int id)
    {
        var report = await _db.BadReports.FindAsync(id);
        if (report == null) return NotFound();
        _db.BadReports.Remove(report);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("admin/reports/{id}/with-post")]
    [Authorize]
    public async Task<ActionResult> DeletePostAndReport(int id)
    {
        var report = await _db.BadReports.FindAsync(id);
        if (report == null) return NotFound();

        var post = await _db.JobPosts.FindAsync(report.PostId);
        var allReports = _db.BadReports.Where(r => r.PostId == report.PostId);
        _db.BadReports.RemoveRange(allReports);
        if (post != null) _db.JobPosts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record ReportRequest(int PostId);
