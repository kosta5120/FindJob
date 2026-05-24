namespace FindJobAPI.DTOs;

public class JobPostDto
{
    public int Id { get; set; }
    public string PostTitle { get; set; } = "";
    public string NameOfPublisher { get; set; } = "";
    public string? Address { get; set; }
    public string? Region { get; set; }
    public string? Salary { get; set; }
    public string? TypOfJob { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? CategoryPosition { get; set; }
    public string? Subcategory { get; set; }
    public int NumberOfPosition { get; set; }
    public DateTime DateTime { get; set; }
}

public class CreateJobPostDto
{
    public string PostTitle { get; set; } = "";
    public string NameOfPublisher { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Salary { get; set; }
    public int NumberOfPosition { get; set; } = 1;
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public int RegionId { get; set; }
    public int? CityId { get; set; }
    public int JobTypeId { get; set; }
}

public class JobsPagedResult
{
    public List<JobPostDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class JobQueryParams
{
    public string? What { get; set; }
    public string? Where { get; set; }
    public int? Category { get; set; }
    public int? JobType { get; set; }
    public int? Region { get; set; }
    public int? City { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
