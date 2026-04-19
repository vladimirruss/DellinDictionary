using System.Linq.Expressions;
using DellinDictionary.Models.Dto;
using DellinDictionary.Models.Entities;

namespace DellinDictionary.Mappers;

public static class OfficeMapper
{
    /// <summary>
    /// Expression для SQL-проекции в <see cref="OfficeDto"/>. EF транслирует его в SELECT
    /// только нужных колонок — избегаем тащить всю сущность в память.
    /// </summary>
    private static readonly Expression<Func<Office, OfficeDto>> ToDtoExpression = office => new OfficeDto
    {
        Id = office.Id,
        Code = office.Code,
        CityCode = office.CityCode,
        Uuid = office.Uuid,
        Type = office.Type,
        CountryCode = office.CountryCode,
        Coordinates = new CoordinatesDto(office.Coordinates.Latitude, office.Coordinates.Longitude),
        AddressRegion = office.AddressRegion,
        AddressCity = office.AddressCity,
        AddressStreet = office.AddressStreet,
        AddressHouseNumber = office.AddressHouseNumber,
        AddressApartment = office.AddressApartment,
        WorkTime = office.WorkTime,
        Phones = new PhoneDto(office.Phones.PhoneNumber, office.Phones.Additional)
    };

    public static IQueryable<OfficeDto> ProjectToDto(this IQueryable<Office> source) =>
        source.Select(ToDtoExpression);
}
