using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Entities.Northwind
{
    public class VwDate
    {
        public int? DateKey { get; set; }

        public DateOnly? FullDate { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

    }
}
