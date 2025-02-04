using System.ComponentModel.DataAnnotations;

namespace NBasis.Commanding
{
    public class CommandValidationException : ValidationException
    {
        public string Code { get; }

        public CommandValidationException(string message) : base(message)
        {
        }

        public CommandValidationException(string message, string code) : base(message)
        {
            Code = code;
        }
    }
}
