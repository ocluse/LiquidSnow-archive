using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Core.Models
{
    /// <summary>
    /// <see cref="HttpContent"/> that implements see <see cref="INotifyPropertyChanged"/> that can be used to determine the progress of a stream transfer.
    /// </summary>
    public class ObservableStreamContent : HttpContent, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Represents the state of stream transfer
        /// </summary>
        public enum TransferState
        {
            /// <summary>
            /// Before the transfer has started.
            /// </summary>
            Pending,
            /// <summary>
            /// When the transfer is in progress.
            /// </summary>
            InProgress,
            /// <summary>
            /// When the transfer has been completed successfully.
            /// </summary>
            Complete,
            /// <summary>
            /// When an error has occurred during the transfer.
            /// </summary>
            Error
        }

        private const int defaultBufferSize = 4096;

        private readonly Stream _content;
        private readonly int _bufferSize;
        private bool _contentConsumed;
        private TransferState _state;
        private long _transferredBytes;
        private double _progress;
        private readonly IProgress<long> _transferredBytesProgress;
        private readonly IProgress<double> _transferProgress;
        /// <summary>
        /// Gets the current state of the transfer.
        /// </summary>
        public TransferState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        /// <summary>
        /// Gets the number of bytes that have already been transferred.
        /// </summary>
        public long TransferredBytes
        {
            get => _transferredBytes;
            private set
            {
                if(SetProperty(ref _transferredBytes, value))
                {
                    _transferredBytesProgress?.Report(value);
                }
            }
        }

        /// <summary>
        /// The length of the underlying stream in bytes.
        /// </summary>
        public long Length
        {
            get => _content.Length;
        }

        /// <summary>
        /// Gets the progress of the transfer, as a value from 0 to 1.
        /// </summary>
        public double Progress
        {
            get => _progress;
            private set
            {
                if(SetProperty(ref _progress, value))
                {
                    _transferProgress?.Report(value);
                }
            }
        }

        /// <inheritdoc cref="ObservableStreamContent(Stream,int, IProgress{long}, IProgress{double})"/>
        public ObservableStreamContent(Stream content) : this(content, defaultBufferSize) { }

        /// <inheritdoc cref="ObservableStreamContent(Stream,int, IProgress{long}, IProgress{double})"/>
        public ObservableStreamContent(Stream content, IProgress<long> transferredBytesProgress) : this(content, defaultBufferSize, transferredBytesProgress: transferredBytesProgress) { }

        /// <inheritdoc cref="ObservableStreamContent(Stream,int, IProgress{long}, IProgress{double})"/>
        public ObservableStreamContent(Stream content, IProgress<double> transferProgress) : this(content, defaultBufferSize, transferProgress: transferProgress) { }

        /// <summary>
        /// Creates a new instance of an <see cref="ObservableStreamContent"/>
        /// </summary>
        /// <param name="content">The base stream to be read from.</param>
        /// <param name="bufferSize">The buffer size.</param>
        /// <param name="transferredBytesProgress">The progress of bytes that have been successfully transferred</param>
        /// <param name="transferProgress">The % progress of the transfer, between 0-1</param>
        /// <exception cref="ArgumentOutOfRangeException">When the buffer size is less than 0</exception>
        /// <exception cref="ArgumentNullException">When the <paramref name="content"/> is null</exception>
        public ObservableStreamContent(Stream content, int bufferSize, IProgress<long> transferredBytesProgress=null, IProgress<double> transferProgress=null)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            _content = content ?? throw new ArgumentNullException(nameof(content));
            _bufferSize = bufferSize;
            _transferredBytesProgress = transferredBytesProgress;
            _transferProgress = transferProgress;
        }

        ///<inheritdoc/>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);

            PrepareContent();

            return Task.Run(() =>
            {
                byte[] buffer = new byte[_bufferSize];
                TransferredBytes = 0;

                State = TransferState.InProgress;
                _contentConsumed = true;
                using (_content)
                {
                    while (true)
                    {
                        int readBytes = _content.Read(buffer, 0, buffer.Length);

                        if (readBytes <= 0)
                        {
                            break;
                        }

                        try
                        {
                            stream.Write(buffer, 0, readBytes);
                        }
                        catch
                        {
                            State = TransferState.Error;
                            throw;
                        }

                        TransferredBytes += readBytes;
                        Progress = (double)TransferredBytes / Length;
                    }
                }

                State = TransferState.Complete;
            });
        }

        ///<inheritdoc/>
        protected override bool TryComputeLength(out long length)
        {
            length = Length;
            return true;
        }

        ///<inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PrepareContent()
        {
            if (_contentConsumed)
            {
                _content.Position = _content.CanSeek ? 0 : throw new InvalidOperationException("The stream has already been read from and does not support seeking");
            }
        }
    }
}
