namespace GPStar.Utils
{
    public class AppResult
    {
        public bool _result { get; set; }
        public List<ErrorModel> Errors { get; set; }

        public ErrorType _errorType { get; set; }

        public bool Result { get { return _result; } }


        public ErrorType? ErrorType { get { return _errorType; } }

    }

    public class AppResult<T> : AppResult
    {
        public T _data { get; set; }
        public T Data { get { return _data; } }

        public static AppResult<T> Success = new AppResult<T> { _result = true };

        public static AppResult<T> Fail(List<ErrorModel> errors)
        {
            return new AppResult<T> { _result = false, Errors = errors };
        }

        public static AppResult<T> Fail(ErrorModel error)
        {
            return new AppResult<T> { _result = false, Errors = new List<ErrorModel>() { error }, _errorType = error.ErrorType };
        }

        public static AppResult<T> Value(T value)
        {
            return new AppResult<T> { _result = true, _data = value };
        }
    }
}
