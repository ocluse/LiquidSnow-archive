using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Thismaker.Core.Models
{
    public class UniversalMessage : INotifyPropertyChanged
    {

        #region Properties

        private string sender, header, content;

        /// <summary>
        /// The sender is the source of the message
        /// </summary>
        public virtual string Sender
        {
            get { return sender; }
            set { SetField(ref sender, value, nameof(Sender)); }
        }

        /// <summary>
        /// The header of the message acts as the title of the message
        /// </summary>
        public virtual string Header
        {
            get { return header; }
            set { SetField(ref header, value, nameof(Header)); }
        }

        /// <summary>
        /// The Content is the actual content in the body of the message
        /// </summary>
        public virtual string Content
        {
            get { return content; }
            set { SetField(ref content, value, nameof(Content)); }
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


        #region INotifyImpelmentation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

    }
}
