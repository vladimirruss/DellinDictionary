using DellinDictionary.Models.Entities;
using DellinDictionary.Models.Import;

namespace DellinDictionary.Mappers;

/// <summary>
/// Маппинг import JSON → domain (inbound).
/// </summary>
internal static class OfficeImportMapper
{
    internal static Office ToOffice(this OfficeImport src) => new()
    {
        Code = src.Code,
        CityCode = src.CityCode,
        Uuid = src.Uuid,
        Type = src.Type,
        CountryCode = src.CountryCode,
        Coordinates = src.Coordinates,
        AddressRegion = src.AddressRegion,
        AddressCity = src.AddressCity,
        AddressStreet = src.AddressStreet,
        AddressHouseNumber = src.AddressHouseNumber,
        AddressApartment = src.AddressApartment,
        WorkTime = src.WorkTime,
        Phones = src.Phones.ToPhone()
    };

    private static Phone ToPhone(this PhoneImport src) => new()
    {
        PhoneNumber = src.PhoneNumber,
        Additional = src.Additional
    };
}
