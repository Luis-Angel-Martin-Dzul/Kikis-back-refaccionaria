namespace Kikis_back_refaccionaria.Core.Responses {
    public class UserRES {

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Curp { get; set; } = null!;

        public GenericCatalog Rol { get; set; }

    }
}
