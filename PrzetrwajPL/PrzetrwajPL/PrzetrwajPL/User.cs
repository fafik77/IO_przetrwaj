namespace PrzetrwajPL;

public class User
{
	public string Id { get; set; }
	public string Email { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public string Role { get; set; }
	public bool TwoFactorEnabled { get; set; }
	public bool Banned { get; set; }
	public string BanReason { get; set; }

	public BannedByUser BannedBy { get; set; }
}
public class BannedByUser
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
}
