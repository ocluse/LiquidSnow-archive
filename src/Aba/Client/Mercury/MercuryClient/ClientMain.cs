using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;
using MClient = Thismaker.Mercury.Client;

namespace Thismaker.Aba.Client.Mercury
{
    public abstract partial class MercuryClientBase<TClient> : ClientBase<TClient> where TClient : MercuryClientBase<TClient>
    {
        private MClient _mClient;

        #region Properties

        public int Port { get; set; }
        #endregion

        #region Events

        public event Action Connected;

        #endregion

        #region Init
        public override void MakeApp()
        {
            _mClient = new MClient
            {
                ServerPort = Port,
                ServerAddress = ReadBaseAddress()
            };
            base.MakeApp();
        }

        public async Task ConnectAsync()
        {
            await _mClient.ConnectAsync(false);

            _mClient.Received += OnReceive;

            Connected?.Invoke();
        }

        private async void OnReceive(byte[] obj)
        {
            //check the nature of the message:

            if (obj.Length == 12)
            {
                if (obj.Compare(Globals.AuthSelf))
                {
                    RPCPayload authLoad = new RPCPayload
                    {
                        MethodName = Globals.AuthResponsePayload,
                        AccessToken = AccessToken.Value,

                    };

                    await SendPayloadAsync(authLoad).ConfigureAwait(false);
                    return;
                }
            }

            //convert to string:
            string json = obj.GetString<UTF8Encoding>();
            RPCPayload payload = Deserialize<RPCPayload>(json);

            PayloadReceived(payload);
        }

        private string ReadBaseAddress()
        {
            return BaseAddress.StartsWith("https:") ?
                BaseAddress.Substring(8) :
                BaseAddress.StartsWith("http:") ?
                BaseAddress.Substring(7) :
                BaseAddress;
        }

        protected abstract string Serialize(object obj, Type type);

        protected abstract object Deserialize(string json, Type type);

        #endregion
    }
}