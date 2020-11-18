using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using NWFCampaignLib;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NWFCampaignAPI {
	[Route("api/[controller]")]
	public class CampaignController : Controller {

		[HttpGet("/hint/{printme}")]
		public String BigOlHint(String printme) {
			String stringArrayCell = printme;
			String[] stringArray = new String[100];
			for(int i = 0; i < 100; i++) {
				stringArray[i] = stringArrayCell;
			}
			String stringJSONArray = JsonConvert.SerializeObject(stringArray);
			return stringJSONArray;

		}

		/* Return Some of the info from our CLI program. */
		[HttpGet("/campaign/basicinfo")]
		public String GetBasicInfo() {
			String stringOurReturn = String.Empty;

			// COPIED FROM PROGRAM
			/* Fixed paths are bad, and this should be read from arguments or a config file. 
			 * But for tonight, this will do. */
			//String stringPathToFile = "C:\\YOURPATHGOESHERE";
			String stringPathToFile = "/Gibbon/Odom_Life/Adjuncting/2019_Spring/cop1510/project_files/project4/gaetz.txt";
			Console.Write("\n\n\nPROGRAM STARTS HERE\n");
			Decimal decMaxContribution = 500.00m;
			Decimal decSmallContributionThreshold = 100m;
			/* Make sure the file exists before proceeding. */
			if(System.IO.File.Exists(stringPathToFile)) {
				String stringEntireText = System.IO.File.ReadAllText(stringPathToFile); /* Read the whole file into memory as a string (could go line by line. */
				String stringNewLine = "\r\n"; /* Force using Windows newline instead of Environment.NewLine */
				Char charDelimiter = '\t'; /* This file uses tabs, but we could change this variable if other files use other delimiters */
				String[] arrayStringLines = stringEntireText.Split(stringNewLine); /* Convert the big string into an array of rows */
				/* Create some more variables we will need. */
				int lineNumber = 0;
				List<String> listStringHeader = new List<String>();
				List<OrderedDictionary> listOrdicRows = new List<OrderedDictionary>();
				/* Go through every line/row and process appropriately. */
				foreach(String stringLine in arrayStringLines) {
					lineNumber++;
					/* If a line is blank, do not do anything with it. */
					if(String.IsNullOrWhiteSpace(stringLine)) {
						Console.WriteLine("Blank Line");
					}
					else {
						/* Create a list to holding header information if this is the first line (the header). */
						if(lineNumber == 1) {
							String[] arrayStringTempHeader = stringLine.Split(charDelimiter);
							foreach(String stringHeaderColumn in arrayStringTempHeader) {
								listStringHeader.Add(stringHeaderColumn.Replace(' ', '_').Replace('/', '_').Trim().ToLower());
								/* We can dice the header string a lot in just one line (above). */
							}
						}
						/* If this is not the first line, treat it like a data row. */
						else {
							String[] arrayStringTempROw = stringLine.Split(charDelimiter);
							OrderedDictionary ordicRow = new OrderedDictionary();
							for(int i = 0; i < listStringHeader.Count; i++) {
								ordicRow.Add(listStringHeader[i], arrayStringTempROw[i]);
								/* (Above) Add each column in the row to an ordered dictionary keyed to its header name. 
								 We can look up this column data in the future either by its order by its header name. */
							}
							listOrdicRows.Add(ordicRow);
						}
					}
				}
				/* This is just one example of using the data 
				 * Go through all the rows and get a count of the City, State, Zip entries. */

				int intMaxLengthColumn1 = 0;
				int intMaxLengthColumn2 = 0;
				int intMaxLengthColumn3 = 0;

				foreach(OrderedDictionary row in listOrdicRows) {

					// candidate_committee	date	amount	typ	contributor_name	Address	City State 
					if(row["contributor_name"].ToString().Count() > intMaxLengthColumn1) {
						intMaxLengthColumn1 = row["contributor_name"].ToString().Count();
					}
					if(row["amount"].ToString().Count() > intMaxLengthColumn2) {
						intMaxLengthColumn2 = row["amount"].ToString().Count();
					}
					if(row["candidate_committee"].ToString().Count() > intMaxLengthColumn3) {
						intMaxLengthColumn3 = row["candidate_committee"].ToString().Count();
					}


				}

				int intBreakpoint = 0;

				/* Make a list of our new Contibution Objects */
				List<Contribution> listOfLocalContributions = new List<Contribution>();

				foreach(OrderedDictionary row in listOrdicRows) {
					/*Console.WriteLine("|" + row["contributor_name"].ToString().PadLeft(intMaxLengthColumn1) + "|" + row["amount"].ToString().PadLeft(intMaxLengthColumn2) + "|" + row["candidate_committee"].ToString().PadRight(intMaxLengthColumn3) + "|");*/
					intBreakpoint++;
					if(intBreakpoint % 20 == 0) {
						Console.Write("");
					}
					String[] arrayStringOutput = new string[3];
					arrayStringOutput[0] = row["contributor_name"].ToString();
					arrayStringOutput[1] = row["amount"].ToString();
					arrayStringOutput[2] = row["candidate_committee"].ToString();
					//Console.Write(BreakLineIntoTwo(arrayStringOutput, 35));

					/* Piggy-Backing on our For Loop to Create Objects */
					try {
						listOfLocalContributions.Add(new Contribution(
							row["amount"].ToString(),
							row["contributor_name"].ToString(),
							row["typ"].ToString(),
							row["date"].ToString(),
							row["city_state_zip"].ToString(),
							row["address"].ToString(),
							row["candidate_committee"].ToString(),
							row["occupation"].ToString(),
							row["inkind_desc"].ToString()));

					}
					catch(Exception e) {
						Console.WriteLine("OH NOES");
					}
				}


				//Contribution example = listOfLocalContributions[345];


				/* Generate some simple stats */
				int intTotalContributions = 0;
				int intTotalMaxContributions = 0;
				int intTotalSmallContributions = 0;
				Decimal decTotalAmountOfSmallContributions = 0m;
				Decimal decAmountOfAllContributions = 0m;
				foreach(Contribution contribution in listOfLocalContributions) {
					if(contribution.GetAmount() == decMaxContribution) {
						intTotalMaxContributions++;
					}
					else if((contribution.GetAmount() < decSmallContributionThreshold) && (contribution.enumCotributionType != Contribution.ContributionType.REFUND)) {
						intTotalSmallContributions++;
						decTotalAmountOfSmallContributions += contribution.GetAmount();
					}
					intTotalContributions++;
					decAmountOfAllContributions += contribution.GetAmount();
				}
				/*
				Console.WriteLine("Total Contributions Was: " + intTotalContributions);
				Console.WriteLine("Dollar Amount of Total Contributions Was: $" + decAmountOfAllContributions);
				Console.WriteLine("Total Max Contributions Was: " + intTotalMaxContributions);
				Console.WriteLine("Dollar Amount of Max Contributions Was: $" + decMaxContribution * intTotalMaxContributions);
				Console.WriteLine("Total Small Contributions Was: " + intTotalSmallContributions);
				Console.WriteLine("Dollar Amount of Small Contributions Was: $" + decTotalAmountOfSmallContributions);
				for(int i = 0; i < 15; i++) {
					Console.WriteLine(String.Empty);
				}

				

				Console.WriteLine("End");*/

				stringOurReturn = "Total Contributions Was: " + intTotalContributions;

				/* End data use example */

			}

			//END COPY

			return stringOurReturn;
		}

		/* Default Boilerplate Below Here */

		// GET: api/values
		[HttpGet]
		public IEnumerable<string> Get() {
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id) {
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value) {
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value) {
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id) {
		}
	}
}
