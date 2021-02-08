using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public abstract class DictionaryAttackerBase
    {
		protected bool dictLoaded = false;

		public int MatchLength { get; set; } = 3;
		public List<string> KeyDictionary { get; set; }
		public List<string> LanguageDictionary { get; set; }

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
				if (str == null) break;

				str = str.Prepare();

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
	}
}