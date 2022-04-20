using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/*
 * Implementation of K-Sortable Globally Unique IDs https://github.com/segmentio/ksuid
 */

namespace NBasis.Identification
{
    public class KSuid : IComparable, IComparable<KSuid>, IEquatable<KSuid>
    {
        private const int EncodedSize = 27;
        private const int PayloadSize = 16;
        private const int TimestampSize = 4;
        private const uint Epoch = 1400000000;
        private static readonly DateTime EpochDateTime = new DateTime(2014, 05, 13, 16, 53, 20, DateTimeKind.Utc);

        readonly uint _timestamp;
        readonly byte[] _payload;

        /// <summary>
        /// Initialize a new KSuid
        /// </summary>
        /// <returns>A new KSuid object</returns>
        public static KSuid NewKSuid()
        {
            return new KSuid();
        }

        /// <summary>
        /// Convert a string KSuid into the equivalent KSuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static KSuid Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input), "Must have a value to parse");

            return FromByteArray(FromBase62(input));
        }

        /// <summary>
        /// Convert a string KSuid into the equivalent KSuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out KSuid result)
        {
            try
            {
                result = Parse(input);
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
            
            return true;
        }

        private KSuid()
        {
            _payload = new byte[PayloadSize];

            var random = RandomNumberGenerator.Create();
            random.GetNonZeroBytes(_payload);

            _timestamp = Convert.ToUInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Epoch);
        }

        public KSuid(byte[] payload, uint timestamp)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload), "Payload cannot be null");
            if (payload.Length != PayloadSize)
                throw new ArgumentException(string.Format("Invalid payload size. {0} != {1}", payload.Length, PayloadSize));

            _payload = new byte[PayloadSize];
            Array.Copy(payload, 0, _payload, 0, PayloadSize);
            _timestamp = timestamp;
        }

        public static uint GetTimestamp(DateTime value)
        {
            return Convert.ToUInt32((value - EpochDateTime).TotalSeconds);
        }

        public static KSuid FromByteArray(byte[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null");
            if (input.Length != (PayloadSize + TimestampSize))
                throw new ArgumentException(string.Format("Invalid input size. {0} != {1}", input.Length, PayloadSize + TimestampSize));

            var timestamp = new byte[TimestampSize];
            Array.Copy(input, 0, timestamp, 0, TimestampSize);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestamp);
            }

            var buffer = new byte[PayloadSize];
            Array.Copy(input, TimestampSize, buffer, 0, PayloadSize);

            return new KSuid(buffer, BitConverter.ToUInt32(timestamp, 0));
        }

        public byte[] ToByteArray()
        {
            var timestamp = BitConverter.GetBytes(_timestamp);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestamp);
            }

            var buffer = new byte[timestamp.Length + _payload.Length];
            Array.Copy(timestamp, 0, buffer, 0, timestamp.Length);
            Array.Copy(_payload, 0, buffer, timestamp.Length, _payload.Length);

            return buffer;
        }

        public byte[] Payload
        {
            get
            {
                return _payload;
            }
        }

        public uint Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public uint EpochSeconds
        {
            get 
            {
                return _timestamp + Epoch;
            }
        }

        public int CompareTo(object obj)
        {
            return obj is KSuid ksuid ? CompareTo(ksuid) : throw new ArgumentException("Cannot compare to KSuid");
        }

        public int CompareTo(KSuid other)
        {
            if (other == this)
                return 0;
            else if (this > other)
                return 1;
            else
                return -1;
        }

        public override string ToString()
        {
            return ToBase62(ToByteArray()).PadLeft(EncodedSize, '0');
        }

        public override bool Equals(object obj)
        {
            return obj is KSuid ksuid ? Equals(ksuid) : throw new ArgumentException("Cannot equate to KSuid");
        }

        public bool Equals(KSuid other)
        {
            if (other == null)
                return false;
            return other.Timestamp == Timestamp && other.Payload.SequenceEqual(Payload);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(KSuid a, KSuid b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((a is null) || (b is null))
            {
                return false;
            }

            return a.Timestamp == b.Timestamp && a.Payload.SequenceEqual(b.Payload);
        }

        public static bool operator !=(KSuid a, KSuid b)
        {
            return !(a == b);
        }

        public static bool operator >(KSuid a, KSuid b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return false;
            }

            if ((a is null) || (b is null))
            {
                return false;
            }

            if (a == b)
            {
                return false;
            }

            return (a.Timestamp > b.Timestamp);
        }

        public static bool operator <(KSuid a, KSuid b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return false;
            }

            if ((a is null) || (b is null))
            {
                return false;
            }

            if (a == b)
            {
                return false;
            }

            return (a.Timestamp < b.Timestamp);
        }

        public static implicit operator string(KSuid k) { return k.ToString(); }

        #region Base62

        private const string Base62CharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static int[] BaseConvert(int[] source, int sourceBase, int targetBase)
        {
            var result = new List<int>();
            var leadingZeroCount = Math.Min(source.TakeWhile(x => x == 0).Count(), source.Length - 1);
            int count;
            while ((count = source.Length) > 0)
            {
                var quotient = new List<int>();
                var remainder = 0;
                for (var i = 0; i != count; i++)
                {
                    var accumulator = source[i] + remainder * sourceBase;
                    var digit = accumulator / targetBase;
                    remainder = accumulator % targetBase;
                    if (quotient.Count > 0 || digit > 0)
                    {
                        quotient.Add(digit);
                    }
                }

                result.Insert(0, remainder);
                source = quotient.ToArray();
            }
            result.InsertRange(0, Enumerable.Repeat(0, leadingZeroCount));
            return result.ToArray();
        }


        private static string ToBase62(byte[] input)
        {
            var arr = Array.ConvertAll(input, t => (int)t);

            var converted = BaseConvert(arr, 256, 62);
            var builder = new StringBuilder();
            foreach (var t in converted)
            {
                builder.Append(Base62CharacterSet[t]);
            }
            return builder.ToString();
        }

        private static byte[] FromBase62(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            var arr = Array.ConvertAll(input.ToCharArray(), Base62CharacterSet.IndexOf);

            var converted = BaseConvert(arr, 62, 256);
            return Array.ConvertAll(converted, Convert.ToByte);
        }

        #endregion
    }
}
