using System;
using System.Linq;
using System.Collections.Generic;

namespace Scratch {
	class Program {
		static void Main(string[] args) {

			/* Basic Linq Example Adapted from Book */

			List<String> linqList = new List<String>();
			linqList.Add("I");
			linqList.Add("don't");
			linqList.Add("know");
			linqList.Add("why");
			linqList.Add("you");
			linqList.Add("say");
			linqList.Add("goodbye");
			linqList.Add("I");
			linqList.Add("say");
			linqList.Add("hello");


			/* Long Version */
			List<String> queryResult =
				(from lyric in linqList
				where lyric.Length < 5
				orderby lyric descending
				select lyric).Distinct().Reverse().ToList<String>();

			/* Shorter Version */
			IEnumerable<String> queryShort = linqList.Where(s => s.Length < 5).Distinct().Reverse();

			Printout(queryShort);

			List<String> listQueryShort = queryShort.ToList<String>();
			
		}
		public static void Printout(IEnumerable<String> enurmerableStrings) {
			Console.Write(Environment.NewLine);
			foreach(String estring in enurmerableStrings) {
				Console.Write(estring + " ");
			}
			Console.Write(Environment.NewLine);
		}
	}
}