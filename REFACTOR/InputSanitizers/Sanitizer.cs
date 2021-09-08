
using Ganss.XSS;

namespace KOMTEK.KundeInnsyn.Common.Services.InputSanitizers
{

    public class Sanitizer
    {
        //private HtmlSanitizer HtmlSanitizer { get; set; }

        public static string SanitizeHtml(string input)
        {
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(input);
        }
    }
}
