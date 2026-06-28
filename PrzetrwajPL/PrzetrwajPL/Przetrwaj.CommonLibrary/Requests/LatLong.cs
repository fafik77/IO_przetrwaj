namespace Przetrwaj.CommonLibrary.Requests;

public record LatLong(double Lat, double Long)
{
	public double Lat { get; set; } = Lat;
	public double Long { get; set; } = Long;

	public override string ToString()
	{
		return $"(lat={Lat}, long={Long})";
	}
}
