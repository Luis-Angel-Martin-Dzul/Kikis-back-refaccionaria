using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Request {
    public class UserREQ {

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Curp { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public GenericCatalog Rol { get; set; }

    }
}
