using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetCategoryByIdQuery : IQuery<CategoryDto?>
{
    public int IdCategory { get; set; }
}
