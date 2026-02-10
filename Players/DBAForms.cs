using Microsoft.Xna.Framework;

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
		// Mastery-scaled stats
		public readonly float StartAtkMult;
		public readonly float MaxAtkMult;

		public readonly float StartDefMult;
		public readonly float MaxDefMult;

		// Mastery-scaled drain
		public readonly float StartKiDrainPerSecond;
		public readonly float MinKiDrainPerSecond;

		// Mastery-scaled move speed (additive to Player.moveSpeed)
		public readonly float StartMoveSpeedBonus;
		public readonly float MaxMoveSpeedBonus;

		// Mastery progression
		public readonly float IdleMasteryGainPerSecond;
		public readonly float DummyHitMasteryGain;
		public readonly float RealHitMasteryGain;

		// Training cap (dummy/low-risk training cannot exceed this without combat)
		public readonly float TrainingCap; // 0..1

		public FormDef(
			float startAtkMult, float maxAtkMult,
			float startDefMult, float maxDefMult,
			float startKiDrainPerSecond, float minKiDrainPerSecond,
			float startMoveSpeedBonus, float maxMoveSpeedBonus,
			float idleMasteryGainPerSecond, float dummyHitMasteryGain, float realHitMasteryGain,
			float trainingCap)
		{
			StartAtkMult = startAtkMult;
			MaxAtkMult = maxAtkMult;

			StartDefMult = startDefMult;
			MaxDefMult = maxDefMult;

			StartKiDrainPerSecond = startKiDrainPerSecond;
			MinKiDrainPerSecond = minKiDrainPerSecond;

			StartMoveSpeedBonus = startMoveSpeedBonus;
			MaxMoveSpeedBonus = maxMoveSpeedBonus;

			IdleMasteryGainPerSecond = idleMasteryGainPerSecond;
			DummyHitMasteryGain = dummyHitMasteryGain;
			RealHitMasteryGain = realHitMasteryGain;

			TrainingCap = trainingCap;
		}
	}

	public static class DBAForms
	{
		public static string GetFormName(DBAForm f) => f switch
		{
			DBAForm.None => "None",
			DBAForm.PotentialUnleashed => "Potential Unleashed",
			DBAForm.SuperSaiyan1 => "Super Saiyan",
			_ => f.ToString()
		};

		public static FormDef GetDef(DBAForm f)
		{
			// Numbers are intentionally tame for v0.5 testing.
			// Start* values apply at mastery 0.0, Max* at mastery 1.0
			return f switch
			{
				DBAForm.PotentialUnleashed => new FormDef(
					startAtkMult: 1.10f, maxAtkMult: 1.25f,
					startDefMult: 1.06f, maxDefMult: 1.15f,
					startKiDrainPerSecond: 6f, minKiDrainPerSecond: 2.5f,
					startMoveSpeedBonus: 0.03f, maxMoveSpeedBonus: 0.08f,
					idleMasteryGainPerSecond: 0.00002f, dummyHitMasteryGain: 0.00035f, realHitMasteryGain: 0.00060f,
					trainingCap: 0.60f
				),

				DBAForm.SuperSaiyan1 => new FormDef(
					startAtkMult: 1.18f, maxAtkMult: 1.45f,
					startDefMult: 1.10f, maxDefMult: 1.25f,
					startKiDrainPerSecond: 9f, minKiDrainPerSecond: 3.5f,
					startMoveSpeedBonus: 0.04f, maxMoveSpeedBonus: 0.10f,
					idleMasteryGainPerSecond: 0.000015f, dummyHitMasteryGain: 0.00030f, realHitMasteryGain: 0.00055f,
					trainingCap: 0.55f
				),

				_ => new FormDef(
					startAtkMult: 1f, maxAtkMult: 1f,
					startDefMult: 1f, maxDefMult: 1f,
					startKiDrainPerSecond: 0f, minKiDrainPerSecond: 0f,
					startMoveSpeedBonus: 0f, maxMoveSpeedBonus: 0f,
					idleMasteryGainPerSecond: 0f, dummyHitMasteryGain: 0f, realHitMasteryGain: 0f,
					trainingCap: 0f
				),
			};
		}

		public static float GetAtkMult(DBAForm f, float mastery)
		{
			var def = GetDef(f);
			return MathHelper.Lerp(def.StartAtkMult, def.MaxAtkMult, MathHelper.Clamp(mastery, 0f, 1f));
		}

		public static float GetDefMult(DBAForm f, float mastery)
		{
			var def = GetDef(f);
			return MathHelper.Lerp(def.StartDefMult, def.MaxDefMult, MathHelper.Clamp(mastery, 0f, 1f));
		}

		public static float GetKiDrainPerSecond(DBAForm f, float mastery)
		{
			var def = GetDef(f);
			return MathHelper.Lerp(def.StartKiDrainPerSecond, def.MinKiDrainPerSecond, MathHelper.Clamp(mastery, 0f, 1f));
		}

		public static float GetMoveSpeedBonus(DBAForm f, float mastery)
		{
			var def = GetDef(f);
			return MathHelper.Lerp(def.StartMoveSpeedBonus, def.MaxMoveSpeedBonus, MathHelper.Clamp(mastery, 0f, 1f));
		}
	}
}
