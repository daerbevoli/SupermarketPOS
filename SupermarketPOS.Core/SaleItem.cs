using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketPOS.Core
{
    public class SaleItem
    {
        public int Id { get; set; }
        public decimal PriceAtSale { get; set; }

        public int Quantity { get; set; }

        // Foreign key to the Sale
        public int SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        // Foreign key to the Product's Barcode
        public string ProductBarcode { get; set; }
        public virtual Product Product { get; set; }
    }
}