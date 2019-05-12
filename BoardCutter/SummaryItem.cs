using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardCutter
{
    class SummaryItem
    {
        private int _board;
        public int Board
        {
            get { return _board; }
            set { _board = value; }
        }
        private string _cutList = string.Empty;
        public string Cutlist
        {
            get { return _cutList; }
            set { _cutList = value; }
        }
        private double _scrap;
        public double Scrap
        {
            get { return _scrap; }
            set { _scrap = value; }
        }
    }
}
