namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbProduct
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

    public int Hallway { get; set; }

    public int Shelf { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public virtual TbProductBrand BrandNavigation { get; set; } = null!;

    public virtual TbProductCategory CategoryNavigation { get; set; } = null!;

    public virtual TbProductHallway HallwayNavigation { get; set; } = null!;

    public virtual TbProductLevel LevelNavigation { get; set; } = null!;

    public virtual TbProductShelf ShelfNavigation { get; set; } = null!;

    public virtual ICollection<TbProductKit> TbProductKits { get; set; } = new List<TbProductKit>();

    public virtual ICollection<TbProductSupplier> TbProductSuppliers { get; set; } = new List<TbProductSupplier>();
}
