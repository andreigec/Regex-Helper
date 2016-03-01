using System.Linq;

namespace RegexHelper
{
	public class RegexFragment
	{
		private char[] chars;
		private string[] meta;
		private int startPosition = 0;
		private int endPosition = 0;

		public int length;

		public bool Forward = true;

		public RegexFragment(char[] chars, string[] meta)
		{
			this.chars = chars;
			this.meta = meta;
			length = chars.Length;
			endPosition = length - 1;
		}

		public RegexFragment(string s)
			: this(s.ToCharArray(), s.Select(MappedChar).ToArray())
		{
		}

		private static string MappedChar(char c)
		{
			if (c >= 48 && c <= 57)
				return @"\d";
			if (c >= 65 && c <= 90 || c >= 97 && c <= 122)
				return @"\w";

			return c.ToString();
		}

		public string CurrentPosMeta()
		{
			return Forward ? meta[startPosition] : meta[endPosition];
		}

		public int CharsCount()
		{
			return chars.Count();
		}

		public char CurrentPosChar()
		{
			return Forward ? chars[startPosition] : chars[endPosition];
		}

		public bool Invalid()
		{
			return startPosition > chars.Length || endPosition < 0 || endPosition < startPosition;
		}

		public void IncrementPosition()
		{
			if (Forward)
				startPosition++;
			else
				endPosition--;
		}

	}
}