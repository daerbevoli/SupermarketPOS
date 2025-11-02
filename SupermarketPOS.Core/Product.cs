

namespace SupermarketPOS.Core
{
    public class Product
    {
        public int Id
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        } = string.Empty;

        public string Naam
        {
            get; set;
        } = string.Empty;

        public decimal Prijs
        {
            get; set;
        }

        public string Eenheid
        {
            get; set;
        } = string.Empty;

        public string Categorie
        {
            get; set;
        } = string.Empty;

        public int Voorraad
        {
            get; set;
        }

    }
}

