namespace FCG.Presentation.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Message = string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> SuccessMethod(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorMethod(string message, T data = default(T))
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}