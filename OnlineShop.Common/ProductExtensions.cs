using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
{
    public static class ProductExtensions
    {
        // Extension method
        public static string ShortInfo(this Product product)
        {
            return $"{product.Name} - {product.Price} грн";
        }
    }
}