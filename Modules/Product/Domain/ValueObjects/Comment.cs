namespace Product.Domain.ValueObjects;


public record Comment : DomainType<Comment, string>
{
	public string Value { get; }
	private Comment() { }
	private Comment(string repr)
	{
		Value = repr;
	}
	public static Fin<Comment> From(string repr) =>
		(Helpers.MinLength10(repr, nameof(Comment)),
			Helpers.MaxLength500(repr, nameof(Comment)))
		.Apply((_, _) => new Comment(repr)).As().ToFin();

	public string To() => Value;
}
