using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardCutter
{
    class InputItem
    {
        public InputItem(string id, int count, double length, double width = 0 )
        {
            Id = id;
            Count = count;
            Length = length;
            Width = width;
            Include = true;
        }
        public InputItem(string saveString)
        {
            Id=Project.ReadString(saveString, "Id");
            Count = Project.ReadInt(saveString, "Count");
            Length = Project.ReadDouble(saveString, "Length");
            Width = Project.ReadDouble(saveString, "Width");
            Include = Project.ReadBool(saveString, "Include");
        }
        public Boolean Include { get; set; }
        public string Id { get; set; }
        public int Count { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }

        internal string ToSaveString()
        {
            return string.Format("Id:{0};Count:{1};Length:{2};Width:{3};Include:{4};", Id, Count, Length, Width, Include);
        }
    }
}
