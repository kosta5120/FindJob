using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;
using FindJobAPI.DTOs;
using FindJobAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly FindJobContext _db;

    public JobsController(FindJobContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<JobsPagedResult>> GetJobs([FromQuery] JobQueryParams q)
    {
        var query = _db.JobPosts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.What))
            query = query.Where(j =>
                j.PostTitle.Contains(q.What) ||
                j.Description!.Contains(q.What) ||
                j.CategoryPosition!.Contains(q.What) ||
                j.Subcategory!.Contains(q.What));

        if (!string.IsNullOrWhiteSpace(q.Where))
            query = query.Where(j =>
                j.Address!.Contains(q.Where) ||
                j.Region!.Contains(q.Where));

        if (q.Category.HasValue)
        {
            var cat = await _db.Categories.FindAsync(q.Category.Value);
            if (cat != null) query = query.Where(j => j.CategoryPosition == cat.CategoryWork);
        }

        if (q.JobType.HasValue)
        {
            var jt = await _db.JobTypes.FindAsync(q.JobType.Value);
            if (jt != null) query = query.Where(j => j.TypOfJob == jt.JobTypeName);
        }

        if (q.Region.HasValue)
        {
            var reg = await _db.Regions.FindAsync(q.Region.Value);
            if (reg != null) query = query.Where(j => j.Region == reg.RegionName);
        }

        if (q.City.HasValue)
        {
            var city = await _db.Cities.FindAsync(q.City.Value);
            if (city != null) query = query.Where(j => j.Address == city.CityName);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(j => j.DateTime)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(j => new JobPostDto
            {
                Id = j.Id,
                PostTitle = j.PostTitle,
                NameOfPublisher = j.NameOfPublisher,
                Address = j.Address,
                Region = j.Region,
                Salary = j.Salary,
                TypOfJob = j.TypOfJob,
                Email = j.Email,
                Description = j.Description,
                Experience = j.Experience,
                CategoryPosition = j.CategoryPosition,
                Subcategory = j.Subcategory,
                NumberOfPosition = j.NumberOfPosition,
                DateTime = j.DateTime,
            })
            .ToListAsync();

        return Ok(new JobsPagedResult { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize });
    }

    [HttpPost]
    public async Task<ActionResult> CreateJob([FromBody] CreateJobPostDto dto)
    {
        var category = await _db.Categories.FindAsync(dto.CategoryId);
        var region = await _db.Regions.FindAsync(dto.RegionId);
        var jobType = await _db.JobTypes.FindAsync(dto.JobTypeId);
        var city = dto.CityId.HasValue ? await _db.Cities.FindAsync(dto.CityId.Value) : null;
        var subcategory = dto.SubcategoryId.HasValue ? await _db.Subcategories.FindAsync(dto.SubcategoryId.Value) : null;

        if (category == null || region == null || jobType == null)
            return BadRequest("Invalid category, region, or job type.");

        var post = new JobPost
        {
            PostTitle = dto.PostTitle,
            NameOfPublisher = dto.NameOfPublisher,
            Email = dto.Email,
            Description = dto.Description,
            Experience = dto.Experience,
            Salary = dto.Salary,
            NumberOfPosition = dto.NumberOfPosition,
            CategoryPosition = category.CategoryWork,
            Subcategory = subcategory?.SubcategoryName,
            Region = region.RegionName,
            Address = city?.CityName ?? region.RegionName,
            TypOfJob = jobType.JobTypeName,
            DateTime = DateTime.Now,
        };

        _db.JobPosts.Add(post);
        await _db.SaveChangesAsync();
        return Ok(new { post.Id });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteJob(int id)
    {
        var post = await _db.JobPosts.FindAsync(id);
        if (post == null) return NotFound();

        var reports = _db.BadReports.Where(r => r.PostId == id);
        _db.BadReports.RemoveRange(reports);
        _db.JobPosts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
