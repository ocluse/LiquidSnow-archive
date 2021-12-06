using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Timers;
namespace Thismaker.Mercury
{
    public class ClientConnection
    {
        #region Private Fields
        private readonly TcpClient _tcpClient;
        private readonly Timer _pingTimer, _timeoutTimer;
        private TaskCompletionSource<bool> _tcsTimeout, _tcsDisconnect;
        private bool _connected;
        private readonly Stream _stream;
        private readonly Server _server;
        #endregion

        #region Properties
        /// <summary>
        /// The connection Id of the client, uniquely assigend when the client is created.
        /// </summary>
        public string ConnectionId { get; private set; }

        #endregion

        #region Events
        /// <summary>
        /// Fired whenever the connected client sends data. Not fired if the data is a ping message.
        /// </summary>
        public event Action<ClientConnection, byte[]> Received;

        /// <summary>
        /// Fired when the client is closed. Exception is null if closure was graceful
        /// </summary>
        public event Action<ClientConnection, Exception> Closed;
        #endregion

        /// <summary>
        /// Creates a new instance of a client connection. Should be part of a <see cref="Server"/>
        /// </summary>
        /// <param name="client">The tcp client to associate with the client connection</param>
        /// <param name="pingInterval">If provided and greater than 0, then the connection periodically
        /// sends ping messages at the provided interval</param>
        public ClientConnection(TcpClient client, Server server)
        {
            ConnectionId = Horus.Horus.GenerateId();
            _tcpClient = client;
            _server = server;

            if (_server.Certificate != null)
            {
                _stream = new SslStream(_tcpClient.GetStream(), false);
                try
                {
                    ((SslStream)_stream)
                        .AuthenticateAsServer(_server.Certificate, false, SslProtocols.Tls12, true);
                }
                catch (AuthenticationException)
                {
                    _stream.Dispose();
                    _tcpClient.Close();
                    throw;
                }
            }
            else
            {
                _stream = client.GetStream();
            }

            _connected = true;

            Task.Run(RunClient);

            if (_server.PingInterval.HasValue && _server.PingInterval.Value > 0)
            {
                _pingTimer = new Timer(_server.PingInterval.Value)
                {
                    AutoReset = true,
                    Enabled = true
                };

                _pingTimer.Elapsed += OnPingTimerElapsed;
                
                _timeoutTimer = new Timer(server.TimeoutMiliseconds)
                {
                    Enabled = false,
                    AutoReset=false
                };
                _timeoutTimer.Elapsed += OnTimeout;
            }
        }

        #region Pinging
        private async void OnPingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _tcsTimeout = new TaskCompletionSource<bool>();
            _pingTimer.Stop();
            _timeoutTimer.Start();
            _timeoutTimer.Enabled = true;

            try
            {
                await SendAsync(Globals.Ping);
                await _tcsTimeout.Task;
                _pingTimer.Start();
                _tcsTimeout = null;
            }
            catch (IOException ex)
            {
                Close(ex);
            }
            catch (OperationCanceledException)
            {
                Close(new TimeoutException());
            }
            finally
            {
                _timeoutTimer.Stop();
                _timeoutTimer.Enabled = false;
            }
        }

        private void OnTimeout(object sender, ElapsedEventArgs e)
        {
            _tcsTimeout.SetCanceled();
        }

        #endregion

        #region Private Methods

        private void Close(Exception ex)
        {
            _connected = false;
            _tcpClient.Close();
            if (_pingTimer != null)
            {
                _pingTimer.Elapsed -= OnPingTimerElapsed;
                _pingTimer.Dispose();
            }
            if (_timeoutTimer != null)
            {
                _timeoutTimer.Elapsed -= OnTimeout;
                _timeoutTimer.Dispose();
            }
            Closed?.Invoke(this, ex);
        }

        private async Task RunClient()
        {
            while (_connected)
            {
                try
                {
                    var bufferSize = _tcpClient.Client.Available;

                    if (bufferSize == 0) continue;

                    var buffer = new byte[bufferSize];
                    var readbytes = await _stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);

                    if (readbytes < bufferSize)
                    {
                        Array.Resize(ref buffer, readbytes);
                    }

                    //check if its a routine message
                    if (buffer.Length == 12)
                    {
                        if (buffer.Compare(Globals.AckPing))
                        {
                            _tcsTimeout.SetResult(true);
                            continue;
                        }

                        else if (buffer.Compare(Globals.Disconnect))
                        {
                            await SendAsync(Globals.AckDisconnect).ConfigureAwait(false);
                            Close(null);
                            break;
                        }
                        else if (buffer.Compare(Globals.AckDisconnect))
                        {
                            _tcsDisconnect.SetResult(true);
                            break;
                        }
                    }

                    Received?.Invoke(this, buffer);
                }
                catch (Exception ex)
                {
                    Close(ex);
                }
            }
        }

        #endregion

        #region Public Methods

        public async Task DisconnectAsync()
        {
            if (!_connected) throw new InvalidOperationException("Client connection is already inactive");

            var timer = new Timer(_server.DisconnectTimeoutMilliseconds)
            {
                Enabled = true,
            };

            timer.Elapsed += (o, e) =>
            {
                _tcsDisconnect.SetCanceled();
            };

            _tcsDisconnect = new TaskCompletionSource<bool>();

            try
            {
                await SendAsync(Globals.Disconnect).ConfigureAwait(false);
                await _tcsDisconnect.Task.ConfigureAwait(false);
                Close(null);
            }
            catch(OperationCanceledException)
            {
                Close(new TimeoutException("Graceful disconnect failed"));
            }
            catch (Exception ex)
            {
                Close(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        public async Task SendAsync(byte[] data)
        {
            try
            {
                await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
            catch(IOException ex)
            {
                Close(ex);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
