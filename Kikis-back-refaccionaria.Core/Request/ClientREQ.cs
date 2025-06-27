namespace Kikis_back_refaccionaria.Core.Request {
    public class ClientREQ {

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Cellphone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

    }
}
