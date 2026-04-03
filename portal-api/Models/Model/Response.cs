namespace portal_api.Models.Model
{
    public class Response
    {
        public Response(int StatusCodes, string Message, object? Data, string ErrorMessage)
        {
            this.StatusCode = StatusCodes;
            this.Message = Message;
            this.Data = Data;
            this.ErrorMessage = ErrorMessage;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
