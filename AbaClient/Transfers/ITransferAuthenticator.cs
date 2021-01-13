using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.Transfers
{
    public interface ITransferAuthenticator
    {
        Task<Uri> GetSASToken(string cloudName);
    }
}
