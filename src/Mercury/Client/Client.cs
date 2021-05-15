using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Thismaker.Mercury
{
    public class Client
    {
        #region Private Fields
        private TcpClient _client;
        private Timer _timeoutTImer;
        private Stream _stream;
        private bool _connected;
        private string _serverAddress;
        private int _port;
        private TaskCompletionSource<bool> _tcsDisconnect;
        #endregion

        #region Events
        /// <summary>
        /// Fired in the event that the client receives data from the server
        /// </summary>
        public event Action<byte[]> Received;

        /// <summary>
        /// Fired when the client's connection to the server is closed. If the closure was graceful,
        /// then exception will be null
        /// </summary>
        public event Action<Exception> Closed;
        #endregion

        #region Properties

        /// <summary>
        /// The address(IP or hostname) of the server to connect to
        /// </summary>
        public string ServerAddress
        {
            get => _serverAddress;
            set
            {
                if (_serverAddress == value) return;
                if (_connected) throw new InvalidOperationException("Cannot change address while client is connected.");
                _serverAddress = value;
            }
        }

        /// <summary>
        /// The specific port to connect to on the server
        /// </summary>
        public int ServerPort
        {
            get => _port;
            set
            {
                if (_port == value) return;
                if (_connected) throw new InvalidOperationException("Cannot change port while client is connected");
                _port = value;
            }
        }

        /// <summary>
        /// If provided, the length of time in milliseconds after which the client disconnects 
        /// if nothing is heard from the server
        /// </summary>
        public double? TimeoutMilliseconds { get; set; }

        /// <summary>
        /// The amout of time to wait until an attempted graceful disconnect is considered ungraceful
        /// </summary>
        public double DisconnectTimeoutMilliseconds { get; set; } = 5000;

        /// <summary>
        /// Returns true while the client is connected and listening for messages and is able to send messages.
        /// Otherwise false.
        /// </summary>
        public bool Connected
        {
            get => _connected;
        }

        #endregion
        
        #region Initialization
        /// <summary>
        /// Connects to the server with the <see cref="ServerAddress"/> and <see cref="ServerPort"/>. 
        /// Throws an <see cref="InvalidOperationException"/> when the client is already connected.
        /// </summary>
        /// <param name="authenticate">If true, the client will require that the server provide a certificate for sercure TLS connection</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AuthenticationException"></exception>
        public async Task ConnectAsync(bool authenticate)
        {
            //throw an error if we are already connected
            if (_connected) 
                throw new InvalidOperationException("Client already connected");
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ServerAddress, ServerPort).ConfigureAwait(false);

                if (authenticate)
                {
                    _stream = new SslStream(_client.GetStream(), false,
                        new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

                    await ((SslStream)_stream).AuthenticateAsClientAsync(ServerAddress).ConfigureAwait(false);
                    _stream.Dispose();
                    _client.Close();
                }
                else
                {
                    _stream = _client.GetStream();
                }

            }
            catch
            {
                throw;
            }
            
            _connected = true;

            //Start listening to traffic
            _ = Task.Run(ListenAsync);

            //When a value for timeout has been provided, set the client to timeout after that while
            if (TimeoutMilliseconds.HasValue && TimeoutMilliseconds.Value > 0)
            {
                _timeoutTImer = new Timer(TimeoutMilliseconds.Value)
                {
                    AutoReset = true,
                    Enabled = true,
                };
                _timeoutTImer.Elapsed += OnTimeout;
            }
        }
        #endregion

        #region Private Methods
        
        //Called when the client times out
        private void OnTimeout(object sender, ElapsedEventArgs e)
        {
            Close(new TimeoutException());
        }

        //Called to stop the connection
        private void Close(Exception ex)
        {
            _connected = false;
            _stream.Dispose();
            _client.Close();
            if (_timeoutTImer != null)
            {
                _timeoutTImer.Elapsed -= OnTimeout;
                _timeoutTImer.Dispose();
            }
            Closed?.Invoke(ex);
        }

        //Waits for information from the server
        private async Task ListenAsync()
        {
            while (_connected)
            {
                try
                {
                    if (_timeoutTImer != null)
                    {
                        _timeoutTImer.Stop();
                        _timeoutTImer.Start();
                    }

                    var bufferSize = _client.Available;
                    if (bufferSize == 0) continue;

                    var buffer = new byte[bufferSize];
                    var readbytes = await _stream.ReadAsync(buffer, 0, bufferSize);

                    //Ensure that we read the actual amount of bytes availble in the stream
                    if (readbytes < bufferSize)
                    {
                        Array.Resize(ref buffer, readbytes);
                    }

                    //Check if its a system message
                    if (buffer.Length == 12)
                    {
                        //if ping:
                        if (buffer.Compare(Globals.Ping))
                        {
                            await SendAsync(Globals.AckPing).ConfigureAwait(false);
                            continue;
                        }
                        else if (buffer.Compare(Globals.AckDisconnect))
                        {
                            _tcsDisconnect.SetResult(true);
                            continue;
                        }
                    }

                    //Inform anyone connected to us that we just received data
                    Received?.Invoke(buffer);
                }
                catch(Exception ex)
                {
                    Close(ex);
                }
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sends the data to the server.
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns></returns>
        public async Task SendAsync(byte[] data)
        {
            if (!_connected)
                throw new InvalidOperationException("Cannot send information while client is disconnected");

            try
            {
                await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
            catch (IOException ex)
            {
                Close(ex);
                throw;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a string to the server using <see cref="UTF8Encoding"/>
        /// </summary>
        /// <param name="data">The string to send</param>
        /// <returns></returns>
        public async Task SendAsync(string data)
        {
            try
            {
                await SendAsync(data.GetBytes<UTF8Encoding>());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Call to gracefully disconnect the client from the server. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DisconnectAsync()
        {
            if (!_connected)
                throw new InvalidOperationException("Cannot disconnect if already inactive");

            var timer = new Timer(DisconnectTimeoutMilliseconds)
            {
                Enabled = true,
            };
            timer.Elapsed += (o, e) =>
            {
                _tcsDisconnect.SetCanceled();
            };

            _tcsDisconnect = new TaskCompletionSource<bool>();

            //Send to the server that we want to gracefully disconnect:
            try
            {
                await SendAsync(Globals.Disconnect).ConfigureAwait(false);
                await _tcsDisconnect.Task.ConfigureAwait(false);
                Close(null);

            }
            catch (OperationCanceledException)
            {
                Close(new TimeoutException("Graceful disconnect timed out"));
            }
            catch(Exception ex)
            {
                Close(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        /// <summary>
        /// This callback is called when authenticating a server certificate. Overide to customize the behaviour of certificate validation.
        /// This is recommended only durind development, in production, it is better to let the client's machine validate the certificate.
        /// </summary>
        public virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;

            return false;
        }

        #endregion
    }
}