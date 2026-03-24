using RealWorld.Data;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;

    public TagService(
        AppDbContext context,
        IHttpContextService httpContextService
    )
    {
        _context = context;
    }
    public async Task<IEnumerable<string>> GetTagsAsync()
    {
        var tagList = await _context.Tags
            .Select(t => t.TagText)
            .ToListAsync();

        return tagList;
    }
}