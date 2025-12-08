using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(int idCategory, CancellationToken cancellationToken);
}