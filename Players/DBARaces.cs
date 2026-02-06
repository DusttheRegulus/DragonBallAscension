namespace DragonBallAscension.Players
{
	public enum DBARace
	{
		Human = 0,
		Saiyan = 1,
		Namekian = 2,
		Majin = 3,
		FriezaClan = 4,
		Android = 5
	}

	public static class DBARaces
	{
		public static string GetRaceName(DBARace r) => r switch
		{
			DBARace.Human => "Human",
			DBARace.Saiyan => "Saiyan",
			DBARace.Namekian => "Namekian",
			DBARace.Majin => "Majin",
			DBARace.FriezaClan => "Frieza Clan",
			DBARace.Android => "Android",
			_ => r.ToString()
		};

		// Placeholder effects text for debugging/printing
		public static string GetRaceEffectsText(DBARace r) => r switch
		{
			DBARace.Human => "+5% KI regen while charging (placeholder)",
			DBARace.Saiyan => "+10% ATK scaling (placeholder)",
			DBARace.Namekian => "+10% DEF scaling (placeholder)",
			DBARace.Majin => "+15% max KI (placeholder)",
			DBARace.FriezaClan => "+12% ATK, -5% DEF (placeholder)",
			DBARace.Android => "No KI charging needed (placeholder)",
			_ => "No race effects"
		};
	}
}
