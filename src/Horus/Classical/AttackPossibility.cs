namespace Thismaker.Horus.Classical
{
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
}
