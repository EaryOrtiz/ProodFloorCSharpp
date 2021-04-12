using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class EFWiringRepository : IWiringRepository
    {
        private ApplicationDbContext context;

        public EFWiringRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

    }
}
