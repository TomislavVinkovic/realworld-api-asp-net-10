namespace dotnet_api_tutorial.Services.Interface;

public interface ITagService
{
    public Task<IEnumerable<string>> GetTagsAsync();
}