namespace AutoReport.Models;

public class ApiResultModel<T>
{
    public string msg { get; set; }
    public string code { get; set; }
    public string token { get; set; }
    public List<T> data { get; set; }
    public List<T> rows { get; set; }

    public bool IsSuccess()
    {
        return code == "200";
    }
}
