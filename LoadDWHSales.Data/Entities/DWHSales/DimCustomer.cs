using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Entities.DWHSales
{
    [Table("DimCustomer")]

    public class DimCustomer
    {
        [Key]
        public string? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
    }
}
