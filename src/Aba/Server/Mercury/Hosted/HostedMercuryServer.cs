using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Server.Mercury.Hosted
{
    public abstract class HostedMercuryServer : MercuryServer, IHostedService
    {
        HostedMercuryManager _manager;

        #region HostedServiceImplements
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start(null);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        internal void SetManager(HostedMercuryManager manager)
        {
            _manager = manager;
        }

        protected override T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        protected override object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type);
        }

        protected override string Serialize(object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type);
        }

        protected override string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize<T>(obj);
        }

        protected override async Task<IPrincipal> ValidateAccessToken(string accessToken, List<string> scopes)
        {
            return await _manager.ValidateAccessToken(accessToken);
        }
    }
}
