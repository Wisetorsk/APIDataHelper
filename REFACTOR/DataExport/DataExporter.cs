using KOMTEK.KundeInnsyn.Common.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public class DataExporter : IDataExporter
    {
        private static readonly string _filename = "Transaksjonslogg";
        public event Action<string> ExportReady;
        private static readonly string _fileLocation = "wwwroot/Logs/";
        private List<string> RedactionList
        {
            get
            {
                var output = new List<string>();
                foreach (var word in _wordArray)
                {
                    output.Add(word.ToLower());
                    output.Add(word.ToUpper());
                    output.Add(word);
                }
                return output;
            }
        }
        private string[] _wordArray = { "Passord", "Password" };
        //private static CultureInfo _culture = new CultureInfo("nb-NO");



        public DataExporter()
        {
            //CultureInfo.CurrentCulture = new CultureInfo("nb-NO");
        }

        public async Task ToJSON<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null)
        {
            Validate(dataArray);
            string extention = ".json";

            string filename = (pathOverride ?? _fileLocation) + (filenameOverride ?? _filename) + nameArguments + extention;

            var data = JsonConvert.SerializeObject(dataArray);
            var writingTask = WriteToFile(filename, data);
            await writingTask.ContinueWith(t => ExportReady?.Invoke(filename));
        }

        public async Task ToCSV<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null)
        {
            string extention = ".csv";
            string filename = (pathOverride ?? _fileLocation) + (filenameOverride ?? _filename) + nameArguments + extention;
            bool first = true;
            List<string> lines = new List<string>();
            foreach (var dataRow in dataArray)
            {
                string line = "";

                PropertyInfo[] properties = typeof(Tdata).GetProperties();
                if (first)
                {
                    string header = "";
                    foreach (PropertyInfo property in properties)
                    {
                        header += (property.Name ?? "null");
                        header += ",";
                    }
                    header = header.Remove(header.Length - 1, 1);
                    lines.Add(header + "\n");
                    first = false;
                }
                foreach (PropertyInfo property in properties)
                {
                    object value;

                    if (RedactionList.Contains(property.Name.Trim()))
                    {
                        value = "REDACTED";
                    }
                    else
                    {
                        value = property.GetValue(dataRow) ?? " ";
                    }

                    line += value.ToString().Replace(",", " ") + ",";
                }
                line = line.Remove(line.Length - 1, 1);
                lines.Add(line + "\n");
            }
            var writingTask = WriteToFile(filename, lines);
            await writingTask.ContinueWith(t => ExportReady?.Invoke(filename));
        }

        private static async Task WriteToFile(string filename, string line)
        {
            FileStream stream = new FileStream(filename, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
            {
                await sw.WriteAsync(line);
            }
        }

        private static async Task WriteToFile(string filename, List<string> lines)
        {
            FileStream stream = new FileStream(filename, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
            {
                foreach (var line in lines)
                {
                    await sw.WriteAsync(line);
                }
            }
        }

        private static void Validate<Tdata>(Tdata[] dataArray)
        {
            if (dataArray.Length == 0) throw new EmptyDataArrayException("Input array is empty");
        }

        public async Task ToXML<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null)
        {
            string extention = ".xml";
            string filename = (pathOverride ?? _fileLocation) + (filenameOverride ?? _filename) + nameArguments + extention;
            var serializer = new XmlSerializer(typeof(Tdata[]));
            var writingTask = Task.Run(() =>
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    serializer.Serialize(sw, dataArray);
                    sw.Close();
                }
            });
            await writingTask.ContinueWith(t => ExportReady?.Invoke(filename));
        }

        public async Task ToPDF<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null)
        {
            await Task.Run(() => throw new NotImplementedException());
        }

        public async Task ToYAML<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null)
        {
            await Task.Run(() => throw new NotImplementedException());
        }
    }


}
