namespace Kikis_back_refaccionaria.Core.Responses {
    public class UserAuthRES {

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Curp { get; set; } = null!;

        public RolRES Rol { get; set; }

    }
}
