using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LEIUTAD.Models;

namespace LEIUTAD.Data
{
    public class LEIUTADContext : DbContext
    {
        public LEIUTADContext (DbContextOptions<LEIUTADContext> options)
            : base(options)
        {
        }

        public DbSet<LEIUTAD.Models.Leitor> Leitor { get; set; } = default!;
    }
}
