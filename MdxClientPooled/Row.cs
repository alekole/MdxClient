using System.Collections.Generic;

namespace MdxClient
{
    internal class Row
    {
        public List<Cell> Cells { get; set; }

        public Row()
        {
            Cells = new List<Cell>();
        }
    }
}
