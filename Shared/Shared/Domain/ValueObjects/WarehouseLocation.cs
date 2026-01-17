using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects;

using static LanguageExt.Prelude;

public record Warehouse
{
    private Warehouse() { }

    private Warehouse(
        WarehouseCode code,
        string name,
        string address,
        string city,
        string? state,
        string country,
        string? postalCode,
        string? contactPhone,
        string? contactEmail)
    {
        Code = code;
        Name = name;
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        _all.Add(this);
    }

    public WarehouseCode Code { get; private init; }
    public string Name { get; private init; }
    public string Address { get; private init; }
    public string City { get; private init; }
    public string? State { get; private init; }
    public string Country { get; private init; }
    public string? PostalCode { get; private init; }
    public string? ContactPhone { get; private init; }
    public string? ContactEmail { get; private init; }

    private static readonly List<Warehouse> _all = new();
    public static IReadOnlyList<Warehouse> All => _all;

    static Warehouse()
    {
        _ = None;
        _ = MainWarehouse;
        _ = BerlinMitte;
        _ = BerlinSpandau;
        _ = BerlinTempelhof;
        _ = Munich;
        _ = Hamburg;
        _ = Frankfurt;
        _ = Cologne;
        _ = Stuttgart;
        _ = Dortmund;
        _ = Essen;
        _ = Leipzig;
        _ = Dresden;
        _ = Hanover;
        _ = Nuremberg;
        _ = Duisburg;
        _ = Bremen;
        _ = Bonn;
        _ = Mannheim;
        _ = Karlsruhe;
    }

    // Predefined warehouse locations - All in Germany
    public static Warehouse None => new(WarehouseCode.None, "None", "", "", null, "", null, null, null);

    public static Warehouse MainWarehouse => new(
        WarehouseCode.MAIN,
        "Hauptlager Berlin",
        "Alexanderplatz 1",
        "Berlin",
        "Berlin",
        "Germany",
        "10178",
        "+49-30-2270-1000",
        "hauptlager@warehouse.de");

    // Berlin Locations
    public static Warehouse BerlinMitte => new(
        WarehouseCode.NYC,
        "Berlin Mitte Logistikzentrum",
        "Friedrichstraße 95",
        "Berlin",
        "Berlin",
        "Germany",
        "10117",
        "+49-30-2270-2000",
        "mitte@warehouse.de");

    public static Warehouse BerlinSpandau => new(
        WarehouseCode.LAX,
        "Berlin Spandau Distributionszentrum",
        "Wilhelmstraße 20",
        "Berlin",
        "Berlin",
        "Germany",
        "13597",
        "+49-30-2270-3000",
        "spandau@warehouse.de");

    public static Warehouse BerlinTempelhof => new(
        WarehouseCode.CHI,
        "Berlin Tempelhof Warenlager",
        "Tempelhofer Damm 45",
        "Berlin",
        "Berlin",
        "Germany",
        "12101",
        "+49-30-2270-4000",
        "tempelhof@warehouse.de");

    // Major German Cities
    public static Warehouse Munich => new(
        WarehouseCode.MIA,
        "München Logistikzentrum",
        "Marienplatz 8",
        "Munich",
        "Bavaria",
        "Germany",
        "80331",
        "+49-89-2270-5000",
        "muenchen@warehouse.de");

    public static Warehouse Hamburg => new(
        WarehouseCode.SEA,
        "Hamburg Hafenlager",
        "Jungfernstieg 16",
        "Hamburg",
        "Hamburg",
        "Germany",
        "20095",
        "+49-40-2270-6000",
        "hamburg@warehouse.de");

    public static Warehouse Frankfurt => new(
        WarehouseCode.DFW,
        "Frankfurt Distributionszentrum",
        "Zeil 112",
        "Frankfurt",
        "Hesse",
        "Germany",
        "60313",
        "+49-69-2270-7000",
        "frankfurt@warehouse.de");

    public static Warehouse Cologne => new(
        WarehouseCode.ATL,
        "Köln Logistikzentrum",
        "Hohe Straße 41",
        "Cologne",
        "North Rhine-Westphalia",
        "Germany",
        "50667",
        "+49-221-2270-8000",
        "koeln@warehouse.de");

    public static Warehouse Stuttgart => new(
        WarehouseCode.BOS,
        "Stuttgart Warenlager",
        "Königstraße 28",
        "Stuttgart",
        "Baden-Württemberg",
        "Germany",
        "70173",
        "+49-711-2270-9000",
        "stuttgart@warehouse.de");

