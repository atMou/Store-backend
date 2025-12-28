namespace Product.Domain.Models;

public record MaterialDetail
{
    private MaterialDetail() { }
    private MaterialDetail(string detail, decimal percentage, Material material)
    {
        Detail = detail;
        Percentage = percentage;
        Material = material;
    }

    public string Detail { get; private set; }
    public decimal Percentage { get; private set; }
    public Material Material { get; private set; }

    public static Fin<MaterialDetail> From(string detail, decimal percentage, string material)
    {
        return Material.From(material).Bind(mat => percentage is >= 0 and <= 100 ?
            new MaterialDetail(detail, percentage, mat)
            : Fin<MaterialDetail>.Fail(ValidationError.New($"Percentage must be between 0 and 100.")));
    }


    public Fin<MaterialDetail> Update(string detail, decimal percentage, string material)
    {
        return MaterialDetail.From(detail, percentage, material)
            .Map(md =>
            {
                Detail = md.Detail; Percentage = md.Percentage; Material = md.Material;
                return this;
            });
    }
}
