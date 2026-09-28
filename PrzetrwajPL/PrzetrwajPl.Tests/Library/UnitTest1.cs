using System.Globalization;
using System.Text.Json;

namespace PrzetrwajPl.Tests.Library;

public class Tests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void DoubleIsStringifiedUsingDot_Success()
	{
		double d = 1.1;
		var str = d.ToString(null, CultureInfo.InvariantCulture);
		Assert.That(str, Is.EquivalentTo("1.1"));
	}

	[Test]
	public void DoubleIsSerializedUsingDot_Success()
	{
		double d = 1.1;
		var str = JsonSerializer.Serialize(d);
		Assert.That(str, Is.EquivalentTo("1.1"));
	}
}
