using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Classical
{
    public abstract class DictionaryAttackerBase
    {
		protected bool dictLoaded = false;

        #region Properties
        public int MatchLength { get; set; } = 3;
		public List<string> KeyDictionary { get; set; }
		public List<string> LanguageDictionary { get; set; }
        #endregion

        #region Dictionary Methods
        /// <summary>
        /// Loads a dictionary to be used for attack.
        /// </summary>
        /// <param name="path">The path to the file containing the words.
        /// This should be a text file with each line containing a single word</param>
        /// <param name="type">The type of dictionary. See DictionaryType for more info</param>
        public void LoadDictionary(string path, DictionaryType type)
		{
			using var msLoad = new MemoryStream(File.ReadAllBytes(path));
			LoadDictionary(msLoad, type);
		}

		/// <summary>
		/// Loads a dictionary to be used for attack.
		/// </summary>
		/// <param name="stream">The stream to load the dictionary from</param>
		/// <param name="type">The type of dictionary. See DictionaryType for more info</param>
		public void LoadDictionary(Stream stream, DictionaryType type)
		{
			KeyDictionary = new List<string>();
			LanguageDictionary = new List<string>();

			using var strReader = new StreamReader(stream);

			while (true)
			{
				var str = strReader.ReadLine();
				if (string.IsNullOrEmpty(str)) break;

				if (type == DictionaryType.Key || type == DictionaryType.Combined)
				{
					KeyDictionary.Add(str);
				}

				if (type == DictionaryType.Language || type == DictionaryType.Combined)
				{
					LanguageDictionary.Add(str);
				}
			}

			dictLoaded = true;
		}

		/// <summary>
		/// Returns true if a similar value is found in the dictionary,
		/// depending on the provided match length
		/// </summary>
		/// <param name="input">The string to check in the dictionary</param>
		/// <returns></returns>
		public bool CheckDictionary(string input)
        {
			return CheckDictionary(input, MatchLength);
        }

		private bool CheckDictionary(string input, int depth)
		{
			//Hello my name is regret
			//I'm pretty sure we have met

			if (input.Length == 0) return true;

			foreach (var word in LanguageDictionary)
			{
				if (word.Length < 3) continue;
				if (input.Contains(word))
				{
					var nextInput = input.Replace(word, "");
					if (nextInput.Length < 3) return true;
					else return CheckDictionary(nextInput, depth - 1);
				}
				continue;
			}
			return false;
		}
        #endregion

        #region Abstract Methods

        /// <summary>
        /// When overriden in a derived class, perfroms a dictionary attack on the specified input.
        /// </summary>
        /// <param name="input">The input to attack</param>
        /// <returns>A list of all possible attacks</returns>
        public abstract List<AttackPossibility> Hack(string input);

		/// <summary>
		/// When overriden in a derived class, async attack that is useful where the dictionary is large
		/// </summary>
		/// <param name="input">The input to attack, i.e ciphertext</param>
		/// <param name="cancellationToken">The cancellation token that will request cancellation of the attack</param>
		/// <param name="progress">If provided, reports on the overall progress of the process</param>
		/// <returns>A list of all possible attacks </returns>
		public abstract Task<List<AttackPossibility>> HackAsync(string input, CancellationToken cancellationToken = default, IProgress<float> progress = null);

        #endregion
    }
}