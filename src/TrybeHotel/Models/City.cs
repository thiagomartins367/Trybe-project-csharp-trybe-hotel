namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class City
{
    [Key]
    public int CityId { get; set; }
    public string Name { get; set; } = null!;
    public string State { get; set; } = null!;
    public ICollection<Hotel>? Hotels { get; set; }
}
