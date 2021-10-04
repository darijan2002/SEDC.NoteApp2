namespace SEDC.NoteApp2.Dto.ValidationModels
{
    public class ValidationResponse
    {
        public ValidationResponse(string message, bool hasError)
        {
            Message = message;
            HasError = hasError;
        }

        public string Message { get; set; }
        public bool HasError { get; set; }

        public static ValidationResponse CreateSuccessValidation()
        {
            return new ValidationResponse(string.Empty, false);
        }

        public static ValidationResponse CreateErrorValidation(string messages)
        {
            return new ValidationResponse(messages, true);
        }
    }
}
