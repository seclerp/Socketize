using System.Collections;
using System.Collections.Generic;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Represents schema information.
    /// </summary>
    public class Schema : IReadOnlyCollection<SchemaItem>
    {
        private readonly IReadOnlyCollection<SchemaItem> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        /// <param name="items">Readonly collection of schema items.</param>
        public Schema(IReadOnlyCollection<SchemaItem> items)
        {
            _items = items;
        }

        /// <inheritdoc />
        public int Count => _items.Count;

        /// <inheritdoc />
        public IEnumerator<SchemaItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}