namespace NBasis.Handling
{
    public class DuplicateHandlersException : Exception
    {
        public DuplicateHandlersException(Type inputType)
            : base(string.Format("Duplicate handlers were found for type {0}", inputType))
        {
        }
    }
}
