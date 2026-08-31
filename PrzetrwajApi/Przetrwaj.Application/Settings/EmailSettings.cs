namespace Przetrwaj.Application.Settings;

public class EmailSettings
{
	public required EmailSettingsAzure Azure { get; set; }
	public required EmailSettingsCustomSmtp CustomSmtp { get; set; }
}

public class EmailSettingsAzure
{
	public string? AzureConnection { get; set; }
	public string? AzureSender { get; set; }

	public bool IsConfigured => !string.IsNullOrWhiteSpace(AzureConnection) && !string.IsNullOrWhiteSpace(AzureSender);
}

public class EmailSettingsCustomSmtp
{
	public string? SmtpServer { get; set; }
	public int SmtpPort { get; set; } = 465;
	public bool SSL { get; set; } = true;
	public string? UserEmail { get; set; }
	public string? UserPassword { get; set; }
	public string? SenderDisplayName { get; set; }
	public string? Domain { get; set; }

	public bool IsConfigured => !string.IsNullOrWhiteSpace(SmtpServer) && !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);
}