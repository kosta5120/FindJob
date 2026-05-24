using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindJobAPI.Models;

public class Category
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public string CategoryWork { get; set; } = "";

    public ICollection<Subcategory> Subcategories { get; set; } = [];
}

public class Subcategory
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("IDCategoryWork")]
    public int CategoryId { get; set; }

    [Column("Subcategory")]
    public string SubcategoryName { get; set; } = "";

    public Category? Category { get; set; }
}
