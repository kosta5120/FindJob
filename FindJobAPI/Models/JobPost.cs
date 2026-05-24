using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindJobAPI.Models;

public class JobPost
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("Post_Title")]
    public string PostTitle { get; set; } = "";

    [Column("Name_of_Publisher")]
    public string NameOfPublisher { get; set; } = "";

    public string? Address { get; set; }
    public string? Region { get; set; }
    public string? Salary { get; set; }

    [Column("Typ_of_job")]
    public string? TypOfJob { get; set; }

    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? CategoryPosition { get; set; }
    public string? Subcategory { get; set; }
    public int NumberOfPosition { get; set; }
    public DateTime DateTime { get; set; }
}
