using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Entities.DWHSales
{
    [Table("FactServedCustomers")]

    public class FactServedCustomers
    {
        [Key]
        public int ClienteAtendidoId { get; set; }

        public int? EmployeeId { get; set; }

        public int? TotalClientes { get; set; }
    }
}
