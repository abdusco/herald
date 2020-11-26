#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.Herald
{
    public class Email
    {
        private readonly List<Address> _toAddresses = new List<Address>();
        private readonly List<Address> _bccAddresses = new List<Address>();
        private readonly List<Address> _ccAddresses = new List<Address>();
        private readonly List<Attachment> _attachments = new List<Attachment>();
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        public ReadOnlyCollection<Address> ToAddresses => _toAddresses.AsReadOnly();
        public ReadOnlyCollection<Address> BccAddresses => _bccAddresses.AsReadOnly();
        public ReadOnlyCollection<Address> CcAddresses => _ccAddresses.AsReadOnly();
        public ReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();
        public ReadOnlyDictionary<string, string> Headers => new ReadOnlyDictionary<string, string>(_headers);
        public Address FromAddress { get; }
        public string? Subject { get; private set; }
        public string? Body { get; private set; }
        public Priority Priority { get; private set; } = Priority.Normal;
        public bool IsHtml { get; private set; } = true;

        public object? Model { get; private set; }
        public string? Template { get; private set; }

        /// <summary>
        /// Create an email
        /// </summary>
        /// <param name="from">"from" address</param>
        public Email(Address from)
        {
            FromAddress = from ?? throw new ArgumentNullException(nameof(from));
        }

        /// <summary>
        /// Compose an email quickly
        /// </summary>
        /// <example>
        /// <code>
        /// var email = new Email(
        ///     from: "me@example.com",
        ///     to: "myfriend@example.com",
        ///     subject: "hi there!",
        ///     bodyHtml: "what's up?"
        /// );
        /// </code>
        /// </example>
        /// 
        /// <param name="from">from address</param>
        /// <param name="to">recipient address</param>
        /// <param name="subject">email subject</param>
        /// <param name="bodyHtml">email body as HTML</param>
        public Email(string from, string to, string subject, string bodyHtml)
        {
            FromAddress = new Address(from ?? throw new ArgumentNullException(nameof(from)));
            _toAddresses.Add(new Address(to ?? throw new ArgumentNullException(nameof(to))));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Body = bodyHtml ?? throw new ArgumentNullException(nameof(bodyHtml));
        }

        public Email WithSubject(string subject)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            return this;
        }


        /// <summary>
        /// Add recipient "to" email address
        /// </summary>
        /// <param name="address">recipient address (recipient@email.com)</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithTo(Address address)
        {
            _toAddresses.Add(address ?? throw new ArgumentNullException(nameof(address)));
            return this;
        }


        /// <summary>
        /// Add recipient "cc" (carbon-copy) email address 
        /// </summary>
        /// <param name="address">recipient address (recipient@email.com)</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithCc(Address address)
        {
            _ccAddresses.Add(address ?? throw new ArgumentNullException(nameof(address)));
            return this;
        }

        /// <summary>
        /// Add recipient "bcc" (blind carbon-copy) email address
        /// </summary>
        /// <param name="address">recipient address (recipient@email.com)</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithBcc(Address address)
        {
            _bccAddresses.Add(address ?? throw new ArgumentNullException(nameof(address)));
            return this;
        }

        /// <summary>
        /// Add attachment
        /// </summary>
        /// <param name="attachment">attachment</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithAttachment(Attachment attachment)
        {
            _attachments.Add(attachment ?? throw new ArgumentNullException(nameof(attachment)));
            return this;
        }

        /// <summary>
        /// Add email header
        /// </summary>
        /// <param name="header">header name</param>
        /// <param name="content">content</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithHeader(string header, string content)
        {
            _headers.Add(header, content);
            return this;
        }

        /// <summary>
        /// Set body content as HTML
        /// </summary>
        /// <param name="body">HTML content</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithHtmlBody(string body)
        {
            Body = body;
            IsHtml = true;
            return this;
        }

        /// <summary>
        /// Set body content as plain text
        /// </summary>
        /// <param name="body">plain text content</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithPlainTextBody(string body)
        {
            Body = body;
            IsHtml = false;
            return this;
        }

        /// <summary>
        /// Set email <see cref="Priority"/>.
        /// </summary>
        /// <param name="priority">priority</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email WithPriority(Priority priority)
        {
            Priority = priority;
            return this;
        }

        /// <summary>
        /// Use embedded template from assembly during render
        /// </summary>
        /// <param name="embeddedTemplatePath">embedded resource path (namespace)</param>
        /// <param name="model">model to use during render</param>
        /// <param name="assembly">assembly to find template in</param>
        /// <param name="isHtml">use rendered output as HTML</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email UsingEmbeddedTemplate<T>(
            string embeddedTemplatePath,
            T model,
            Assembly? assembly = null,
            bool isHtml = true)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var template = assembly.GetEmbeddedResource(embeddedTemplatePath);
            return UsingStringTemplate(template, model, isHtml);
        }

        /// <summary>
        /// Use an existing template file during render
        /// </summary>
        /// <param name="templateFile">path to template file</param>
        /// <param name="model">model to use during render</param>
        /// <param name="isHtml">use rendered output as HTML</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email UsingFileTemplate<T>(FileInfo templateFile, T model, bool isHtml = true)
        {
            var template = templateFile.ReadAsString();
            return UsingStringTemplate(template, model, isHtml);
        }

        /// <summary>
        /// Use string template as is during render
        /// </summary>
        /// <param name="template">template as string</param>
        /// <param name="model">model to use during render</param>
        /// <param name="isHtml">use rendered output as HTML</param>
        /// <returns><see cref="Email"/> for chaining</returns>
        public Email UsingStringTemplate<T>(string template, T model, bool isHtml = true)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Model = model ?? throw new ArgumentNullException(nameof(model));
            IsHtml = isHtml;

            return this;
        }

        /// <summary>
        /// Render email body as string
        /// </summary>
        /// <param name="renderer"><see cref="IRenderer"/> instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>email body</returns>
        /// <exception cref="ConstraintException">thrown when <see cref="Body"/> or <see cref="Template"/> is not set</exception>
        public async Task<string> RenderBodyAsync(IRenderer renderer, CancellationToken cancellationToken = default)
        {
            if (Template == null && Body == null)
            {
                throw new ConstraintException($"{nameof(Body)} or {nameof(Template)} must be set");
            }

            return Template == null
                ? Body!
                : await renderer.RenderAsync(Template, Model, cancellationToken);
        }
    }
}