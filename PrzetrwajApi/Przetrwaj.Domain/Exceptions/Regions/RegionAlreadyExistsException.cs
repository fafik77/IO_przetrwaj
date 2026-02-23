using Przetrwaj.Domain.Exceptions._base;
using System.Globalization;

namespace Przetrwaj.Domain.Exceptions.Regions;

public class RegionAlreadyExistsException(string identity) : AlreadyExistsException<RegionInfo>(identity)
{ }
