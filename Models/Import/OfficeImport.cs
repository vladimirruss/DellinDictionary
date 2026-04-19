using DellinDictionary.Models.Entities;

namespace DellinDictionary.Models.Import;

internal sealed record OfficeImport(
    string? Code,
    int CityCode,
    string? Uuid,
    OfficeType? Type,
    string CountryCode,
    Coordinates Coordinates,
    string? AddressRegion,
    string? AddressCity,
    string? AddressStreet,
    string? AddressHouseNumber,
    int? AddressApartment,
    string WorkTime,
    PhoneImport Phones);

internal sealed record PhoneImport(
    string PhoneNumber,
    string? Additional);
