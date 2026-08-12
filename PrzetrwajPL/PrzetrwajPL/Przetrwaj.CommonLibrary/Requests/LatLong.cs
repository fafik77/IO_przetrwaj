using Przetrwaj.CommonLibrary.Extensions;

namespace Przetrwaj.CommonLibrary.Requests;

public record LatLong(double Lat, double Long) : IMultipartFormDataCreator
{
	public double Lat { get; set; } = Lat;
	public double Long { get; set; } = Long;

	public override string ToString()
	{
		return $"(lat={Lat}, long={Long})";
	}

	public MultipartFormDataContent ToMultipartData(MultipartFormDataContent multipartFormData, string? rootPath)
	{
		return multipartFormData
		.AddStringContent(new(nameof(Lat), Lat), rootPath)
		.AddStringContent(new(nameof(Long), Long), rootPath);
	}
}
