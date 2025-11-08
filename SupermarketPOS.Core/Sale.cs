using System;
using System.Collections.Generic;

namespace SupermarketPOS.Core
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation property: A Sale has many SaleItems
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}