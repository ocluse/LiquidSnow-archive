using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Classical
{
	/// <summary>
	/// The base class upon which classical algorithms, such as Playfair, are built.
	/// </summary>
    public abstract class ClassicalAlgorithm : DictionaryAttackerBase
    {
		/// <summary>
		/// The alphabet, i.e set of characters, to be used by the algorithm
		/// </summary>
		public virtual Alphabet Alphabet { get; set; }

		/// <summary>
		/// The key used when running the algorithm.
		/// </summary>
		public virtual string Key { get; set; }

		/// <summary>
		/// Runs the algorithm on the input, producing a ciphertext
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public virtual string Encrypt(string input)
        {
			return Run(input, true);
        }

		/// <summary>
		/// Runs the algorithm in reverse, producing a plaintext
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
        public virtual string Decrypt(string input)
        {
			return Run(input, false);
        }

		/// <summary>
		/// When implemented in a derived class, runs the specified algorithm
		/// </summary>
		/// <param name="input">The input to run the algorithm on, e.g plaintext intended for encryption</param>
		/// <param name="forward">The direction to run the algorithm in. If true, the call is equal to <see cref="Encrypt(string)"/></param>
		/// <returns>A ciphertext or plaintext, depending on the direction</returns>
		public abstract string Run(string input, bool forward);

		#region Hack

		///<inheritdoc/>
		public override List<AttackPossibility> Hack(string input)
        {
			var storedKey = Key;
			var result = new List<AttackPossibility>();
            try
            {
				if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");

				foreach (var key in KeyDictionary)
				{
					Key = key;
					var output = Decrypt(input);
					if (CheckDictionary(output))
					{
						var poss = new AttackPossibility(key, output);
						result.Add(poss);
					}
				}
			}
            catch
            {
				throw;
			}
            finally
            {
				Key = storedKey;
			}
			return result;
		}

		///<inheritdoc/>
        public override async Task<List<AttackPossibility>> HackAsync(string input, CancellationToken cancellationToken = default, IProgress<float> progress = null)
        {
			return await Task.Run(() =>
			{
				var storedKey = Key;

				var result = new List<AttackPossibility>();
				try
				{
					if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");
					float index = 0;
					float size = KeyDictionary.Count;
					foreach (var key in KeyDictionary)
					{
						//Check if we have been cancelled;
						if (cancellationToken != default
							&& cancellationToken.IsCancellationRequested)
						{
							result.Clear();
							Key = storedKey;
							cancellationToken.ThrowIfCancellationRequested();
						}

						//Run the decryption
						Key = key;
						var output = Decrypt(input);
						if (CheckDictionary(output))
						{
							var poss = new AttackPossibility(key, output);
							result.Add(poss);
						}

						index++;
						//Report on progress:
						if (progress != null)
						{
							progress.Report(index / size);
						}
					}

					return result;
				}
				catch
				{
					throw;
				}
				finally
				{
					Key = storedKey;
				}
			});
			
		}
    }

	#endregion
}