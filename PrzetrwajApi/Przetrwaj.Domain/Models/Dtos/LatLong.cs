using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models.Dtos;

public record LatLong(
    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90.")]
    double Lat,

    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180.")]
    double Long
)
{
    public override string ToString() => $"(lat={Lat}, long={Long})";
}
