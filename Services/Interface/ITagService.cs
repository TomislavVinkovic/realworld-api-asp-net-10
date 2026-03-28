using RealWorld.Common;
using RealWorld.Models.DTOs.Tags;

namespace RealWorld.Services.Interface;

public interface ITagService
{
    public Task<ServiceResult<TagListResponse>> GetTagsAsync();
}