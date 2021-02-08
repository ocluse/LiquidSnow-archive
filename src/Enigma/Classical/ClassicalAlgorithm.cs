using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public abstract class ClassicalAlgorithm : DictionaryAttackerBase
    {
		public Alphabet Alphabet { get; set; }

		public virtual string Key { get; set; }

		public virtual string Encrypt(string input)
        {
			return Run(input, true);
        }

        public virtual string Decrypt(string input)
        {
			return Run(input, false);
        }

		public abstract string Run(string input, bool forward);

		#region Hack

		/// <summary>
		/// Uses the loaded key dictionary to attack the provided input, returning only likely results
		/// based on the loaded language dictionary
		/// </summary>
		/// <param name="input">The data to be attacked</param>
		/// <returns></returns>
		public List<AttackPossibility> Hack(string input)
        {
			var storedKey = Key;
			var result = new List<AttackPossibility>();
			try
            {
				if (!dictLoaded) throw new InvalidOperationException("Dictionary not loaded");

				input = input.Prepare();

				
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
	}

	#endregion
}