using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardCutter
{
    class Part
    {
        public enum PartStatus
        {
            Unused,     // initial state
            Placed,     // done finally
            Proposed,   // under evaluation
            Current,    // under evaluation
            Impossible  // not possible, so we will skip.
        }
        private static int _instance_ = 0;
        public bool NotDone
        { get { return (status != PartStatus.Placed) && (status != PartStatus.Impossible); } }
        public Part(string id, double length, double width = 0)
        {
            Instance = _instance_++;
            Id = id;
            Length = length;
            Width = width;
            status = PartStatus.Unused;
        }
        public int Instance { get; set; }   // unique serial number - matches the Parts[] index.
        public string Id { get; set; }      // the group Id
        public double Length { get; set; }  // length in inches (or units)
        public double Width { get; set; }   // width can be zero
        public int position { get; set; }   // position on the board
        public int source { get; set; }     // which source board
        public string NOfN { get; set; }    // use for "number x of y" labeling
        public PartStatus status { get; set; }
    }
}
