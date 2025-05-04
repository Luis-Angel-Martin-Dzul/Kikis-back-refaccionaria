namespace Kikis_back_refaccionaria.Core.Entities;

public partial class TbUser
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public int Rol { get; set; }

    public DateTime CreateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual TbRol RolNavigation { get; set; } = null!;

    public virtual ICollection<TbSale> TbSales { get; set; } = new List<TbSale>();
}
