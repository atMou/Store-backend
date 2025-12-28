namespace Product.Domain.ValueObjects;

public sealed record Material
{
    private static readonly List<Material> _all = [];

    private Material(string name)
    {
        Name = name;
        _all.Add(this);
    }

    public string Name { get; }

    // NONE
    public static readonly Material None;

    // NATURAL
    public static readonly Material Cotton;
    public static readonly Material Wool;
    public static readonly Material Silk;
    public static readonly Material Linen;
    public static readonly Material Cashmere;
    public static readonly Material Leather;
    public static readonly Material Suede;
    public static readonly Material Hemp;

    // SYNTHETIC
    public static readonly Material Polyester;
    public static readonly Material Nylon;
    public static readonly Material Acrylic;
    public static readonly Material Elastane;
    public static readonly Material Polyurethane;

    // SEMI-SYNTHETIC
    public static readonly Material Viscose;
    public static readonly Material Rayon;
    public static readonly Material Modal;
    public static readonly Material Lyocell;
    public static readonly Material Acetate;

    // FABRIC / COMMON
    public static readonly Material Denim;
    public static readonly Material Fleece;
    public static readonly Material Velvet;
    public static readonly Material Satin;
    public static readonly Material Jersey;
    public static readonly Material Corduroy;
    public static readonly Material Canvas;

    static Material()
    {
        None = new(string.Empty);

        Cotton = new(nameof(Cotton));
        Wool = new(nameof(Wool));
        Silk = new(nameof(Silk));
        Linen = new(nameof(Linen));
        Cashmere = new(nameof(Cashmere));
        Leather = new(nameof(Leather));
        Suede = new(nameof(Suede));
        Hemp = new(nameof(Hemp));

        Polyester = new(nameof(Polyester));
        Nylon = new(nameof(Nylon));
        Acrylic = new(nameof(Acrylic));
        Elastane = new(nameof(Elastane));
        Polyurethane = new(nameof(Polyurethane));

        Viscose = new(nameof(Viscose));
        Rayon = new(nameof(Rayon));
        Modal = new(nameof(Modal));
        Lyocell = new(nameof(Lyocell));
        Acetate = new(nameof(Acetate));

        Denim = new(nameof(Denim));
        Fleece = new(nameof(Fleece));
        Velvet = new(nameof(Velvet));
        Satin = new(nameof(Satin));
        Jersey = new(nameof(Jersey));
        Corduroy = new(nameof(Corduroy));
        Canvas = new(nameof(Canvas));
    }

    // UTILITIES
    public static IReadOnlyList<Material> All => _all;
    public static Fin<Material> From(string name) =>
        _all.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? FinFail<Material>(ValidationError.New("Material not found"));

    public static Material FromUnsafe(string name) =>
        Optional(_all.FirstOrDefault(material => material.Name == name)).IfNone(() => None);
}
