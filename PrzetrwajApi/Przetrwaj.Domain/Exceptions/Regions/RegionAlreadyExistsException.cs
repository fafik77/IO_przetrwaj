using System.Globalization;

namespace Przetrwaj.Domain.Exceptions;

public class RegionAlreadyExistsException(string identity) : AlreadyExistsException<RegionInfo>(identity)
{ }
