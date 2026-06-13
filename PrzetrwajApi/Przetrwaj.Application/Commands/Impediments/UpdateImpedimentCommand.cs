using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Impediments;

public class UpdateImpedimentCommand : EditImpediment, ICommand<Impediment> { }
