namespace Kikis_back_refaccionaria.Core.Entities {
    public partial class TbSupplier
    {
        public int Id { get; set; }

        public string BusinessName { get; set; } = null!;

        public string TradeName { get; set; } = null!;

        public string Rfc { get; set; } = null!;

        public string Curp { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Cellphone { get; set; } = null!;

        public string? Cellphone2 { get; set; }

        public string Address { get; set; } = null!;

        public string Owner { get; set; } = null!;

        public string? Representative { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<TbProductSupplier> TbProductSuppliers { get; set; } = new List<TbProductSupplier>();
    }
}
