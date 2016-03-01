using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace RegexHelper
{
    public class GeneratedPattern
    {
        private string start = "";
        private string end = "";

        public void Add(string s, bool fromStart)
        {
            if (fromStart)
                start += s;
            else
                end = s + end;
        }

        public string Generate()
        {
            return start + end;
        }

        public int StartLength => start.Length;

        public int EndLength => end.Length;

        public void Insert(string s, int index, bool fromStart)
        {
            if (fromStart)
                start = start.Insert(index, s);
            else
                end = end.Insert(end.Length - index, s);
        }
    }

    public class RegexStack
    {
        public RegexStack()
        {
        }

        public RegexStack(int length, bool fromFront, string str)
        {
            Length = length;
            FromFront = fromFront;
            Str = str;
        }

        public int Length { get; set; }
        public bool FromFront { get; set; }
        public string Str { get; set; }
    }

    public class RegexObject
    {
        /// <summary>
        /// false= a or b = [ab], true = \w 
        /// </summary>
        public bool Generic;

        public List<RegexFragment> fragments = new List<RegexFragment>();
        public List<RegexStack> stack = new List<RegexStack>();

        public RegexObject(List<string> lines, bool generic)
        {
            fragments = lines.Select(s => new RegexFragment(s)).ToList();
            Generic = generic;
        }

        private void PushParenthesis(ref GeneratedPattern pattern, bool forward)
        {
            if (forward)
            {
                pattern.Add("(", true);
                stack.Add(new RegexStack(pattern.EndLength, false, ")?"));
            }
            else
            {
                pattern.Add(")?", false);
                stack.Add(new RegexStack(pattern.StartLength, true, "("));
            }
        }

        private bool PopStack(ref GeneratedPattern pattern)
        {
            if (stack.Count == 0)
                return false;

            var str = stack.Last();
            stack.RemoveAt(stack.Count - 1);

            pattern.Insert(str.Str, str.Length, str.FromFront);
            return true;
        }

        /// <summary>
        /// increment pointer
        /// </summary>
        /// <param name="selectedFragments"></param>
        private void IncrementPosition(List<RegexFragment> selectedFragments)
        {
            selectedFragments.ForEach(s => s.IncrementPosition());
        }

        /// <summary>
        /// true if counts dont match
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private int CountLengthInvalid(List<RegexFragment> selectedFragments)
        {
            return selectedFragments.Count(s => s.Invalid());
        }

        /// <summary>
        /// all fragments have a matching character on position
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private char? CharacterMatch(IEnumerable<RegexFragment> selectedFragments)
        {
            var same = selectedFragments.Select(s => s.CurrentPosChar()).Distinct().ToList();
            if (same.Count() != 1)
                return null;
            return same.First();
        }

        /// <summary>
        /// all fragments have a matching meta character on position
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private string MetaMatch(List<RegexFragment> selectedFragments)
        {
            var same = selectedFragments.Select(s => s.CurrentPosMeta()).Distinct().ToList();
            if (same.Count() != 1)
                return null;
            return same.First();
        }

        /// <summary>
        /// get all the characters
        /// increment pos
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private string GetAllChars(List<RegexFragment> selectedFragments)
        {
            var str = selectedFragments.Aggregate("", (a, b) => a + b.CurrentPosChar());
            return str;
        }

        /// <summary>
        /// either returns the meta, or all the characters if not generic
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private string GetMetaAppropriate(List<RegexFragment> selectedFragments, string meta)
        {
            var pattern = "";
            if (Generic)
                pattern += meta;
            else
                pattern += "[" + GetAllChars(selectedFragments) + "]";

            return pattern;
        }

        /// <summary>
        /// get either the first fragments char or meta depending on generic
        /// </summary>
        /// <param name="selectedFragments"></param>
        /// <returns></returns>
        private string GetCharAppropriate(List<RegexFragment> selectedFragments)
        {
            return Generic ? selectedFragments.First().CurrentPosMeta() : selectedFragments.First().CurrentPosChar().ToString();
        }

        private static string CompressRep(string pattern, params string[] reps)
        {
            foreach (var rep in reps)
            {
                Tuple<int, int, int> x1;
                while ((x1 = StringExtras.GetFirstRepetition(pattern, rep, 3)) != null)
                {
                    var repstr = rep + @"{" + x1.Item1 + "}";
                    pattern = StringExtras.ReplaceStringAtPosition(pattern, repstr, x1.Item2, x1.Item3);
                }
            }

            return pattern;
        }

        private static string CompressSingleBrackets(string pattern)
        {
            pattern = Regex.Replace(pattern, @"\((.)\)", "$1");
            return pattern;
        }

        public static string Compress(string pattern)
        {
            pattern = CompressRep(pattern, @"\w", @"\d", @"\ ");
            pattern = CompressSingleBrackets(pattern);
            return pattern;
        }

        private bool ExtractAtCurrentPosition(ref GeneratedPattern gp, List<RegexFragment> selectedFragments, bool returnOnNoMatch = false, bool forward = true)
        {
            char? matchchar;
            string matchmeta;

            //all the same, easy
            if ((matchchar = CharacterMatch(selectedFragments)) != null)
                gp.Add(GetCharAppropriate(selectedFragments), forward);
            //meta type same
            else if ((matchmeta = MetaMatch(selectedFragments)) != null)
                gp.Add(GetMetaAppropriate(selectedFragments, matchmeta), forward);
            //different types
            else
            {
                if (returnOnNoMatch)
                    return false;

                gp.Add("[" + GetAllChars(selectedFragments) + "]", forward);
            }
            return true;
        }

        private void SwitchDirection(bool forward)
        {
            fragments.ForEach(s => s.Forward = forward);
        }

        public string Extract()
        {
            var pattern = new GeneratedPattern();

            List<RegexFragment> selectedFragments = null;
            bool forward = true;
            while ((selectedFragments = fragments.Where(s => s.Invalid() == false).ToList()).Count > 0)
            {
                //keep switching until match, or continue forward
                if (ExtractAtCurrentPosition(ref pattern, selectedFragments, true, forward) == false)
                {
                    forward = !forward;
                    SwitchDirection(forward);
                    if (ExtractAtCurrentPosition(ref pattern, selectedFragments, true, forward) == false)
                    {
                        forward = true;
                        SwitchDirection(forward);
                        ExtractAtCurrentPosition(ref pattern, selectedFragments, false, forward);
                    }
                }

                //increment pos
                IncrementPosition(selectedFragments);

                //future length is different, parenthesis, but dont if at the end
                var invalid = CountLengthInvalid(selectedFragments);
                if (invalid > 0 && invalid != selectedFragments.Count)
                    PushParenthesis(ref pattern, forward);
            }

            //empty parens stack
            while (PopStack(ref pattern))
            {

            }

            var ret = pattern.Generate();
            return ret;
        }
    }
}