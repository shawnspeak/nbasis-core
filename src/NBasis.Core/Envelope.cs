namespace NBasis
{
    /// <summary>
    /// Static factory class for <see cref="Envelope{T}"/>.
    /// </summary>
    public abstract class Envelope
    {
        /// <summary>
        /// Creates an envelope for the given body.
        /// </summary>
        public static Envelope<T> Create<T>(T body, IDictionary<string, object> headers = null)
        {
            return new Envelope<T>(body, headers);
        }
    }

    /// <summary>
    /// Provides the envelope for an object that will be sent to a bus.
    /// </summary>
    public class Envelope<T> : Envelope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        public Envelope(T body, IDictionary<string, object> headers = null)
        {
            this.Body = body;
            Headers = headers;
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public T Body { get; private set; }

        /// <summary>
        /// Gets the headers
        /// </summary>
        public IDictionary<string, object> Headers { get; private set; }
        
        /// <summary>
        /// Add a set of headers to the evelope
        /// </summary>
        /// <param name="headers"></param>
        public void AddHeaders(IDictionary<string, object> headers)
        {
            if (headers == null) return;
            Headers ??= new Dictionary<string, object>();
            headers.CopyTo(Headers);
        }
    }
}
