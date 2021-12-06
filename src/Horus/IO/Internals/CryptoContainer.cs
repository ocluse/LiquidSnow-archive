using System;
using System.IO;
using System.Linq;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Threading;
using Thismaker.Core.Utilities;
using System.Text;

namespace Thismaker.Horus.IO
{
    
    internal class CryptoContainer : ICryptoContainer
    {
        #region Private Fields

        private readonly Package _package;

        private readonly Stream _source;

        #endregion

        #region Constructors

        public CryptoContainer(string key)
        {
            Key = key;
            _source = new MemoryStream();
            _package = Package.Open(_source, FileMode.OpenOrCreate);
        }

        public CryptoContainer(Stream stream, string key)
        {
            Key = key;
            _source = stream;
            _package = Package.Open(stream, FileMode.OpenOrCreate);
        }

        #endregion

        #region Properties
        
        public string Key { get; }

        #endregion

        #region Stream IO
        
        public async Task AddAsync(string name, Stream input, bool overwrite = false, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));

            PackagePart part = _package.PartExists(uri)
                ? overwrite ? _package.GetPart(uri) : throw new IOException("Item already exists")
                : _package.CreatePart(uri, "");

            Stream output = part.GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Key, output);
            await ef.WriteAsync(input, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetAsync(string name, Stream output, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));

            if (!_package.PartExists(uri))
            {
                throw new FileNotFoundException("Item does not exist");
            }

            Stream input = _package.GetPart(uri).GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Key, input);
            await ef.ReadAsync(output, progress, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Object IO
        public async Task AddAsync<T>(string name, T o, bool overwrite = false)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));

            PackagePart part = _package.PartExists(uri)
                ? overwrite ? _package.GetPart(uri) : throw new InvalidOperationException("Part already exists")
                : _package.CreatePart(uri, "");

            Stream output = part.GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Key, output);
            await ef.SerializeAsync(o).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string name)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));

            if (!_package.PartExists(uri))
            {
                throw new FileNotFoundException("The specified item does not exist");
            }

            Stream input = _package.GetPart(uri).GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Key, input);
            return await ef.DeserializeAsync<T>().ConfigureAwait(false);
        }
        #endregion

        #region Byte IO

        public async Task AddBytesAsync(string name, byte[] data, bool overwrite = false, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            using MemoryStream msData = new MemoryStream(data);
            await AddAsync(name, msData, overwrite, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytesAsync(string name, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            using MemoryStream msData = new MemoryStream();
            await GetAsync(name, msData, progress, cancellationToken).ConfigureAwait(false);
            return msData.ToArray();
        }
        #endregion

        #region String IO
        public async Task AddTextAsync(string name, string contents, bool overwrite = false, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            await AddBytesAsync(name, contents.GetBytes<UTF8Encoding>(), overwrite, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> GetTextAsync(string name, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            byte[] bytes = await GetBytesAsync(name, progress, cancellationToken).ConfigureAwait(false);
            return bytes.GetString<UTF8Encoding>();
        }
        #endregion

        #region Misc Methods
        public async Task ExtractContainerAsync(string outputDirecotry, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(outputDirecotry))
            {
                _ = Directory.CreateDirectory(outputDirecotry);
            }
            PackagePartCollection parts = _package.GetParts();

            int index = 0;
            int count = parts.Count();

            Progress<double> innerProgress = new Progress<double> { };
            innerProgress.ProgressChanged += (o, e) =>
            {
                double percent = (index + e) / count;
                progress.Report(percent);
            };

            if (progress == null)
            {
                innerProgress = null;
            }

            foreach (PackagePart part in parts)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                string name = part.Uri.ToString();
                string path = IOUtility.CombinePath(outputDirecotry, name);
                using FileStream fs = File.OpenWrite(path);
                await GetAsync(name, fs, innerProgress);
                index++;
            }
        }

        public bool Exists(string name)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));
            return _package.PartExists(uri);
        }

        public bool Delete(string name)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));
            if (_package.PartExists(uri))
            {
                try
                {
                    _package.DeletePart(uri);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public void Dispose()
        {
            _package.Close();
            _source.Dispose();
        }

        #endregion
    }
}
