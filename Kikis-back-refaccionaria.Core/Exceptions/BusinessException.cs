namespace Kikis_back_refaccionaria.Core.Exceptions {
    public class BusinessException : Exception {
        public BusinessException() {}
        public BusinessException(string message) : base(message) {}
    }
}
