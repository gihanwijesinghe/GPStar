namespace GPStarAPI.Errors
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public ErrorType ErrorType { get; set; }
    }

    public enum ErrorType
    {
        ArgumentException = 0,
        NotFound = 1
    }
}
