using Microsoft.EntityFrameworkCore;
using FindJobAPI.Data;

namespace FindJobAPI;

public class PostCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<PostCleanupService> _logger;

    public PostCleanupService(IServiceProvider services, ILogger<PostCleanupService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanOldPosts();
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }

    private async Task CleanOldPosts()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FindJobContext>();

        var cutoff = DateTime.Now.AddDays(-14);
        var oldPosts = await db.JobPosts.Where(p => p.DateTime < cutoff).ToListAsync();

        if (oldPosts.Count == 0) return;

        var oldIds = oldPosts.Select(p => p.Id).ToList();
        var reports = db.BadReports.Where(r => oldIds.Contains(r.PostId));
        db.BadReports.RemoveRange(reports);
        db.JobPosts.RemoveRange(oldPosts);
        await db.SaveChangesAsync();

        _logger.LogInformation("Deleted {count} expired job posts.", oldPosts.Count);
    }
}
