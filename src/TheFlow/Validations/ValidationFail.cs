namespace TheFlow.Validations
{
    public class ValidationFail
    {
        public string Message { get; }

        public ValidationFail(string message)
        {
            Message = message;
        }
    }
}
