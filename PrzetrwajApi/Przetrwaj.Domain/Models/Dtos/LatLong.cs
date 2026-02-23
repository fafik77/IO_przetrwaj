namespace Przetrwaj.Domain.Models.Dtos;

public class LatLong(double Lat, double Long)
{
	public double Lat { get; set; } = Lat;
	public double Long { get; set; } = Long;
}
