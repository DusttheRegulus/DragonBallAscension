using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

	public readonly struct RaceDef
	{
		public readonly string Name;
		public readonly int IconBuffId;

		public readonly float AtkMult;
		public readonly float DefMult;
		public readonly float MaxKiMult;

		public readonly float ChargeRateMult;
		public readonly float MoveSpeedBonus;

		public readonly float PassiveKiRegenPerSecond;

		public RaceDef(
			string name,
			int iconBuffId,
			float atkMult,
			float defMult,
			float maxKiMult,
			float chargeRateMult,
			float moveSpeedBonus,
			float passiveKiRegenPerSecond)
		{
			Name = name;
			IconBuffId = iconBuffId;

			AtkMult = atkMult;
			DefMult = defMult;
			MaxKiMult = maxKiMult;

			ChargeRateMult = chargeRateMult;
			MoveSpeedBonus = moveSpeedBonus;

			PassiveKiRegenPerSecond = passiveKiRegenPerSecond;
		}
	}

	public static class DBARaces
	{
		public static string GetRaceName(DBARace r) => GetDef(r).Name;

		public static RaceDef GetDef(DBARace r) => r switch
		{
			DBARace.Human => new RaceDef(
				name: "Human",
				iconBuffId: BuffID.WellFed,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.05f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),

			DBARace.Saiyan => new RaceDef(
				name: "Saiyan",
				iconBuffId: BuffID.Rage,
				atkMult: 1.10f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),

			DBARace.Namekian => new RaceDef(
				name: "Namekian",
				iconBuffId: BuffID.Ironskin,
				atkMult: 1.00f, defMult: 1.10f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),

			DBARace.Majin => new RaceDef(
				name: "Majin",
				iconBuffId: BuffID.Lifeforce,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.15f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),

			DBARace.FriezaClan => new RaceDef(
				name: "Frieza Clan",
				iconBuffId: BuffID.Wrath,
				atkMult: 1.12f, defMult: 0.95f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),

			DBARace.Android => new RaceDef(
				name: "Android",
				iconBuffId: BuffID.ManaRegeneration,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.05f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 6f),

			_ => new RaceDef(
				name: r.ToString(),
				iconBuffId: BuffID.WellFed,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f,
				passiveKiRegenPerSecond: 0f),
		};

		public static string GetRaceEffectsText(DBARace r)
		{
			var d = GetDef(r);

			string atk = d.AtkMult == 1f ? "" : $"{(d.AtkMult - 1f) * 100f:+0;-0;0}% ATK, ";
			string def = d.DefMult == 1f ? "" : $"{(d.DefMult - 1f) * 100f:+0;-0;0}% DEF, ";
			string ki = d.MaxKiMult == 1f ? "" : $"{(d.MaxKiMult - 1f) * 100f:+0;-0;0}% Max Ki, ";
			string chg = d.ChargeRateMult == 1f ? "" : $"{(d.ChargeRateMult - 1f) * 100f:+0;-0;0}% Charge Rate, ";
			string ms = d.MoveSpeedBonus == 0f ? "" : $"{d.MoveSpeedBonus * 100f:+0.0;-0.0;0.0}% Move Speed, ";
			string regen = d.PassiveKiRegenPerSecond <= 0f ? "" : $"{d.PassiveKiRegenPerSecond:0.0} Ki/s passive regen, ";

			string s = atk + def + ki + chg + ms + regen;
			if (s.EndsWith(", "))
				s = s.Substring(0, s.Length - 2);

			return s.Length == 0 ? "No race effects" : s;
		}

		public static void ApplyBuff(Player player, DBARace race, int refresh = 2)
		{
			switch (race)
			{
				case DBARace.Human:      player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.HumanRaceBuff>(), refresh); break;
				case DBARace.Saiyan:     player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.SaiyanRaceBuff>(), refresh); break;
				case DBARace.Namekian:   player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.NamekianRaceBuff>(), refresh); break;
				case DBARace.Majin:      player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.MajinRaceBuff>(), refresh); break;
				case DBARace.FriezaClan: player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.FriezaClanRaceBuff>(), refresh); break;
				case DBARace.Android:    player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.AndroidRaceBuff>(), refresh); break;
			}
		}
	}
}

namespace DragonBallAscension.Content.Buffs
{
	// Key trick: alias Players namespace using global::
	using DBAPlayers = global::DragonBallAscension.Players;

	public abstract class BaseRaceBuff : ModBuff
	{
		protected abstract DBAPlayers.DBARace Race { get; }

		public override string Texture =>
			"Terraria/Images/Buff_" + DBAPlayers.DBARaces.GetDef(Race).IconBuffId;

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			var d = DBAPlayers.DBARaces.GetDef(Race);
			buffName = d.Name;
			tip = DBAPlayers.DBARaces.GetRaceEffectsText(Race);
		}
	}

	public class HumanRaceBuff      : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.Human; }
	public class SaiyanRaceBuff     : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.Saiyan; }
	public class NamekianRaceBuff   : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.Namekian; }
	public class MajinRaceBuff      : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.Majin; }
	public class FriezaClanRaceBuff : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.FriezaClan; }
	public class AndroidRaceBuff    : BaseRaceBuff { protected override DBAPlayers.DBARace Race => DBAPlayers.DBARace.Android; }
}
