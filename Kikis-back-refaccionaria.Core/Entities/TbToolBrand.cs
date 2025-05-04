namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbToolBrand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TbTool> TbTools { get; set; } = new List<TbTool>();
}
