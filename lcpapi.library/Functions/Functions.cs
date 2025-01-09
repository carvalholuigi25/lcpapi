using System.Runtime.InteropServices;
using System.Globalization;
using Newtonsoft.Json;

namespace lcpapi.Functions;

public static class Functions
{
    public static string GetSlash()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";
    }

    public static async Task<List<dynamic>> GetLanguagesList(bool isdetailed = false)
    {
        var fslash = GetSlash();
        var fdir = $"{Path.GetDirectoryName(Directory.GetCurrentDirectory())}{fslash}lcpapi.library{fslash}JSON{fslash}";
        var fname = $"{fdir}languageslist.json";
        using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (StreamReader r = new StreamReader(fs)) {
            string jsonString = await r.ReadToEndAsync();
            var lst = JsonConvert.DeserializeObject<List<Languages>>(jsonString)!.ToList();
            return isdetailed ? lst.OrderBy(x => x.Id).ToList<dynamic>()! : lst.Select(x => x.Value)!.ToList<dynamic>()!;    
        }
    }

    public static async Task<List<CultureInfo>> GetLanguagesCultureList()
    {
        var glang = await GetLanguagesList(false);
        var lc = new List<CultureInfo>();
        for (var i = 0; i < glang.Count; i++)
        {
            lc.Add(new CultureInfo(glang[i]));
        }
        return lc;
    }

    public class Languages
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
    }
}
