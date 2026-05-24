using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindJobAPI.Models;

public class JobType
{
    [Key]
    [Column("IDTypePosition")]
    public int IdTypePosition { get; set; }

    [Column("jobType")]
    public string JobTypeName { get; set; } = "";
}
