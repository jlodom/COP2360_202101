/* Sample code from Week 2 by Johnnie Odom */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using NWFCampaignLib;

namespace Cop2360_Notes_1 {
	class Program {
		static void Main(string[] args) {

			/* Check how many arguments are sent to our program.
			 * There should only be one -- the path to the configuration file
			 * where we will get the rest of our settings. */
			if(args.Length != 1) {
				Console.WriteLine("This program requires one argument -- a path to a configuration file.");
				Environment.Exit(0);
			}

			String stringConfigurationFile = args[0];
			if(!File.Exists(stringConfigurationFile)) {
				Console.WriteLine("Configuration file does not exist.");
				Environment.Exit(0);
			}

			/* Set up configuration source using a factory pattern */
			IConfiguration configuration;
			/* This should be in try/catch but dotnet is being grumpy. */
			var configurationBuilder = new ConfigurationBuilder()
					.AddXmlFile(stringConfigurationFile);
			configuration = configurationBuilder.Build();


			String stringPathToFile = configuration["INPUTFILE"];
			Decimal decMaxContribution = Convert.ToDecimal(configuration["MAXCONTRIB"]);
			Decimal decSmallContributionThreshold = Convert.ToDecimal(configuration["MINCONTRIB"]);
			String stringPathToOutputFile = configuration["OUTPUTFILE"];


			/* Now set up logging */
			String stringLogPath = configuration["LOGFILEDIRECTORYPATH"] + Path.DirectorySeparatorChar + configuration["LOGFILEPREFIXNAME"] + "_{Date}.log";
			ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
					builder.AddConsole()
					.AddSerilog());
			Log.Logger = new LoggerConfiguration()
					.WriteTo.RollingFile(stringLogPath)
					.CreateLogger();
			Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<Program>();
			logger.LogInformation("Logging has begun.");

			/* Make sure the file exists before proceeding. */
			if(File.Exists(stringPathToFile)) {
				String stringEntireText = File.ReadAllText(stringPathToFile); /* Read the whole file into memory as a string (could go line by line. */
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
						logger.LogInformation("Blank Line");
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
				List <Contribution> listOfLocalContributions = new List<Contribution>();

				foreach(OrderedDictionary row in listOrdicRows) {
					/*logger.LogInformation("|" + row["contributor_name"].ToString().PadLeft(intMaxLengthColumn1) + "|" + row["amount"].ToString().PadLeft(intMaxLengthColumn2) + "|" + row["candidate_committee"].ToString().PadRight(intMaxLengthColumn3) + "|");*/
					intBreakpoint++;
					if(intBreakpoint%20 == 0) {
						Console.Write("");
					}
					String[] arrayStringOutput = new string[3];
					arrayStringOutput[0] = row["contributor_name"].ToString();
					arrayStringOutput[1] = row["amount"].ToString();
					arrayStringOutput[2] = row["candidate_committee"].ToString();
					Console.Write(BreakLineIntoTwo(arrayStringOutput, 35));

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
						logger.LogInformation("OH NOES");
					}
				}


				/* Example Added October 27  - Get All Zip Codes from File WITHOUT Duplicates */


				// Attempt 0 - Keep the duplicates
				List<String> listAllZipsV0 = new List<string>();
				foreach(Contribution contrib in listOfLocalContributions) {
					//This line don't worry about. We are breaking apart the zip from the rest of the line.
					String zip = contrib.GetCityStateZip().Substring(contrib.GetCityStateZip().LastIndexOf(" ") + 1);
					listAllZipsV0.Add(zip);
				}

				// Attempt 1 - See if the data already exists in the list.
				List<String> listAllZipsV1 = new List<string>();
				foreach(Contribution contrib in listOfLocalContributions) {
					//This line don't worry about. We are breaking apart the zip from the rest of the line.
					String zip = contrib.GetCityStateZip().Substring(contrib.GetCityStateZip().LastIndexOf(" ") + 1);
					if(!(listAllZipsV1.Contains(zip))) {
						listAllZipsV1.Add(zip);
					}
				}

				// Attempt 2 - Use LINQ to de-duplicate
				List<String> listAllZipsV2 = listAllZipsV0.Distinct<String>().ToList<String>();

				// Attempt 3 - Use a HashSet instead of a list (HashSets are always unique). 
				HashSet<String> hsAllZipsV3 = new HashSet<string>();
				foreach(Contribution contrib in listOfLocalContributions) {
					//This line don't worry about. We are breaking apart the zip from the rest of the line.
					String zip = contrib.GetCityStateZip().Substring(contrib.GetCityStateZip().LastIndexOf(" ") + 1);
					hsAllZipsV3.Add(zip);
				}


				logger.LogInformation("End of the October 27 code.");

				/* End October 27 */



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
				logger.LogInformation("Total Contributions Was: " + intTotalContributions);
				logger.LogInformation("Dollar Amount of Total Contributions Was: $" + decAmountOfAllContributions);
				logger.LogInformation("Total Max Contributions Was: " + intTotalMaxContributions);
				logger.LogInformation("Dollar Amount of Max Contributions Was: $" + decMaxContribution * intTotalMaxContributions);
				logger.LogInformation("Total Small Contributions Was: " + intTotalSmallContributions);
				logger.LogInformation("Dollar Amount of Small Contributions Was: $" + decTotalAmountOfSmallContributions);
				for(int i = 0; i < 15; i++) {
					logger.LogInformation(String.Empty);
				}

				logger.LogInformation("End");
				/* End data use example */

			}
			else {
				logger.LogInformation("Could not find file: " + stringPathToFile);
			}



			// This is a comment.
			/* This is also a comment. */

			/* Old Code 
			String stringFullName = args[0];

			String[] arrayStringTheSplit = stringFullName.Split(" ");

			logger.LogInformation(arrayStringTheSplit[1] + ", " + arrayStringTheSplit[0]);
			*/
		}

		static String BreakLineIntoTwo(String[] arrayStringInput, int intMaxCharacters, String stringSeparator = "") {

			String stringReturn = String.Empty;
			String[] arraySecondLine = new string[arrayStringInput.Count()];

			Boolean boolUseSecondLine = false;

			for(int i = 0; i< arrayStringInput.Count(); i++) {
				if(arrayStringInput[i].Count() > intMaxCharacters) {
					String stringFullLine = arrayStringInput[i];
					arrayStringInput[i] = stringFullLine.Substring(0, intMaxCharacters);
					arraySecondLine[i] = stringFullLine.Substring((intMaxCharacters + 1));
					boolUseSecondLine = true;
				}
				else {
					arraySecondLine[i] = String.Empty;
				}

			}

			String stringLine2 = String.Empty;
			for(int i = 0; i < arrayStringInput.Count(); i++) {
				stringReturn += arrayStringInput[i].PadRight(intMaxCharacters) + stringSeparator;
				stringLine2 += arraySecondLine[i].PadRight(intMaxCharacters) + stringSeparator;
			}
			stringReturn += Environment.NewLine;
			if(boolUseSecondLine) {
				stringReturn += stringLine2 + Environment.NewLine;
			}

			return stringReturn;
		}


	}
}
