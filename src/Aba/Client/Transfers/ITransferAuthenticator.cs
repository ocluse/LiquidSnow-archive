using System;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.Transfers
{
    public interface ITransferAuthenticator
    {
        /// <summary>
        /// When implemented in a derived class, starts the process for obtaining 
        /// a Shared Access Signature from the server needed for obtaining aceess to a blob
        /// </summary>
        /// <param name="cloudName">The name of the blob for which the SAS token is needed</param>
        /// <returns>A <see cref="Uri"/> of the relevant blob with permissions applied</returns>
        Task<Uri> GetSASToken(string cloudName);
    }
}
