using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WeatherBar.Core
{
    public class QueryResultContainer<T> : ICollection<T>
    {
        #region Fields

        private readonly List<KeyValuePair<int, T>> container;

        private int counter = 0;

        public int Count => throw new System.NotImplementedException();

        public bool IsReadOnly => throw new System.NotImplementedException();

        #endregion

        #region Constructor

        public QueryResultContainer()  
        {
            container = new List<KeyValuePair<int, T>>();
        }

        #endregion

        #region Public methods

        public void Add(T item)
        {
            container.Add(new KeyValuePair<int, T>(counter++, item));
        }

        public void Clear()
        {
            container.Clear();
        }

        public bool Contains(T item)
        {
            return container.Any(x => x.Value.Equals(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            container.Select(x => x.Value).ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return container.Any(x => x.Value.Equals(item)) && container.Remove(container.First(x => x.Value.Equals(item)));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return container.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return container.GetEnumerator();
        }

        #endregion
    }
}
