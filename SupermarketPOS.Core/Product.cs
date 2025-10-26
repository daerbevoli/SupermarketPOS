

namespace SupermarketPOS.Core
{
    public class Product
    {
        public int id
        {
            get; set;
        }

        public string barcode
        {
            get; set;
        } = string.Empty;

        public string name
        {
            get; set;
        } = string.Empty;

        public decimal price
        {
            get; set;
        }

        public bool hasPromo
        {
            get; set;
        }

    }
}

