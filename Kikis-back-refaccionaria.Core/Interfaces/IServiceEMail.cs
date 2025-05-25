namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceEMail {

        bool SendUserPasswordEmail(string to, string password);
    }
}
