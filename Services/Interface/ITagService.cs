namespace RealWorld.Services.Interface;

public interface ITagService
{
    public Task<IEnumerable<string>> GetTagsAsync();
}