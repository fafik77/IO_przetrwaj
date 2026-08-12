using Przetrwaj.CommonLibrary.Extensions;
using Przetrwaj.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class AddPostCommand : IMultipartFormDataCreator
{
	[Required(ErrorMessage = "Tytuł jest wymagany")]
	[Length(3, 200, ErrorMessage = "Tytuł musi mieć od 3 do 200 znaków")]
	public required string Title { get; set; } = "";

	[MaxLength(2000, ErrorMessage = "Opis nie może być dłuższy niż 2'000 znaków")]
	public string? Description { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Kategoria jest wymagana")]
	public int IdCategory { get; set; }

	[Length(3, 100, ErrorMessage = "Własna kategoria musi mieć od 3 do 100 znaków")]
	public string? CustomCategory { get; set; }

	[Required(ErrorMessage = "Lokalizacja zdarzenia jest wymagana")]
	public LatLong LatLong { get; set; } = new(0, 0);


	public RegionPrecision RegionPrecision { get; set; } = RegionPrecision.GMI;


	public AddAttachments? Attachments { get; set; }

	public MultipartFormDataContent ToMultipartData(MultipartFormDataContent? multipartFormData, string? rootPath = null)
	{
		if (multipartFormData == null) multipartFormData = new();
		return multipartFormData
			.AddStringContent(new(nameof(Title), Title), rootPath)
			.AddStringContent(new(nameof(Description), Description), rootPath)
			.AddStringContent(new(nameof(IdCategory), IdCategory), rootPath)
			.AddStringContent(new(nameof(CustomCategory), CustomCategory), rootPath)
			.AddContent(nameof(LatLong), LatLong, rootPath)
			.AddStringContent(new(nameof(RegionPrecision), RegionPrecision), rootPath)
			.AddContent(nameof(Attachments), Attachments, rootPath);
	}
}
