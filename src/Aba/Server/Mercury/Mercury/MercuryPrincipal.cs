using System.Security.Claims;

namespace Thismaker.Aba.Server.Mercury
{
    public class MercuryPrincipal : MercuryClientBase
    {
        #region Properties
        public string ConnectionId
        {
            get => _connectionId;
        }

        public ClaimsPrincipal Principal { get; private set; }

        public string UserIdentifier
        {
            get => Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        #endregion

        #region Public Methods

        public MercuryPrincipal(string connectionId, MercuryServer server, ClaimsPrincipal principal) : base(connectionId, server)
        {
            Principal = principal;
        }
        #endregion
    }

    
}
