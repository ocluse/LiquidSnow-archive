namespace Thismaker.Horus.Classical
{
	/// <summary>
	/// Defines a possible key from a hack, with the output as well
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

		/// <summary>
		/// Creates a new instance of the <see cref="AttackPossibility"/>.
		/// </summary>
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
	/// Used to determine the function a dictionary will be used for.
	/// </summary>
	public enum DictionaryType 
	{ 
		/// <summary>
		/// The dictionary will be used to check if any sequence of characters in the attacked data match any meaningful words.
		/// </summary>
		Language,
		/// <summary>
		/// The dictionary will be used to provide keys used to attack a particular cyphertext.
		/// </summary>
		Key, 
		/// <summary>
		/// The dictionary is used both as a source of language and to obtain attack keys.
		/// </summary>
		Combined
	};
}
