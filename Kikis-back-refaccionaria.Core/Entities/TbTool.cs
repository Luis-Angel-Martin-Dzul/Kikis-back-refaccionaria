namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbTool
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public int Brand { get; set; }

    public int Category { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public string? Path { get; set; }

    public bool IsActive { get; set; }

    public virtual TbToolBrand BrandNavigation { get; set; } = null!;

    public virtual TbToolCategory CategoryNavigation { get; set; } = null!;
}
