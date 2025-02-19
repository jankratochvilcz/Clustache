using System.Threading.Tasks;
using Clustache.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/[controller]")]
public class CachedItemController : ControllerBase
{
    private readonly CachedItemService _cachedItemService;

    public CachedItemController(CachedItemService cachedItemService)
    {
        _cachedItemService = cachedItemService;
    }

    [HttpGet("{itemId}")]
    public async Task<ActionResult<CachedItem>> GetCachedItem(string itemId)
    {
        var item = await _cachedItemService.GetItem(itemId);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }
}
