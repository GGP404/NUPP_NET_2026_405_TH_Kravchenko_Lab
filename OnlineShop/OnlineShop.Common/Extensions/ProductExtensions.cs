using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShop.Common.Models;

namespace OnlineShop.Common.Extensions
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