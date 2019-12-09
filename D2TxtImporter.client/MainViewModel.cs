using System.ComponentModel;
using System.IO;

namespace D2TxtImporter.client
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private D2TxtImporter.lib.Importer _importer;
        private string excelPath;
        private string tablePath;
        private string outputPath;

        public D2TxtImporter.lib.Importer Importer
        {
            get => _importer;
            set
            {
                _importer = value;
                OnPropertyChange(nameof(ExportEnabled));
            }
        }

        public string ExcelPath
        {
            get => excelPath; set
            {
                excelPath = value;
                OnPropertyChange(nameof(ExcelPath));
                OnPropertyChange(nameof(ImportEnabled));
            }
        }

        public string TablePath
        {
            get => tablePath; set
            {
                tablePath = value;
                OnPropertyChange(nameof(TablePath));
                OnPropertyChange(nameof(ImportEnabled));
            }
        }

        public string OutputPath
        {
            get => outputPath; set
            {
                outputPath = value;
                OnPropertyChange(nameof(OutputPath));
                OnPropertyChange(nameof(ImportEnabled));
            }
        }

        public bool CubeRecipeUseDescription
        {
            get
            {
                return lib.Model.CubeRecipe.UseDescription;
            }
            set
            {
                lib.Model.CubeRecipe.UseDescription = value;
            }
        }

        public bool ContinueOnException
        {
            get
            {
                return lib.Importer.ContinueOnException;
            }
            set
            {
                lib.Importer.ContinueOnException = value;
            }
        }

        public bool ExportEnabled => Importer != null && Importer.CubeRecipes != null && Importer.Runewords != null && Importer.Uniques != null;
        public bool ImportEnabled => Directory.Exists(ExcelPath) && Directory.Exists(TablePath) && Directory.Exists(OutputPath);

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
