using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RegexHelper
{
	public static class Controller
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		public static string Extract(List<string> lines, bool generic = false)
		{
			var o = new RegexObject(lines, generic);
			var str = o.Extract();
			str = RegexObject.Compress(str);
			return str;
		}
	}
}
