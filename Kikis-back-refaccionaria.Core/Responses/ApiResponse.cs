namespace Kikis_back_refaccionaria.Core.Responses {
    public class ApiResponse<T> {

        public int Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public T Data { get; set; }

        public ApiResponse(T data) {
            Status = 200;
            Title = "OK";
            Detail = "";
            Data = data;
        }

        public ApiResponse(int status, string title, string detail, T data) {
            Status = status;
            Title = title;
            Detail = detail;
            Data = data;
        }

    }
}
