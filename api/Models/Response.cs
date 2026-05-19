namespace api.Models
{
    public class Response<Type>
    {
        public Type? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
