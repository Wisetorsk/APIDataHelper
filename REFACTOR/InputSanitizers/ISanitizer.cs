namespace KOMTEK.KundeInnsyn.Common.Services.InputSanitizers
{
    public interface ISanitizer
    {
        string SanitizeHtml(string input);
    }
}