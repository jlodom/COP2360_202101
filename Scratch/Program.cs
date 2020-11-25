using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Text.Json;

namespace Scratch {
	class Program {
		static void Main(string[] args) {

			/* 20201124 Example of debugging one program while running WebAPI separate. */
			WebClient wcScratch = new WebClient();
			String stringScratchURL = "http://localhost:5000/hint/scratchexample";
			String jsonScratch = wcScratch.DownloadString(stringScratchURL);
			String[] stringArrayScratch = JsonSerializer.Deserialize<String[]>(jsonScratch);
			// We could also do this:
			// List<String> stringArrayScratch = JsonSerializer.Deserialize<List<String>>(jsonScratch);


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