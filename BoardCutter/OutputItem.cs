using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardCutter
{
    class OutputItem
    {
        public OutputItem(Part part)
        {
            Source = part.source + 1;
            Position = part.position + 1;
            Id = part.Id;
            Length = part.Length;
            Width = part.Width;
        }
        public int Source { get; set; }
        public int Position { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public string Id { get; set; }
    }
}
