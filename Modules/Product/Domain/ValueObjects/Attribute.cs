namespace Product.Domain.ValueObjects;

public record Attribute
{
    private Attribute() { }

    private Attribute(string name, Description description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; init; }
    public Description Description { get; init; }



    public static Fin<Attribute> Create(string name, string description)
    {
        return (Description.From(description), ValidateName(name)).Apply((desc, _) =>
            new Attribute(name, desc)).As();

        Fin<Unit> ValidateName(string _name)
        {
            return (Helpers.MaxLength200(_name, nameof(Attribute)) >>
                    Helpers.MinLength10(_name, nameof(Attribute))).ToFin();
        }
    }

    public Fin<Attribute> Update(string description)
    {
        return Description.From(description).Map(desc =>
            this with { Description = desc });
    }

    public virtual bool Equals(Attribute? other)
        => other is { } o && other.Name == Name;

    public override int GetHashCode()
        => Name.GetHashCode();
}
