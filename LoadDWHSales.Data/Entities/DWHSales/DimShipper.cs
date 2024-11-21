using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Entities.DWHSales
{
    [Table("DimShipper")]

    public class DimShipper
    {
        [Key]
        public int ShipperID { get; set; }
        public string? ShipperName { get; set; }
    }
}
