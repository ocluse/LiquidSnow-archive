using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Thismaker.Enigma
{
	/// <summary>
	/// The classical class is used for the encryption and decryption of text only
	/// by use of classical methods like Vigenere and Playfair
	/// </summary>
	public static class Classical
	{

		#region Cipher

		public static string VigenereCipher(string input, string key)
		{
			int indexer = 0;
			string txtPlain = input, txtCipher = "";
			txtPlain = txtPlain.Prepare();
			key = key.Prepare();
			for (int i = 0; i < txtPlain.Length; i++)
			{
				if (indexer == key.Length)
				{
					indexer = 0;
				}

				int ciph = GetLetterValue(key[indexer]) + GetLetterValue(txtPlain[i]);

				while (ciph > 25) ciph -= 26;
				txtCipher += capital_letters[ciph];
				indexer++;
			}
			return txtCipher;
		}

		public static string PlayfairCipher(string input, string key, char omit)
		{
			char[,] KeyGrid = new char[5, 5];
			string skey = key.Prepare();
			input = input.Prepare();

			for (int i = 0; i < key.Length; i++)
			{
				bool exists = false;
				for (int check = 0; check < i; check++)
				{
					if (skey[i] == skey[check]) { exists = true; break; }
				}
				if (exists) skey = skey.ReplaceAt(i, ' ');

			}

			skey = skey.Prepare();

			string salpha = "ABCDEFGHIJKLMNOPRQSTUVWXYZ";
			for (int i = 0; i < salpha.Length; i++)
			{
				bool exists = false;
				for (int check = 0; check < skey.Length; check++)
				{
					if (salpha[i] == skey[check]) exists = true;
				}
				if (exists || salpha[i] == omit)
				{
					salpha = salpha.ReplaceAt(i, ' ');
				}
			}
			salpha = salpha.Prepare();
			//Then we fit it all up in the key grid:
			skey += salpha;
			int col = 0;
			int row = 0;
			for (int i = 0; i < 25; i++)
			{
				if (col > 4)
				{
					row++;
					col = 0;
				}
				KeyGrid[row, col] = skey[i];
				col++;
			}
			//Now we can begin Cyphering:::::
			//Fist we prepare our plaintext:
			string plaintext = input.Prepare();

			if (plaintext.Length % 2 != 0) plaintext += 'Z'; //To maje it even
			string ciphertext = plaintext;
			//We begin to do the Cyphering, for real this time:
			for (int i = 0; i < (plaintext.Length / 2); i++)
			{
				//We locate our diagraphs in the grid first:
				int[] cell1 = new int[2];
				int[] cell2 = new int[2];
				//The first cell:
				col = 0;
				row = 0;
				for (int check = 0; check < 25; check++)
				{
					if (col > 4)
					{
						row++;
						col = 0;
					}
					if (plaintext[i * 2] == KeyGrid[row, col])
					{
						cell1[0] = row;
						cell1[1] = col;
					}
					if (plaintext[(i * 2) + 1] == KeyGrid[row, col])
					{
						cell2[0] = row;
						cell2[1] = col;
					}
					col++;
				}
				//Now we move to the next step, filling in the cyphertext:
				//STARTING: If they occur in different cols and rows:
				if (cell1[0] != cell2[0] && cell1[1] != cell2[1])
				{
					int temp = cell1[1];
					cell1[1] = cell2[1];
					cell2[1] = temp;

				}
				//STARTING: If they fall in same cols diff rows:
				else if (cell1[0] != cell2[0] && cell1[1] == cell2[1])
				{
					cell1[0]++;
					cell2[0]++;
				}
				//STARTING: If they fall in the same row:
				else if (cell1[0] == cell2[0] && cell1[1] != cell2[1])
				{
					cell1[1]++;
					cell2[1]++;
				}
				//STARTING: They fall in the same cell:
				else
				{
					cell1[0]++;
					cell2[0]++;
				}
				if (cell1[0] > 4) cell1[0] = 0;
				if (cell2[0] > 4) cell2[0] = 0;
				if (cell1[1] > 4) cell1[1] = 0;
				if (cell2[1] > 4) cell2[1] = 0;
				//Now we finally add the value to ciphertext, finally!!!! :)
				ciphertext = ciphertext.ReplaceAt((i * 2), KeyGrid[cell1[0], cell1[1]]);
				ciphertext = ciphertext.ReplaceAt((i * 2) + 1, KeyGrid[cell2[0], cell2[1]]);
			}



			return ciphertext;
		}

		public static string CeaserCipher(string input, char key)
		{
			string txtOutput = input;
			int valKey = GetLetterValue(key);
			for (int i = 0; i < input.Length; i++)
			{
				int newVal = valKey + GetLetterValue(input[i]);
				while (newVal > 25) newVal -= 26;

				txtOutput = txtOutput.ReplaceAt(i, capital_letters[newVal]);

			}

			return txtOutput;
		}

		#endregion

		#region Decipher
		public static string VigenereDecipher(string input, string key)
		{
			int indexer = 0;
			string txtPlain = input, txtCipher = "";
			txtPlain = txtPlain.Prepare();
			key = key.Prepare();
			for (int i = 0; i < txtPlain.Length; i++)
			{
				if (indexer == key.Length)
				{
					indexer = 0;
				}

				int ciph = GetLetterValue(txtPlain[i]) - GetLetterValue(key[indexer]);

				while (ciph < 0) ciph += 26;
				txtCipher += capital_letters[ciph];
				indexer++;
			}
			return txtCipher;
		}

		public static string PlayfairDecipher(string input, string key, char omit)
		{
			if (small_letters.Contains(omit)) omit = capital_letters[small_letters.IndexOf(omit)];

			char[,] KeyGrid = new char[5, 5];
			string skey = key.Prepare();
			input = input.Prepare();

			for (int i = 0; i < key.Length; i++)
			{
				bool exists = false;
				for (int check = 0; check < i; check++)
				{
					if (skey[i] == skey[check]) { exists = true; break; }
				}
				if (exists) skey = skey.ReplaceAt(i, ' ');

			}

			skey = skey.Prepare();

			string salpha = "ABCDEFGHIJKLMNOPRQSTUVWXYZ";
			for (int i = 0; i < salpha.Length; i++)
			{
				bool exists = false;
				for (int check = 0; check < skey.Length; check++)
				{
					if (salpha[i] == skey[check]) exists = true;
				}
				if (exists || salpha[i] == omit)
				{
					salpha = salpha.ReplaceAt(i, ' ');
				}
			}
			salpha = salpha.Prepare();
			//Then we fit it all up in the key grid:
			skey += salpha;
			int col = 0;
			int row = 0;
			for (int i = 0; i < 25; i++)
			{
				if (col > 4)
				{
					row++;
					col = 0;
				}
				KeyGrid[row, col] = skey[i];
				col++;
			}
			//Now we can begin Cyphering:::::
			//Fist we prepare our plaintext:
			string plaintext = input.Prepare();

			if (plaintext.Length % 2 != 0) plaintext += 'Z'; //To maje it even
			string ciphertext = plaintext;
			//We begin to do the Cyphering, for real this time:
			for (int i = 0; i < (plaintext.Length / 2); i++)
			{
				//We locate our diagraphs in the grid first:
				int[] cell1 = new int[2];
				int[] cell2 = new int[2];
				//The first cell:
				col = 0;
				row = 0;
				for (int check = 0; check < 25; check++)
				{
					if (col > 4)
					{
						row++;
						col = 0;
					}
					if (plaintext[i * 2] == KeyGrid[row, col])
					{
						cell1[0] = row;
						cell1[1] = col;
					}
					if (plaintext[(i * 2) + 1] == KeyGrid[row, col])
					{
						cell2[0] = row;
						cell2[1] = col;
					}
					col++;
				}
				//Now we move to the next step, filling in the cyphertext:
				//STARTING: If they occur in different cols and rows:
				if (cell1[0] != cell2[0] && cell1[1] != cell2[1])
				{
					int temp = cell1[1];
					cell1[1] = cell2[1];
					cell2[1] = temp;

				}
				//STARTING: If they fall in same cols diff rows:
				else if (cell1[0] != cell2[0] && cell1[1] == cell2[1])
				{
					cell1[0]--;
					cell2[0]--;
				}
				//STARTING: If they fall in the same row:
				else if (cell1[0] == cell2[0] && cell1[1] != cell2[1])
				{
					cell1[1]--;
					cell2[1]--;
				}
				//STARTING: They fall in the same cell:
				else
				{
					cell1[0]--;
					cell2[0]--;
				}
				if (cell1[0] < 0) cell1[0] = 4;
				if (cell2[0] < 0) cell2[0] = 4;
				if (cell1[1] < 0) cell1[1] = 4;
				if (cell2[1] < 0) cell2[1] = 4;
				//Now we finally add the value to ciphertext, finally!!!! :)
				ciphertext = ciphertext.ReplaceAt((i * 2), KeyGrid[cell1[0], cell1[1]]);
				ciphertext = ciphertext.ReplaceAt((i * 2) + 1, KeyGrid[cell2[0], cell2[1]]);
			}



			return ciphertext;
		}

		public static string CeaserDecipher(string input, char key)
		{
			string txtOutput = input;
			int valKey = GetLetterValue(key);
			for (int i = 0; i < input.Length; i++)
			{
				int newVal = GetLetterValue(input[i]) - valKey;
				while (newVal < 0) newVal += 26;

				txtOutput = txtOutput.ReplaceAt(i, capital_letters[newVal]);

			}

			return txtOutput;
		}
		#endregion

		#region Hack
		static List<string> key_dict;
		static List<string> lang_dict;

		static bool dictLoaded = false;

		/// <summary>
		/// Uses the loaded key dictionary to attack the provided input, returning only likely results
		/// based on the loaded language dictionary
		/// </summary>
		/// <param name="input">The data to be attacked</param>
		/// <param name="omit">The character to omit, if not included, Enigma will loop through all 26 characters.
		/// Specify this value to improve performance</param>
		/// <returns></returns>
		public static List<AttackPossibility> PlayfairHack(string input, char omit)
        {
			if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");

			input = input.Prepare();

			var result = new List<AttackPossibility>();
			foreach (var key in key_dict)
			{
				var output = PlayfairDecipher(input, key, omit);
				if (CheckDictionary(output, 3))
				{
					var poss = new AttackPossibility(key, output);
					result.Add(poss);
				}
			}
			return result;
        }
		
		/// <summary>
		/// Uses the loaded key dictionary to attack the provided input, returning only likely results
		/// based on the loaded language dictionary
		/// </summary>
		/// <param name="input">The data to be attacked</param>
		/// <returns></returns>
		public static List<AttackPossibility>PlayfairHack(string input)
        {
			var result = new List<AttackPossibility>();
			foreach (var ch in capital_letters)
			{
				var chHack = PlayfairHack(input, ch);
				result.AddRange(chHack);
			}
			return result;
		}

		/// <summary>
		/// An asynchronous task that runs the Playfair hack on the input
		/// </summary>
		/// <param name="progress">Implement the Progress interface to get results of the hacking progress.
		/// 0 is no progress and 1 is completed progress</param>
		/// <param name="input">The input that is to be attacked</param>
		/// <param name="omit">The omitted character while attacking</param>
		/// <returns></returns>
		public static async Task<List<AttackPossibility>> PlayfairHackAsync(IProgress<float> progress, string input, char omit)
        {
			if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");

			input = input.Prepare();
			var result = new List<AttackPossibility>();
			await Task.Run(() => {
				
				int curr = 0, max = key_dict.Count;
				foreach (var key in key_dict)
				{
					curr++;
					var output = PlayfairDecipher(input, key, omit);
					if (CheckDictionary(output, 3))
					{
						var poss = new AttackPossibility(key, output);
						result.Add(poss);
					}
					float prog = curr / (float)max;
					progress?.Report(prog);
				}
			});
			
			return result;
		}

		/// <summary>
		/// An asynchronous task that runs the Playfair hack on the input
		/// </summary>
		/// <param name="progress">Implement the Progress interface to get results of the hacking progress.
		/// 0 is no progress and 1 is completed progress</param>
		/// <param name="input">The input that is to be attacked</param>
		/// <param name="omit">The omitted character while attacking</param>
		/// <returns></returns>
		public static async Task PlayfairHackAsync(IProgress<float> progress, IProgress<AttackPossibility> discovered, string input, char omit)
        {
			if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");

			input = input.Prepare();
			await Task.Run(() => {

				int curr = 0, max = key_dict.Count;
				foreach (var key in key_dict)
				{
					curr++;
					var output = PlayfairDecipher(input, key, omit);
					if (CheckDictionary(output, 3))
					{
						var poss = new AttackPossibility(key, output);
						discovered?.Report(poss);
					}
					float prog = curr / (float)max;
					progress?.Report(prog);
				}
			});
		}

		/// <summary>
		/// Loads a dictionary to be used for attack.
		/// </summary>
		/// <param name="stream">The stream to load the dictionary from</param>
		/// <param name="type">The type of dictionary. See DictionaryType for more info</param>
		public static void LoadDictionary(Stream stream, DictionaryType type)
        {
			key_dict = new List<string>();
			lang_dict = new List<string>();

			using var strReader = new StreamReader(stream);

            while (true)
            {
				var str = strReader.ReadLine();
				if (str == null) break;

				str=str.Prepare();

				if(type==DictionaryType.Key || type == DictionaryType.Combined)
                {
					key_dict.Add(str);
                }

				if(type==DictionaryType.Language|| type == DictionaryType.Combined)
                {
					lang_dict.Add(str);
                }
            }

			dictLoaded = true;
        }

		/// <summary>
		/// Loads a dictionary to be used for attack.
		/// </summary>
		/// <param name="path">The path to the file containing the words.
		/// This should be a text file with each line containing a single word</param>
		/// <param name="type">The type of dictionary. See DictionaryType for more info</param>
		public static void LoadDictionary(string path, DictionaryType type)
        {
			using var msLoad = new MemoryStream(File.ReadAllBytes(path));
			LoadDictionary(msLoad, type);
        }

		private static bool CheckDictionary(string input, int depth)
        {
			//ADDED
			if (input.Length == 0) return true;
			//

			foreach(var word in lang_dict)
            {
				if (word.Length < 3) continue;
				if (input.Contains(word))
				{
					//if (depth == 0) return true;
     //               else
     //               {
					//	var nextInput = input.Replace(word, "");
					//	if (nextInput.Length < 3) return true;
					//	else return CheckDictionary(nextInput, depth - 1);

					//}


					var nextInput = input.Replace(word, "");
					if (nextInput.Length < 3) return true;
					else return CheckDictionary(nextInput, depth - 1);
				}
				continue;
            }
			return false;
        }
		
        #endregion

        #region Auxilliary
        private const string capital_letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string small_letters = "abcdefghijklmnopqrstuvwxyz";

		public static int GetLetterValue(char letter)
		{
			int val = 0;
			try
			{
				while (letter != capital_letters[val])
				{
					val++;
				}
			}
			catch
			{
				val = 0;
			}
			return val;

		}

		#endregion
	}

    #region Misc

    /// <summary>
    /// Usually returned as a list after a dictionary attack to show various
    /// possibilities of the attack
    /// </summary>
    public class AttackPossibility
	{
		/// <summary>
		/// The key that was used in creating the possiblity
		/// </summary>
		public string Key { get;}

		/// <summary>
		/// The output as a result of the possibility
		/// </summary>
		public string Output { get; }

		public AttackPossibility(string key, string output)
		{
			Key = key;
			Output = output;
		}

		/// <summary>
		/// Returns a neatly formatted string of the possibility containing
		/// the key and possible output
		/// </summary>
		public override string ToString()
        {
			return $"Key: {Key} Output: {Output}";

		}
	}

	/// <summary>
	/// Dictionary type determines whether the dictionary will be used as a key reference
	/// for the attack, or as a language reference. The language reference allows you to check
	/// whether any sequence of characters in the attacked data match any meaningful words
	/// of a specified language. When combined, it will be used as both key and language refs.
	/// </summary>
	public enum DictionaryType { Language, Key, Combined};
	#endregion
}
