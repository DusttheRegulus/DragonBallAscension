namespace DragonBallAscension.Players
{
	public enum DBATrait
	{
		Legendary,
		Prodigy,
		WarriorsSpirit,
		BattleGenius,
		IronWill,
		Draconic,
		Corrupted
	}

	public static class Traits
	{
		public static string GetTraitName(DBATrait t) => t switch
		{
			DBATrait.Legendary => "Legendary",
			DBATrait.Prodigy => "Prodigy",
			DBATrait.WarriorsSpirit => "Warrior's Spirit",
			DBATrait.BattleGenius => "Battle Genius",
			DBATrait.IronWill => "Iron Will",
			DBATrait.Draconic => "Draconic",
			DBATrait.Corrupted => "Corrupted",
			_ => t.ToString()
		};

		// Placeholder effects text for debugging/printing
		public static string GetTraitEffectsText(DBATrait t) => t switch
		{
			DBATrait.Legendary => "+10% ATK, +10% DEF (placeholder)",
			DBATrait.Prodigy => "+15% KI gain while charging (placeholder)",
			DBATrait.WarriorsSpirit => "+5% DEF, +5% move speed (placeholder)",
			DBATrait.BattleGenius => "+8% ATK (placeholder)",
			DBATrait.IronWill => "+12% DEF (placeholder)",
			DBATrait.Draconic => "+5% ATK, +5% KI max (placeholder)",
			DBATrait.Corrupted => "+20% ATK, -10% DEF (placeholder)",
			_ => "No trait effects"
		};
	}
}
