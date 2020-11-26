using System;
using System.Reflection;
using Xunit;

namespace AbdusCo.Herald.Tests
{
    public class EmailTests
    {
        [Fact]
        public void CannotCreateInvalidAddress()
        {
            Assert.Throws<ArgumentException>(() => new Address("invalid"));
        }

        [Fact]
        public void CanCreateValidAddress()
        {
            var e = new Address("email@address");
            Assert.Equal("email@address", e.EmailAddress);
        }

        [Fact]
        public void CannotCreateEmailWithInvalidAddresses()
        {
            Assert.Throws<ArgumentException>(() => new Email(
                from: "from",
                to: "to",
                subject: "subjec",
                bodyHtml: "body"
            ));
        }

        [Fact]
        public void CannotSetInvalidAddresses()
        {
            var e = new Email(new Address("hey@example.com"));
            Assert.Throws<ArgumentNullException>(() => e = e.WithTo(null));
            Assert.Throws<ArgumentNullException>(() => e = e.WithCc(null));
            Assert.Throws<ArgumentNullException>(() => e = e.WithBcc(null));
        }

        [Fact]
        public void CanAddMultipleRecipients()
        {
            var e = new Email(new Address("a@example.com"))
                .WithTo(new Address("1@example.com"))
                .WithTo(new Address("2@example.com"));
            Assert.Equal(2, e.ToAddresses.Count);
        }

        [Fact]
        public void CanAddMultipleCcRecipients()
        {
            var e = new Email(new Address("a@example.com"))
                .WithCc(new Address("1@example.com"))
                .WithCc(new Address("2@example.com"));
            Assert.Equal(2, e.CcAddresses.Count);
        }

        [Fact]
        public void CanAddMultipleBccRecipients()
        {
            var e = new Email(new Address("a@example.com"))
                .WithBcc(new Address("1@example.com"))
                .WithBcc(new Address("2@example.com"));
            Assert.Equal(2, e.BccAddresses.Count);
        }
    }
}