using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator4_PSSC.Models
{
    public class OrderHeaderDto
    {
        public int OrderId { get; set; }
        public string Address { get; set; }
        public decimal Total { get; set; }
    }
}
