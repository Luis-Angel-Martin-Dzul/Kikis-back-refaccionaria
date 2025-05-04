namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbModule
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TbPermission> TbPermissions { get; set; } = new List<TbPermission>();
}
