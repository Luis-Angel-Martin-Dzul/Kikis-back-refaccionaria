namespace Kikis_back_refaccionaria.Core.Responses {

    public class ClientRES {

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Cellphone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
