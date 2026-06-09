using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class AddPostCommand
{
	[Required(ErrorMessage = "Tytuł jest wymagany")]
	[MaxLength(250, ErrorMessage = "Tytuł nie może być dłuższy niż 250 znaków")]
	public required string Title { get; set; } = "";
	[MaxLength(2500, ErrorMessage = "Opis nie może być dłuższy niż 250, znaków")]
	public string? Description { get; set; }
	[Range(1, int.MaxValue, ErrorMessage = "Kategoria jest wymagana")]
	public int IdCategory { get; set; }
	[Length(3, 60, ErrorMessage = "Własna kategoria musi mieć od 3 do 60 znaków")]
	public string? CustomCategory { get; set; }
	[Range(0, int.MaxValue, ErrorMessage = "Region jest wymagany")]
	public int IdRegion { get; set; }
}
