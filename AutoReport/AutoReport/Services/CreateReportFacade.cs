namespace AutoReport.Services;

public class ReportFacade
{
    public LoginService LoginService { get; }
    public Document2Base64Service Document2Base64Service { get; }
    public DocumentAddService DocumentAddService { get; }

    public ReportFacade(LoginService loginService,
        Document2Base64Service document2Base64Service,
        DocumentAddService documentAddService)
    {
        LoginService = loginService;
        Document2Base64Service = document2Base64Service;
        DocumentAddService = documentAddService;
    }
}
