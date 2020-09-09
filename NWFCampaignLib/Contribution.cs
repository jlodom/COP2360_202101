using System;

namespace NWFCampaignLib {
	public class Contribution {
		String stringCandidateCommittee = String.Empty;
		DateTime dtContributionDate = new DateTime();
		Decimal decAmount = 0.00m;
		String stringFullName = String.Empty;
		enum ContributionType { CHECK = 1, CASH = 2, INKIND = 3, REFUND = 4, UNKNOWN = 5 };
		ContributionType enumCotributionType = ContributionType.UNKNOWN;
		String stringFullStreetAddress = String.Empty;
		String stringFullCityStateZip = String.Empty;
		String stringOccupation = String.Empty;
		String stringInKindDescription = String.Empty;


		public Contribution(String decTempAmount, String stringTempFullName, String stringTempContributionType, String stringTempContributionDate, String stringTempFullCityStateZip = "", String stringTempFullStreetAddress = "", String stringTempCandidateCommittee = "", String stringTempOccupation = "", String stringTempInKindDescription = "") {

			this.decAmount = Convert.ToDecimal(decTempAmount);
			this.enumCotributionType = this.FloridaContributionTypeToEnum(stringTempContributionType);
			DateTime dtTempContributionDate;
			/* Out Variables Are Cool. Like Fezzes. */
			if(DateTime.TryParse(stringTempContributionDate, out dtTempContributionDate)) {
				this.dtContributionDate = dtTempContributionDate;
			}
			this.stringFullName = stringTempFullName;
			this.stringCandidateCommittee = stringTempCandidateCommittee;
			this.stringFullCityStateZip = stringTempFullCityStateZip;
			this.stringFullStreetAddress = stringTempFullStreetAddress;
			this.stringOccupation = stringTempOccupation;
			this.stringInKindDescription = stringTempInKindDescription;
		}

		ContributionType FloridaContributionTypeToEnum(String stringType) {

			switch(stringType) {
				case "CHK":
					return ContributionType.CHECK;
					break;
				case "CAS":
					return ContributionType.CASH;
					break;
				case "INK":
					return ContributionType.INKIND;
					break;
				case "REF":
					return ContributionType.REFUND;
					break;
				default:
					return ContributionType.UNKNOWN;
			}
			return ContributionType.UNKNOWN;
		}

	}

}
