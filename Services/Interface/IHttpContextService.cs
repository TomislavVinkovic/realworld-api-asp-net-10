namespace RealWorld.Services.Interface;

public interface IHttpContextService
{
    public int? GetCurrentUserId();
    public string GetBaseUrl();
}