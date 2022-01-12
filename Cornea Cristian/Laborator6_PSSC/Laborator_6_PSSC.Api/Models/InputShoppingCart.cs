using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Laborator_6_PSSC.Domain.Models;

namespace Laborator_6_PSSC.Api.Models
{
    public class InputShoppingCart
    {
        [Required]
        [RegularExpression(ProductCode.Pattern)]
        public string _ProductCode { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int _Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int _OrderId { get; set; }

        [Required]
	    [RegularExpression(Address.Pattern)]
        public string _Address { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal _Price { get; set; }
    }
}
