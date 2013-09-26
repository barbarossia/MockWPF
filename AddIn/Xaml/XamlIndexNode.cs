using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class XamlIndexNode
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public int Parent { get; set; }
    }
}
