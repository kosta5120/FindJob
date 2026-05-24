using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using FindJobAPI.Data;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplyController : ControllerBase
{
    private readonly FindJobContext _db;
    private readonly IConfiguration _config;

    public ApplyController(FindJobContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost]
    public async Task<ActionResult> Apply([FromForm] ApplyRequest req)
    {
        var post = await _db.JobPosts.FindAsync(req.JobId);
        if (post == null) return NotFound("Job not found");
        if (string.IsNullOrWhiteSpace(post.Email)) return BadRequest("No contact email for this job");

        var smtpHost = _config["Email:Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_config["Email:Port"] ?? "587");
        var smtpUser = _config["Email:User"] ?? "";
        var smtpPass = _config["Email:Password"] ?? "";

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpUser, "FindJob"),
            Subject = $"מועמדות למשרת {post.PostTitle}",
            Body = req.Letter,
            IsBodyHtml = false,
        };
        mail.To.Add(post.Email);

        if (req.Cv != null && req.Cv.Length > 0)
        {
            var stream = req.Cv.OpenReadStream();
            mail.Attachments.Add(new Attachment(stream, req.Cv.FileName, req.Cv.ContentType));
        }

        await client.SendMailAsync(mail);
        return Ok(new { message = "Application sent successfully" });
    }
}

public class ApplyRequest
{
    public int JobId { get; set; }
    public string Letter { get; set; } = "";
    public IFormFile? Cv { get; set; }
}
