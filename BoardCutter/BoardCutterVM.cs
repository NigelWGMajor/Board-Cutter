using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace BoardCutter
{
    class BoardCutterVM : INotifyPropertyChanged
    {
        #region Commands
        private ICommand _calculateCommand, _exportCommand, _loadCommand, _saveCommand, _addRowCommand;
        public ICommand CalculateCommand { get { return _calculateCommand; } }
        public ICommand ExportCommand { get { return _exportCommand; } }
        public ICommand LoadCommand { get { return _loadCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }
        public ICommand AddRowCommand { get { return _addRowCommand; } }
        private void initializeCommands()
        {
            _calculateCommand = new CommandClass(c => true, c => doCalculate());
            _exportCommand = new CommandClass(c => true, c => doExport());
            _loadCommand = new CommandClass(c => true, c => doLoad());
            _saveCommand = new CommandClass(c => true, c => doSave());
            _addRowCommand = new CommandClass(c => true, c => doAddRow());
        }


        #endregion // commands

        #region ctor
        public BoardCutterVM()
        {
            initializeData();
            initializeCommands();
        }
        #endregion

        #region Properties
        private ObservableCollection<InputItem> _inputs;
        private ObservableCollection<OutputItem> _outputs;
        private ObservableCollection<Part> _parts;
        private ObservableCollection<SummaryItem> _summaries;
        public ObservableCollection<SummaryItem> Summaries
        {
            get { return _summaries; }
            set
            {
                if (value == _summaries) return;
                _summaries = value;
                raisePropertyChanged("Summaries");
            }
        }
        public ObservableCollection<InputItem> Inputs
        {
            get { return _inputs; }
            set
            {
                if (value == _inputs) return;
                _inputs = value;
                raisePropertyChanged("Inputs");
            }
        }
        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (value == _projectName) return;
                _projectName = value;
                raisePropertyChanged("ProjectName");
            }
        }
        public ObservableCollection<OutputItem> Outputs
        {
            get { return _outputs; }
            set
            {
                if (value == _outputs) return;
                _outputs = value;
                raisePropertyChanged("Outputs");
            }
        }
        private double _sourceLength;
        public double SourceLength
        {
            get { return _sourceLength; }
            set
            {
                if (value == _sourceLength) return;
                _sourceLength = value;
                raisePropertyChanged("SourceHeight");
            }
        }
        private double _sourceWidth;
        public double SourceWidth
        {
            get { return _sourceWidth; }
            set
            {
                if (value == _sourceWidth) return;
                _sourceWidth = value;
                raisePropertyChanged("SourceWidth");
            }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message) return;
                _message = value;
                raisePropertyChanged("Message");
            }
        }
        private double _kerf;
        public double Kerf
        {
            get { return _kerf; }
            set
            {
                if (value == _kerf) return;
                _kerf = value;
                raisePropertyChanged("Kerf");
            }
        }
        private double _endLoss;
        public double EndLoss
        {
            get { return _endLoss; }
            set
            {
                if (value == _endLoss) return;
                _endLoss = value;
                raisePropertyChanged("EndLoss");
            }
        }
        private bool _includeBoards = true;
        public bool IncludeBoards
        {
            get { return _includeBoards; }
            set
            {
                if (value == _includeBoards) return;
                _includeBoards = value;
                raisePropertyChanged("IncludeBoards");
            }
        }
        private bool _includeParts = true;
        public bool IncludeParts
        {
            get { return _includeParts; }
            set
            {
                if (value == _includeParts) return;
                _includeParts = value;
                raisePropertyChanged("IncludeParts");
            }
        }
        private bool _includeInput = true;
        public bool IncludeInput
        {
            get { return _includeInput; }
            set
            {
                if (value == _includeInput) return;
                _includeInput = value;
                raisePropertyChanged("IncludeInput");
            }
        }
        #endregion // properties
        #region Methods
        private void initializeData()
        {   // possibly read last project or settings.

            Inputs = new ObservableCollection<InputItem>();
            initialize();
            initializeTestData();
        }
        private void doLoad()
        {
            String loadPath = GetLoadPath();
            if (loadPath == null) return;
            Project project = new Project(loadPath);
            _projectName = project.Name;
            _sourceLength = project.SourceLength;
            _sourceWidth = project.SourceWidth;
            _kerf = project.Kerf;
            _endLoss = project.EndLoss;
            _inputs = new ObservableCollection<InputItem>(project.Inputs);
            raisePropertyChanged("Inputs", "ProjectName", "SourceLength", "SourceWidth", "Kerf", "EndLoss");
        }
        private void doSave()
        {
            string filePath = GetSavePath();
            if (filePath != null)
                new Project(_projectName, _sourceLength, _sourceWidth, _kerf, _endLoss, _inputs.ToArray()).Save(filePath);
        }
        private void doAddRow()
        {

            if (_inputs == null) _inputs = new ObservableCollection<InputItem>();
            int i = _inputs.Count - 1;
            if (i > -1)
            {
                string s = _inputs[i].Id;
                // we want to somehow "increment" this.  If it is a letter, we use the next.
                // If a number we should bump it.
                Regex r = new Regex(@".*[0-9]+\Z");
                Match m = r.Match(s);
                if (m.Success)
                {   // we have a string ending in a number.  Increment the number.
                    string t = m.Value;
                    int n = int.Parse(t) + 1;
                    // we format the number to the same size it was.
                    s = s.Substring(0, s.Length - t.Length) + n.ToString(new String('0', t.Length));
                }
                else
                {   // If a single letter, we increment that.
                    if (s.Length == 1)
                    {
                        char c = s.ToCharArray()[0];
                        c++;
                        s = c.ToString();
                    }
                }
                _inputs.Add(new InputItem(s, _inputs[i].Count, _inputs[i].Length, _inputs[i].Width));
            }
            else
                _inputs.Add(new InputItem("new", 1, 0, 0));
            raisePropertyChanged("Inputs");
        }
        private string GetSavePath()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.CheckPathExists = true;
            d.DefaultExt = ".bcp";
            d.FileName = _projectName + ".bcp";
            d.Title = "Save Board Cutter Project to File";
            if (d.ShowDialog() == true)
                return d.FileName;
            return null;
        }
        private string GetLoadPath()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.CheckFileExists = true;
            d.DefaultExt = ".bcp";
            d.Title = "Load Board Cutter Project from file";
            if (d.ShowDialog() == true)
                return d.FileName;
            return null;
        }
        private void initializeTestData()
        {
            ProjectName = "NewProject";
            Inputs.Add(new InputItem("A", 1, 12, 24));
            SourceLength = 96;
            SourceWidth = 24;
            Kerf = 0.125;
            EndLoss = 1.0;
        }
        private void doCalculate()
        {   // calculate from grid
            initialize();
            generateParts();
            allocateParts();
            showOutput();
            showStatistics();
            showSummaries();
        }
        private void initialize()
        {
            Outputs = new ObservableCollection<OutputItem>();
            _parts = new ObservableCollection<Part>();
            Summaries = new ObservableCollection<SummaryItem>();
        }
        private void showStatistics()
        {
            Message = String.Format(
                "Uses {0:0} boards. Average Scrap: {1:00}\", left over: {2:00}\"",
                _currentBoard + 1,
                _scrap / _currentBoard,

                _finalScrap);
        }
        private int _partsCount;
        private double usedLength;
        private double _scrap;
        private double _finalScrap;
        private void generateParts()
        {   // turn inputs into parts sequenced on width then length. 
            foreach (var input in Inputs.Where(i => i.Include).OrderByDescending(i => i.Width).ThenByDescending(i => i.Length))
            {
                int index = 1;
                for (int i = input.Count; i > 0; i--)
                    _parts.Add(new Part(String.Format("{0}-{1}", input.Id, index++), input.Length, input.Width));
            }
            _partsCount = _parts.Count;
            usedLength = _parts.Sum(p => p.Length);
        }
        int _currentBoard;
        private void allocateParts()
        {
            _scrap = 0.0;
            _currentBoard = -1;
            while (_parts.Where(p => p.NotDone).Count() > 0)
                allocatePartsToSource();
        }

        private void allocatePartsToSource()
        {   // As long as parts are not marked impossible, or placed, the part allocation will try to use up more boards.
            _currentBoard++;
            double usableLength = getNewLength();
            int position = 0;
            // first we lay the biggest possible board at the start...
            Part primary = getLargest();
            if (primary == null)
                throw new Exception("Unable to get a part - are there any?");
            if (primary.Length > usableLength)
            {   // mark board as impossible (if using only one source size...) 
                primary.status = Part.PartStatus.Impossible;
                return;
            }
            else
            {
                primary.source = _currentBoard;
                primary.position = position++;
                primary.status = Part.PartStatus.Placed;
            }
            usableLength -= Kerf;
            usableLength -= primary.Length;
            bool fullEnough = false;
            while (!fullEnough)
            {   // here we try to fill the rest as best as we can...
                // 1: Try fill with the largest possible remaining:
                Part proposed = getJustSmallerThan(usableLength);
                if (proposed == null)
                {   // we have run out of stuff!  Nothing to do!
                    _finalScrap = usableLength;
                    return;
                }
                usableLength -= Kerf;
                proposed.status = Part.PartStatus.Proposed;
                usableLength -= proposed.Length;

                if (usableLength < getSmallest().Length)
                    // nothing left that fits!
                    fullEnough = true;
                proposed.source = _currentBoard;
                proposed.position = position++;
                proposed.status = Part.PartStatus.Placed;
            }
            _scrap += usableLength;
        }
        private double getNewLength()
        {   // we can extend this to accept multiple sourcelengths if need be.  
            // We might deliver these as an enum from smaller to largest. 
            return _sourceLength - _endLoss - _endLoss;
        }
        // These allow us to quickly find parts...
        private Part getLargest()
        {   // return the first one of the largest parts not yet allocated
            return _parts
                .Where(p => p.NotDone)
                .Where(p => p.Length == _parts.Where(q => q.NotDone).Max(q => q.Length))
                .OrderBy(p => p.Instance)
                .FirstOrDefault();
        }
        private Part getSmallest()
        {   // return the first one of the smallest parts not yet allocated
            return _parts
                .Where(p => p.NotDone)
                .Where(p => p.Length == _parts.Where(q => q.NotDone).Min(q => q.Length))
                .OrderBy(p => p.Instance)
                .FirstOrDefault();
        }
        private Part getJustSmallerThan(double size)
        {   // return the largest part that is within the given size
            return _parts
               .Where(p => p.NotDone && p.Length < size)
               .OrderByDescending(q => q.Length).ThenBy(q => q.Instance)
               .FirstOrDefault();
        }
        private void showSummaries()
        {   // summaries show one line per board.
            int lastSource = -1;
            usedLength = 0;
            SummaryItem summary = null;
            foreach (Part part in _parts.OrderBy(p => p.source).ThenBy(p => p.position))
            {
                if (part.source == lastSource)
                {   // do the repetitive stuff
                    summary.Cutlist += String.Format("{0}={1:0.000}  ", part.Id, part.Length);
                    usedLength += part.Length + _kerf;
                }
                else
                {   // do the new item stuff.
                    lastSource = part.source;

                    if (summary != null)
                    {
                        summary.Scrap = getNewLength() - usedLength;
                        _summaries.Add(summary);
                        usedLength = 0;
                    }
                    summary = new SummaryItem();
                    summary.Board = part.source + 1;
                    summary.Cutlist += String.Format("{0}={1:0.000}  ", part.Id, part.Length);
                    usedLength += part.Length + _kerf;
                }
            }
            if (summary != null)
            {
                summary.Scrap = getNewLength() - usedLength;
                _summaries.Add(summary);
            }
        }
        private void showOutput()
        {
            foreach (Part p in _parts.OrderBy(p => p.source).ThenBy(p => p.position))
            {
                Outputs.Add(new OutputItem(p));
            }
        }
        private void doExport()
        {   // export results
            StringBuilder sb = new StringBuilder(String.Format("Cut List {0:yyyy-MM-dd hh:mm a}\r\n", DateTime.Now));
            if (_includeInput) exportDefinitions(ref sb);
            if (_includeBoards) exportSummaries(ref sb);
            if (IncludeParts) exportDetails(ref sb);
            FinalizeExport(ref sb);
        }
        private void exportDefinitions(ref StringBuilder sb)
        {
            sb.AppendLine("\r\n\r\nDefinitions");
            sb.AppendFormat("Name\tQuantity\tLength\tWidth\r\n");
            foreach (InputItem item in _inputs)
                if (item.Include) sb.AppendFormat("{0}\t{1}\t{2} x {3}\r\n", item.Id, item.Count, item.Length, item.Width);
        }
        private void exportSummaries(ref StringBuilder sb)
        {
            sb.AppendLine("\r\n\r\nBoard Cut List");
            sb.AppendFormat("Board\tCuts\t(scrap)\r\n");
            foreach (SummaryItem item in _summaries)
                sb.AppendFormat("{0}:\t{1}\t(Scrap {2})\r\n", item.Board, item.Cutlist, item.Scrap);
        }
        private void exportDetails(ref StringBuilder sb)
        {
            sb.AppendLine("\r\n\r\nParts");
            sb.AppendFormat("Part\tBoard\tCut#\tLength\tWidth\r\n");
            foreach (Part item in _parts)
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\r\n", item.Id, item.source, item.position, item.Length, item.Width);
        }
        private void FinalizeExport(ref StringBuilder sb)
        {
            Clipboard.SetText(sb.ToString());
            MessageBox.Show(sb.ToString(), "Placed in clipboard:");
        }
        #endregion // methods
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void raisePropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
                foreach (string propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion // INotifyPropertyChanged
    }
}
