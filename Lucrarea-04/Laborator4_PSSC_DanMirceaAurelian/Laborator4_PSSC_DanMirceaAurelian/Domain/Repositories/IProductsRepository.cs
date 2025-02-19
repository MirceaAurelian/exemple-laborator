﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator4_PSSC_DanMirceaAurelian.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<List<ProductCode>> TryGetExistingProduct(IEnumerable<string> productsToCheck);
    }
}
