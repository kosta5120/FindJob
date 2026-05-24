using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindJobAPI.Models;

public class BadReport
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("IDPost")]
    public int PostId { get; set; }

    public int NumberOfReports { get; set; }

    public JobPost? Post { get; set; }
}
