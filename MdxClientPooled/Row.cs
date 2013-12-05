using System.Collections.Generic;

namespace MdxClientPooled
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
