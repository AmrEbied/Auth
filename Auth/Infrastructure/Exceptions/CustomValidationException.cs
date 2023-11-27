namespace Auth.Infrastructure.Exceptions
{
    public class CustomValidationException : Exception
    {
        public List<string> Errors { get; }

        public CustomValidationException(List<string> errors, string message = null)
         : base(message ?? string.Join(", ", errors))
        {
            Errors = errors ?? new List<string>();
        }
    }
}