    public static Warehouse Dortmund => new(
        WarehouseCode.DEN,
        "Dortmund Distributionszentrum",
        "Westenhellweg 68",
        "Dortmund",
        "North Rhine-Westphalia",
        "Germany",
        "44137",
        "+49-231-2270-1100",
        "dortmund@warehouse.de");

    public static Warehouse Essen => new(
        WarehouseCode.PHX,
        "Essen Logistikzentrum",
        "Kettwiger Straße 2",
        "Essen",
        "North Rhine-Westphalia",
        "Germany",
        "45127",
        "+49-201-2270-1200",
        "essen@warehouse.de");

    public static Warehouse Leipzig => new(
        WarehouseCode.LDN,
        "Leipzig Warenlager",
        "Grimmaische Straße 1",
        "Leipzig",
        "Saxony",
        "Germany",
        "04109",
        "+49-341-2270-1300",
        "leipzig@warehouse.de");

    public static Warehouse Dresden => new(
        WarehouseCode.PAR,
        "Dresden Distributionszentrum",
        "Prager Straße 8",
        "Dresden",
        "Saxony",
        "Germany",
        "01069",
        "+49-351-2270-1400",
        "dresden@warehouse.de");

    public static Warehouse Hanover => new(
        WarehouseCode.BER,
        "Hannover Logistikzentrum",
        "Georgstraße 35",
        "Hanover",
        "Lower Saxony",
        "Germany",
        "30159",
        "+49-511-2270-1500",
        "hannover@warehouse.de");

    public static Warehouse Nuremberg => new(
        WarehouseCode.TYO,
        "Nürnberg Warenlager",
        "Königstraße 85",
        "Nuremberg",
        "Bavaria",
        "Germany",
        "90402",
        "+49-911-2270-1600",
        "nuernberg@warehouse.de");

    public static Warehouse Duisburg => new(
        WarehouseCode.SYD,
        "Duisburg Hafenlager",
        "Königstraße 48",
        "Duisburg",
        "North Rhine-Westphalia",
        "Germany",
        "47051",
        "+49-203-2270-1700",
        "duisburg@warehouse.de");

    public static Warehouse Bremen => new(
        WarehouseCode.TOR,
        "Bremen Distributionszentrum",
        "Obernstraße 22",
        "Bremen",
        "Bremen",
        "Germany",
        "28195",
        "+49-421-2270-1800",
        "bremen@warehouse.de");

    public static Warehouse Bonn => new(
        WarehouseCode.DXB,
        "Bonn Logistikzentrum",
        "Poststraße 2",
        "Bonn",
        "North Rhine-Westphalia",
        "Germany",
        "53111",
        "+49-228-2270-1900",
        "bonn@warehouse.de");

    public static Warehouse Mannheim => new(
        WarehouseCode.SIN,
        "Mannheim Warenlager",
        "Planken 10",
        "Mannheim",
        "Baden-Württemberg",
        "Germany",
        "68161",
        "+49-621-2270-2000",
        "mannheim@warehouse.de");

    public static Warehouse Karlsruhe => new(
        WarehouseCode.HKG,
        "Karlsruhe Distributionszentrum",
        "Kaiserstraße 100",
        "Karlsruhe",
        "Baden-Württemberg",
        "Germany",
        "76133",
        "+49-721-2270-2100",
        "karlsruhe@warehouse.de");

    public static Fin<Warehouse> From(string code) =>
        Optional(_all.FirstOrDefault(w => w.Code.ToString().Equals(code, StringComparison.OrdinalIgnoreCase)))
            .ToFin(ValidationError.New($"Invalid warehouse code '{code}'"));

    public static Warehouse FromUnsafe(string code) =>
        Optional(_all.FirstOrDefault(w => w.Code.ToString().Equals(code, StringComparison.OrdinalIgnoreCase)))
            .IfNone(() => MainWarehouse);

    public static IEnumerable<Warehouse> Like(IEnumerable<string> codes)
    {
        return _all.Where(warehouse => codes.Any(s => warehouse.Code.ToString().Contains(s, StringComparison.OrdinalIgnoreCase)));
    }

    public virtual bool Equals(Warehouse? other)
    {
        return other is not null && Code == other.Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public override string ToString() => $"{Name} ({Code}) - {City}, {Country}";
}
