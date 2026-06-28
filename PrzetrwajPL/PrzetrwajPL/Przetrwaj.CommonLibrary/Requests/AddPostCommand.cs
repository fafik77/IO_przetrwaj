using Przetrwaj.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class AddPostCommand
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
	public LatLong LatLong { get; set; }


	public RegionPrecision RegionPrecision { get; set; } = RegionPrecision.GMI;


	public AddAttachments? Attachments { get; set; }
}
