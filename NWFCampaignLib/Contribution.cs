using System;
using System.Collections.Generic;

namespace NWFCampaignLib {
	public class Contribution {
		String stringCandidateCommittee = String.Empty;
		DateTime dtContributionDate = new DateTime();
		Decimal decAmount = 0.00m;
		String stringFullName = String.Empty;
		public enum ContributionType { CHECK = 1, CASH = 2, INKIND = 3, REFUND = 4, UNKNOWN = 5 };
		public ContributionType enumCotributionType = ContributionType.UNKNOWN;
		String stringFullStreetAddress = String.Empty;
		String stringFullCityStateZip = String.Empty;
		String stringOccupation = String.Empty;
		String stringInKindDescription = String.Empty;
		readonly DateTime dtDefault = DateTime.Parse("01/01/2000");
		List<String> listSimpleErrors = new List<string>();


		/*  */
		public Contribution(String decTempAmount, String stringTempFullName, String stringTempContributionType, String stringTempContributionDate, String stringTempFullCityStateZip = "", String stringTempFullStreetAddress = "", String stringTempCandidateCommittee = "", String stringTempOccupation = "", String stringTempInKindDescription = "") {

			this.decAmount = Convert.ToDecimal(decTempAmount);
			this.enumCotributionType = this.FloridaContributionTypeToEnum(stringTempContributionType);
			DateTime dtTempContributionDate;
			/* Out Variables Are Cool. Like Fezzes. */
			if(DateTime.TryParse(stringTempContributionDate, out dtTempContributionDate)) {
				this.dtContributionDate = dtTempContributionDate;
			}
			else {
				this.dtContributionDate = this.dtDefault;
				listSimpleErrors.Add("We have a bad date: |" + stringTempContributionDate +  "|");
			}
			this.stringFullName = stringTempFullName;
			this.stringCandidateCommittee = stringTempCandidateCommittee;
			this.stringFullCityStateZip = stringTempFullCityStateZip;
			this.stringFullStreetAddress = stringTempFullStreetAddress;
			this.stringOccupation = stringTempOccupation;
			this.stringInKindDescription = stringTempInKindDescription;
		}

		public Decimal GetAmount() {
			return this.decAmount;
		}

		public String GetCityStateZip() {
			return this.stringFullCityStateZip;
		}

		ContributionType FloridaContributionTypeToEnum(String stringType) {

			ContributionType contribReturn = ContributionType.UNKNOWN;

			switch(stringType) {
				case "CHK":
					contribReturn = ContributionType.CHECK;
					break;
				case "CAS":
					contribReturn = ContributionType.CASH;
					break;
				case "INK":
					contribReturn = ContributionType.INKIND;
					break;
				case "REF":
					contribReturn = ContributionType.REFUND;
					break;
			}
			return contribReturn;
		}


	}

}
