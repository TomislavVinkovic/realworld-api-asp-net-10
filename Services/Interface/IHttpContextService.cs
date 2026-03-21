namespace dotnet_api_tutorial.Services.Interface;

public interface IHttpContextService
{
    public int? GetCurrentUserId();
    public string GetBaseUrl();
}