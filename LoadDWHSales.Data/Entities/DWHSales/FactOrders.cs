using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Entities.DWHSales
{
    [Table("FactOrders")]
    public class FactOrders
    {
        [Key]
        public int OrderId { get; set; }

        public string CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public int? DateId { get; set; }

        public int? ProductId { get; set; }

        public int? ShipperId { get; set; }

        public string Country { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalSales { get; set; }

    }
}
