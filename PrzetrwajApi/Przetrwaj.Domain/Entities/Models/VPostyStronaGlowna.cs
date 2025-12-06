namespace Przetrwaj.Domain.Entities.Models;

public partial class VPostyStronaGlowna
{
	public int? IdPost { get; set; }

	public string? Typ { get; set; }

	public string? Tytul { get; set; }

	public string? OpisSkrocony { get; set; }

	public string? Region { get; set; }

	public string? Kategoria { get; set; }

	public string? Autor { get; set; }

	public long? LiczbaPotwierdzen { get; set; }

	public long? LiczbaOdwolan { get; set; }
}
