using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindJobAPI.Models;

public class Region
{
    [Key]
    [Column("idRegion")]
    public int IdRegion { get; set; }

    [Column("Region")]
    public string RegionName { get; set; } = "";

    public ICollection<City> Cities { get; set; } = [];
}

public class City
{
    [Key]
    [Column("idCity")]
    public int IdCity { get; set; }

    [Column("idRegion")]
    public int RegionId { get; set; }

    [Column("City")]
    public string CityName { get; set; } = "";

    public Region? Region { get; set; }
}
