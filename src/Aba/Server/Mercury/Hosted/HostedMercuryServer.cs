using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Server.Mercury.Hosted
{
    public abstract class HostedMercuryServer : MercuryServer, IHostedService
    {
        private HostedMercuryManager _manager;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        #region HostedServiceImplements
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopAsync();
        }

        #endregion

        internal void SetManager(HostedMercuryManager manager)
        {
            _manager = manager;
        }

        internal override void AddUserConnectionId(string connectionId, string userId)
        {
            _manager.AddUserConnectionId(connectionId, userId);
        }

        public override MercuryUser User(string userId)
        {
            return _manager.GetUser(userId, this);
        }

        public override MercuryUser UserWithConnectionId(string connectionId)
        {
            return _manager.GetUserWithConnectionId(connectionId, this);
        }

        protected override T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        protected override object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, _jsonOptions);
        }

        protected override string Serialize(object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type, _jsonOptions);
        }

        protected override string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _jsonOptions);
        }

        protected override async Task<ClaimsPrincipal> ValidateAccessToken(string accessToken, List<string> scopes)
        {
            return await _manager.ValidateAccessToken(accessToken);
        }
    }
}
