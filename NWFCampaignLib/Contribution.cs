using System;

namespace NWFCampaignLib {
	public class Contribution {
		String stringCandidateCommittee = String.Empty;
		DateTime dtContributionDate = new DateTime();
		Decimal decAmount = 0.00m;
		String stringFullName = String.Empty;
		enum ContributionType { CHECK = 1, CASH = 2, INKIND = 3, REFUND = 4, UNKNOWN = 5  };
		String stringFullStreetAddress = String.Empty;
		String stringFullCityStateZip = String.Empty;
		String stringOccupation = String.Empty;
		String stringInKindDescription = String.Empty;


	}

}
