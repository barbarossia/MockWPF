using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public interface ILinqTree<T>
    {
        IEnumerable<T> Children();

        T Parent { get; }
    }
}
