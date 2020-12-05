using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Mercury.Azure.Blobs
{
    public interface ITransferAuthenticator
    {
        Task<Uri> GetSASToken(string cloudName);
    }
}
