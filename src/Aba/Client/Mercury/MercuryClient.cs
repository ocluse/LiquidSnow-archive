using System;
using System.Threading.Tasks;
using Thismaker.Aba.Client.Core;
using MClient = Thismaker.Mercury.Client;

namespace Thismaker.Aba.Client.Mercury
{
    public abstract class MercuryClient<TClient>:CoreClient<TClient> where TClient:MercuryClient<TClient>
    {
        private MClient Client { get; set; }

        #region Properties

        public int Port { get; set; }
        #endregion

        #region Init
        public override void MakeApp()
        {
            Client = new MClient
            {
                ServerPort = Port,
                ServerAddress = BaseAddress
            };
            base.MakeApp();
        }

        public async Task Connect()
        {
            await Client.ConnectAsync();

            await Negociate();

            await Client.Send()
        }

        public async Task Negociate()
        {
            //Send the hello:
        }
        #endregion

    }
}