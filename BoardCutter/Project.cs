using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardCutter
{
    class Project
    {
        private string _data;
        public Project(string projectName, double sourceLength, double sourceWidth, double kerf, double endLoss, InputItem[] inputs) 
        {
            StringBuilder sb = new StringBuilder("Name:" + projectName + ";");
            sb.Append("Length:" + sourceLength.ToString() + ";");
            sb.Append("Width:" + sourceWidth.ToString() + ";");
            sb.Append("Kerf:" + kerf.ToString() + ";");
            sb.AppendLine("EndLoss:" + endLoss.ToString() + ";");
            foreach (InputItem item in inputs)
                sb.AppendLine(item.ToSaveString());
            _data = sb.ToString();
        }
        public Project(string filePath)
        {
            Load(filePath);
        }
        public void Save(string filePath)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false))
            {
                writer.Write(_data);
            }
        }
        public void Load(string FilePath)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(FilePath))
            {
                _data = reader.ReadToEnd();
            }
        }
        public string Name { get { return ReadString(_data, "Name"); } }
        public double SourceLength { get { return ReadDouble(_data, "Length"); } }
        public double SourceWidth { get { return ReadDouble(_data, "Width"); } }
        public double Kerf { get { return ReadDouble(_data, "Kerf"); } }
        public double EndLoss { get { return ReadDouble(_data, "EndLoss"); } }
        public int Count { get { return ReadInt(_data, "Count"); } }
        public bool Include { get { return ReadBool(_data, "Include"); } }
        public IEnumerable<InputItem> Inputs
        {
            get
            {
                string[] items = _data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < items.Length; i++) // skip the first line, it is the other settings.
                {
                    yield return new InputItem(items[i]);
                }
            }
        }
        public static int ReadInt(string data, string key)
        {
            int result;
            if (int.TryParse(findByKey(data, key), out result))
                return result;
            return 0;
        }
        public static double ReadDouble(string data, string key)
        {
            double result;
            if (double.TryParse(findByKey(data, key), out result))
                return result;
            return 0.0;
        }
        public static string ReadString(string data, string key)
        {
            return findByKey(data, key);
        }
        public static bool ReadBool(string data, string key)
        {
            bool result;
            if (bool.TryParse(findByKey(data, key), out result))
                return result;
            return false;
        }
        private static string findByKey(string data, string key)
        {
            int start = data.IndexOf(key + ":");
            if (start < 0) return string.Empty;
            start += key.Length + 1;
            int end = data.IndexOf(";", start);
            if (end < 0)
                return data.Substring(start);
            else
                return data.Substring(start, end - start );
        }
    }
}
