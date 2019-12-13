using D2TxtImporter.lib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib
{
    public class ExceptionHandler
    {
        public static bool ContinueOnException { get; set; }

        private readonly static string _exceptionFile = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}/errorlog.txt";
        private readonly static string _debugFile = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}/debuglog.txt";

        public static List<string> _exceptionsWritten;

        public static void Initialize()
        {
            _exceptionsWritten = new List<string>();

            // Empty output files
            if (File.Exists(_exceptionFile))
            {
                File.Delete(_exceptionFile);
            }
            if (File.Exists(_debugFile))
            {
                File.Delete(_debugFile);
            }
        }

        public static void WriteException(Exception e)
        {
            var ex = e;

            var errorMessage = "";
            var debugMessage = "";
            do
            {
                errorMessage += $"Message:\n{ex.Message}\n\nStacktrace:\n{ex.StackTrace}\n";
                debugMessage += $"Message:\n{ex.Message}\n";

                if (ex is ItemPropertyException || ex is ItemStatCostException)
                {
                    errorMessage = $"Message:\n{ex.Message}\n\nStacktrace:\n{ex.StackTrace}\n";
                    debugMessage = $"Message:\n{ex.Message}\n";

                    break;
                }

                ex = ex.InnerException;
            }
            while (ex != null);

            if (!_exceptionsWritten.Contains(debugMessage))
            {
                _exceptionsWritten.Add(debugMessage);

                File.AppendAllText(_exceptionFile, errorMessage + "\n");
                File.AppendAllText(_debugFile, debugMessage + "\n");
            }

            if (!ContinueOnException)
            {
                throw e;
            }
        }

        public static void LogException(Exception e)
        {
            WriteException(e);

            if (!ContinueOnException)
            {
                throw e;
            }
        }
    }

    public class ItemStatCostException : Exception
    {
        private ItemStatCostException(string message) : base(message)
        {

        }

        public static ItemStatCostException Create(string message)
        {
            var resultMessage = "";

            resultMessage += $"Exception loading properties for item: '{Item.CurrentItem.Index}'\n";
            resultMessage += $"\tCould not generate properties for property '{ItemProperty.CurrentItemProperty.Property.Code}' with parameter '{ItemProperty.CurrentItemProperty.Parameter}' min '{ItemProperty.CurrentItemProperty.Min}' max '{ItemProperty.CurrentItemProperty.Max}' index '{ItemProperty.CurrentItemProperty.Index}' itemlvl '{ItemProperty.CurrentItemProperty.ItemLevel}'\n";
            resultMessage += $"\t\t{message}";

            return new ItemStatCostException(resultMessage);
        }
    }

    public class ItemPropertyException : Exception
    {
        private ItemPropertyException(string message, Exception e) : base(message, e)
        {

        }

        public static ItemPropertyException Create(string message, Exception e = null)
        {
            var resultMessage = "";

            resultMessage += $"Exception loading properties for item: '{Item.CurrentItem.Index}'\n";
            resultMessage += $"\t{message}";

            return new ItemPropertyException(resultMessage, e);
        }
    }
}
