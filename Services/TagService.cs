using RealWorld.Data;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Common;
using RealWorld.Models.DTOs.Tags;
using System.Diagnostics;

namespace RealWorld.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;

    public TagService(
        AppDbContext context
    )
    {
        _context = context;
    }
    public async Task<ServiceResult<TagListResponse>> GetTagsAsync()
    {
        var tagList = await _context.Tags
            .Select(t => t.TagText)
            .ToListAsync();
        var response = new TagListResponse(tagList);

        return ServiceResult<TagListResponse>.Ok(response);
    }
}