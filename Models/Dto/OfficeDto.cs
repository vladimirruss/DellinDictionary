namespace DellinDictionary.Models.Dto;

public sealed record OfficeDto
{
    public int Id { get; init; }
    public string? Code { get; init; }
    public int CityCode { get; init; }
    public string? Uuid { get; init; }
    public OfficeType? Type { get; init; }
    public required string CountryCode { get; init; }
    public required CoordinatesDto Coordinates { get; init; }
    public string? AddressRegion { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressStreet { get; init; }
    public string? AddressHouseNumber { get; init; }
    public int? AddressApartment { get; init; }
    public required string WorkTime { get; init; }
    public required PhoneDto Phones { get; init; }
}
