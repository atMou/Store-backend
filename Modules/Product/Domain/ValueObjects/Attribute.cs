namespace Product.Domain.ValueObjects;

public record Attribute
{
    private Attribute()
    {
    }

    private Attribute(string name, Description description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public Description Description { get; private set; }

    public virtual bool Equals(Attribute? other)
        => other is { } o && other.Name == Name;


    public static Fin<Attribute> From(string name, string description)
    {
        return (Description.From(2, 200, description), ValidateName(name)).Apply((desc, _) =>
            new Attribute(name, desc)).As();

        Fin<Unit> ValidateName(string repr)
        {
            return (Helpers.MaxLength(200, nameof(Attribute))(repr) >>
                    Helpers.MinLength(2, nameof(Attribute))(repr)).ToFin();
        }
    }

    public Fin<Attribute> Update(string name, string description)
    {
        return From(name, description).Map(att =>
        {
            Name = name;
            Description = att.Description;
            return this;
        });
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, Description);


}