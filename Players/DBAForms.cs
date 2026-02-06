namespace DragonBallAscension.Players
{
	public enum DBAForm
	{
		None = 0,
		PotentialUnleashed = 1,
		SuperSaiyan1 = 2
	}

	public readonly struct FormDef
	{
		public readonly float AtkMult;
		public readonly float DefMult;
		public readonly float KiDrainPerSecond;
		public readonly float Mastery; // placeholder (0..1)

		public FormDef(float atkMult, float defMult, float kiDrainPerSecond, float mastery)
		{
			AtkMult = atkMult;
			DefMult = defMult;
			KiDrainPerSecond = kiDrainPerSecond;
			Mastery = mastery;
		}
	}

	public static class DBAForms
	{
		public static string GetFormName(DBAForm f) => f switch
		{
			DBAForm.None => "None",
			DBAForm.PotentialUnleashed => "Potential Unleashed",
			DBAForm.SuperSaiyan1 => "Super Saiyan 1",
			_ => f.ToString()
		};

		public static FormDef GetDef(DBAForm f)
		{
			// KiDrainPerSecond is intentionally noticeable for testing
			return f switch
			{
				DBAForm.PotentialUnleashed => new FormDef(atkMult: 1.25f, defMult: 1.15f, kiDrainPerSecond: 6f, mastery: 0.00f),
				DBAForm.SuperSaiyan1 => new FormDef(atkMult: 1.45f, defMult: 1.25f, kiDrainPerSecond: 9f, mastery: 0.00f),
				_ => new FormDef(atkMult: 1f, defMult: 1f, kiDrainPerSecond: 0f, mastery: 0f),
			};
		}
	}
}
