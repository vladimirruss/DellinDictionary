using DellinDictionary.Data;
using DellinDictionary.Mappers;
using DellinDictionary.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DellinDictionary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfficesController : ControllerBase
{
    private readonly DellinDictionaryDbContext _db;
    private readonly ILogger<OfficesController> _logger;

    public OfficesController(DellinDictionaryDbContext db, ILogger<OfficesController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Поиск терминалов по названию города и области или по идентификатору города.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<OfficeDto>>> GetOffices(
        [FromQuery] string? city,
        [FromQuery] string? region,
        [FromQuery] int? cityCode,
        CancellationToken cancellationToken)
    {
        city = city?.Trim();
        region = region?.Trim();

        if (cityCode is null && string.IsNullOrWhiteSpace(city))
            return BadRequest(new ErrorDto("Укажите 'city' или 'cityCode'"));

        var query = _db.Offices.AsNoTracking();

        if (cityCode is not null)
        {
            _logger.LogInformation("Поиск терминалов по cityCode={CityCode}", cityCode);
            query = query.Where(o => o.CityCode == cityCode);
        }
        else
        {
            _logger.LogInformation("Поиск терминалов: город={City}, область={Region}", city, region);
            query = query.Where(o => EF.Functions.ILike(o.AddressCity!, city!));

            if (!string.IsNullOrWhiteSpace(region))
                query = query.Where(o => EF.Functions.ILike(o.AddressRegion!, region));
        }

        var offices = await query.ProjectToDto().ToListAsync(cancellationToken);

        _logger.LogInformation("Найдено {Count} терминалов", offices.Count);

        return Ok(offices);
    }

    /// <summary>
    /// Поиск идентификатора города по названию города и области.
    /// </summary>
    [HttpGet("city-code")]
    public async Task<ActionResult<CityCodeDto>> GetCityCode(
        [FromQuery] string city,
        [FromQuery] string? region,
        CancellationToken cancellationToken)
    {
        city = city.Trim();
        region = region?.Trim();

        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new ErrorDto("Параметр 'city' не может быть пустым"));

        _logger.LogInformation("Поиск идентификатора города: город={City}, область={Region}", city, region);

        var query = _db.Offices
            .AsNoTracking()
            .Where(o => EF.Functions.ILike(o.AddressCity!, city));

        if (!string.IsNullOrWhiteSpace(region))
            query = query.Where(o => EF.Functions.ILike(o.AddressRegion!, region));

        var cityCode = await query
            .Select(o => (int?)o.CityCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (cityCode is null)
        {
            _logger.LogWarning("Город не найден: город={City}, область={Region}", city, region);
            return NotFound(new ErrorDto($"Город '{city}' не найден"));
        }

        _logger.LogInformation("Идентификатор города={CityCode} для города={City}", cityCode.Value, city);

        return Ok(new CityCodeDto(cityCode.Value));
    }
}
