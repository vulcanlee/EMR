using AutoReport.Helpers;
using AutoReport.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoReport.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerateReportController : ControllerBase
{
    private readonly ILogger<GenerateReportController> _logger;
    private readonly ReportFacade reportFacade;

    public GenerateReportController(ILogger<GenerateReportController> logger,
        ReportFacade reportFacade)
    {
        _logger = logger;
        this.reportFacade = reportFacade;
    }

    [HttpGet]
    public async Task Get()
    {
        var resultLoginService = await reportFacade.LoginService
            .LoginAsync("exentric", "70400845");
        string accessToken = resultLoginService.token;

        foreach (var xmlFilename in MagicValueHelper.RawReportXmls)
        {
            string filename = Path.Combine(Environment.CurrentDirectory,
                "Datas", xmlFilename);
            string contentXml = System.IO.File.ReadAllText(filename);
            var resultDocument2Base64Service = await reportFacade
                .Document2Base64Service.ConvertBase64Async(accessToken, contentXml);
            string contentBase64 = resultDocument2Base64Service.data;
            var resultDocumentAddService = await reportFacade
                .DocumentAddService.ConvertBase64Async(accessToken, contentBase64);
            //string contentBase64 = resultDocument2Base64Service.data;

        }
    }
}
