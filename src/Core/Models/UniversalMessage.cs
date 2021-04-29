namespace Thismaker.Core.Models
{
    /// <summary>
    /// A template that can be used for univeral messages.
    /// I thought I'd just throw this there, it may be useful, you never know.
    /// It is Binding ready, and notifies whenever there are changes.
    /// </summary>
    public class UniversalMessage : BindableBase
    {
        #region Properties

        private string sender, header, content;

        /// <summary>
        /// The sender is the source of the message
        /// </summary>
        public virtual string Sender
        {
            get { return sender; }
            set { SetProperty(ref sender, value); }
        }

        /// <summary>
        /// The header of the message acts as the title of the message
        /// </summary>
        public virtual string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }

        /// <summary>
        /// The Content is the actual content in the body of the message
        /// </summary>
        public virtual string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        /// <summary>
        /// Updates the current Universal Message with one from source
        /// </summary>
        /// <param name="source"></param>
        public virtual void Update(UniversalMessage source)
        {
            Sender = source.Sender;
            Header = source.Header;
            Content = source.Content;
        }

        /// <summary>
        /// Duplicates a universal message from the provided source, returning the duplicate
        /// </summary>
        /// <param name="source">The source to be duplicated</param>
        /// <returns></returns>
        public virtual UniversalMessage Duplicate(UniversalMessage source)
        {
            var result = new UniversalMessage
            {
                Sender = source.Sender,
                Header = source.Header,
                Content = source.Content
            };

            return result;
        }

        #endregion
    }
}
