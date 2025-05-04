namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbPermission
{
    public int Id { get; set; }

    public int Rol { get; set; }

    public int Module { get; set; }

    public bool CanAdd { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }

    public bool CanView { get; set; }

    public virtual TbModule ModuleNavigation { get; set; } = null!;

    public virtual TbRol RolNavigation { get; set; } = null!;
}
