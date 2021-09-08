using System;
using System.Threading.Tasks;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public interface IDataExporter
    {
        event Action<string> ExportReady;

        Task ToCSV<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null);
        Task ToJSON<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null);
        Task ToXML<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null);
        Task ToPDF<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null);
        Task ToYAML<Tdata>(Tdata[] dataArray, string nameArguments = "", string filenameOverride = null, string pathOverride = null);
    }
}