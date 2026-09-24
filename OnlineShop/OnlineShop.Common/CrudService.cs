using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
{
    public class CrudService<T> : ICrudService<T>
        where T : Product
    {
        private List<T> items = new List<T>();

        // Create
        public void Create(T element)
        {
            items.Add(element);
        }

        // Read
        public T Read(Guid id)
        {
            return items.First(x => x.Id == id);
        }

        // Read All
        public IEnumerable<T> ReadAll()
        {
            return items;
        }

        // Update
        public void Update(T element)
        {
            var oldElement = items.First(x => x.Id == element.Id);

            int index = items.IndexOf(oldElement);

            items[index] = element;
        }

        // Remove
        public void Remove(T element)
        {
            items.Remove(element);
        }
    }
}