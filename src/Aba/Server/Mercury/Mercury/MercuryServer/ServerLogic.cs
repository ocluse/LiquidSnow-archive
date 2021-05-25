using System.Collections.Generic;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public partial class MercuryServer
    {
        private Dictionary<string, MercuryUser> _users;
        private Dictionary<string, Group> _groups;

        #region Users

        public virtual MercuryUser User(string userId)
        {
            if (_users == null) return null;
            if (_users.ContainsKey(userId)) return null;
            return _users[userId];
        }

        public virtual MercuryUser UserWithConnectionId(string connectionId)
        {
            if (_users == null) return null;

            foreach (var user in _users)
            {
                if (user.Value.HasConnectionId(connectionId)) return user.Value;
            }

            return null;
        }

        public IBeamer Client(string connectionId)
        {
            if (!_mServer.HasClient(connectionId)) return null;

            var clientBase = new MercuryClientBase(connectionId, this);

            return clientBase;
        }

        private async Task DisconnectClientAsync(string connectionId, string reason)
        {
            //create the special payload:
            var payload = new RPCPayload
            {
                Parameters = new List<string> { reason },
                MethodName = Globals.CloseConnection
            };

            await SendPayloadAsync(connectionId, payload).ConfigureAwait(false);

            await _mServer.DisconnectClientAsync(connectionId);
        }

        internal virtual void AddUserConnectionId(string connectionId, string userId)
        {
            MercuryUser user;

            if (_users == null) _users = new Dictionary<string, MercuryUser>();

            if (_users.ContainsKey(userId))
            {
                user = _users[userId];
            }
            else
            {
                user = new MercuryUser(userId, this);
            }

            user.AddConnectionId(connectionId);
        }

        #endregion

        #region Groups

        public Group Group(string groupName)
        {
            if (_groups == null) return null;
            if (_groups.ContainsKey(groupName))
            {
                return _groups[groupName];
            }
            return null;
        }

        public void AddToGroup(string connectionId, string groupName)
        {
            if (_mServer.HasClient(connectionId))
            {
                Group group;
                if (_groups == null) _groups = new Dictionary<string, Group>();

                if (_groups.ContainsKey(groupName)) group = _groups[groupName];
                else group = new Group(this, groupName);

                group.AddConnectionId(connectionId);
            }
        }

        public void RemoveFromGroup(string connectionId, string groupName)
        {
            if (_groups == null) return;
            if (!_groups.ContainsKey(groupName)) return;
            _groups[groupName].RemoveFromGroup(connectionId);
        }

        #endregion
    }
}