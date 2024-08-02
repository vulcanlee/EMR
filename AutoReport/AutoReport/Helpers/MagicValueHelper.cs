namespace AutoReport.Helpers;

public class MagicValueHelper
{
    public const string WebemrEndpoint = $"https://office.exentric.com.tw:8002/emr";
    public const string LoginApi = $"/login";
    public const string DocumentToBase64Api = $"/document/toBase64";
    public const string DocumentAddApi = $"/document/add";
    public static List<string> RawReportXmls = new List<string>()
    {
        "Progress Note_1234567807230001.xml",
        "Progress Note_1234567807230002.xml",
        "Progress Note_1234567807230003.xml",
        "門診病歷單_1234567810030001.xml",
        "門診病歷單_1234567810030002.xml",
        "門診病歷單_1234567810030003.xml",
        "門諾測試_12345612345.xml",
        "門諾測試_12345612345678.xml",
        "門諾測試_12345654321.xml",
    };
}
