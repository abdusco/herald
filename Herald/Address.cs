using System;

namespace Herald
{
    public class Address : IEquatable<Address>
    {
        public string EmailAddress { get; }
        public string Name { get; }

        /// <summary>
        /// Email address
        /// </summary>
        /// <param name="emailAddress">john.doe@example.com</param>
        /// <param name="name">John Doe</param>
        public Address(string emailAddress, string name = null)
        {
            if (!(emailAddress ?? "").Contains("@"))
            {
                throw new ArgumentException("Invalid email", nameof(emailAddress));
            }

            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
            Name = name;
        }

        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EmailAddress == other.EmailAddress;
        }

        public override bool Equals(object obj)
            => ReferenceEquals(this, obj) || obj is Address other && Equals(other);

        public override int GetHashCode()
            => EmailAddress.GetHashCode();
    }
}