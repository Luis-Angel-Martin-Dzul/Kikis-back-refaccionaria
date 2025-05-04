namespace Kikis_back_refaccionaria.Core.Responses {
    public class RolRES {

        public int Id { get; set; }

        public string Name { get; set; }

        public List<ModuleRES> Modules { get; set; }
    }

    public class ModuleRES {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public PermissionRES Permissions { get; set; }
    }

    public class PermissionRES {

        public int Id { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }
    }
}
