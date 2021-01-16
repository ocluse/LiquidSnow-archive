using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common;

namespace Thismaker.Aba.Server.Authentication
{
    public interface IAbaTokenKeeper
    {
        public bool CheckIfTokenExists(string userId, string token);

        public List<string> GetRequiredClaims();

        public List<Claim> MakeClaims<T>(T userId);
    }
}
