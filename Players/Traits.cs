using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

	public readonly struct TraitDef
	{
		public readonly string Name;
		public readonly int IconBuffId;

		public readonly float AtkMult;
		public readonly float DefMult;
		public readonly float MaxKiMult;

		public readonly float ChargeRateMult;
		public readonly float MoveSpeedBonus;

		public TraitDef(
			string name,
			int iconBuffId,
			float atkMult,
			float defMult,
			float maxKiMult,
			float chargeRateMult,
			float moveSpeedBonus)
		{
			Name = name;
			IconBuffId = iconBuffId;

			AtkMult = atkMult;
			DefMult = defMult;
			MaxKiMult = maxKiMult;

			ChargeRateMult = chargeRateMult;
			MoveSpeedBonus = moveSpeedBonus;
		}
	}

	public static class Traits
	{
		public static string GetTraitName(DBATrait t) => GetDef(t).Name;

		public static TraitDef GetDef(DBATrait t) => t switch
		{
			DBATrait.Legendary => new TraitDef(
				name: "Legendary",
				iconBuffId: BuffID.Wrath,
				atkMult: 1.10f, defMult: 1.10f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),

			DBATrait.Prodigy => new TraitDef(
				name: "Prodigy",
				iconBuffId: BuffID.MagicPower,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.15f, moveSpeedBonus: 0.00f),

			DBATrait.WarriorsSpirit => new TraitDef(
				name: "Warrior's Spirit",
				iconBuffId: BuffID.Endurance,
				atkMult: 1.00f, defMult: 1.05f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.05f),

			DBATrait.BattleGenius => new TraitDef(
				name: "Battle Genius",
				iconBuffId: BuffID.Rage,
				atkMult: 1.08f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),

			DBATrait.IronWill => new TraitDef(
				name: "Iron Will",
				iconBuffId: BuffID.Ironskin,
				atkMult: 1.00f, defMult: 1.12f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),

			DBATrait.Draconic => new TraitDef(
				name: "Draconic",
				iconBuffId: BuffID.Inferno,
				atkMult: 1.05f, defMult: 1.00f, maxKiMult: 1.05f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),

			DBATrait.Corrupted => new TraitDef(
				name: "Corrupted",
				iconBuffId: BuffID.ShadowDodge,
				atkMult: 1.20f, defMult: 0.90f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),

			_ => new TraitDef(
				name: t.ToString(),
				iconBuffId: BuffID.WellFed,
				atkMult: 1.00f, defMult: 1.00f, maxKiMult: 1.00f,
				chargeRateMult: 1.00f, moveSpeedBonus: 0.00f),
		};

		public static string GetTraitEffectsText(DBATrait t)
		{
			var d = GetDef(t);

			string atk = d.AtkMult == 1f ? "" : $"{(d.AtkMult - 1f) * 100f:+0;-0;0}% ATK, ";
			string def = d.DefMult == 1f ? "" : $"{(d.DefMult - 1f) * 100f:+0;-0;0}% DEF, ";
			string ki = d.MaxKiMult == 1f ? "" : $"{(d.MaxKiMult - 1f) * 100f:+0;-0;0}% Max Ki, ";
			string chg = d.ChargeRateMult == 1f ? "" : $"{(d.ChargeRateMult - 1f) * 100f:+0;-0;0}% Charge Rate, ";
			string ms = d.MoveSpeedBonus == 0f ? "" : $"{d.MoveSpeedBonus * 100f:+0.0;-0.0;0.0}% Move Speed, ";

			string s = atk + def + ki + chg + ms;
			if (s.EndsWith(", "))
				s = s.Substring(0, s.Length - 2);

			return s.Length == 0 ? "No trait effects" : s;
		}

		// Optional helper to apply the visible "trait buff icon"
		public static void ApplyBuff(Player player, DBATrait trait, int refresh = 2)
		{
			switch (trait)
			{
				case DBATrait.Legendary:      player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.LegendaryTraitBuff>(), refresh); break;
				case DBATrait.Prodigy:        player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.ProdigyTraitBuff>(), refresh); break;
				case DBATrait.WarriorsSpirit: player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.WarriorsSpiritTraitBuff>(), refresh); break;
				case DBATrait.BattleGenius:   player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.BattleGeniusTraitBuff>(), refresh); break;
				case DBATrait.IronWill:       player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.IronWillTraitBuff>(), refresh); break;
				case DBATrait.Draconic:       player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.DraconicTraitBuff>(), refresh); break;
				case DBATrait.Corrupted:      player.AddBuff(ModContent.BuffType<global::DragonBallAscension.Content.Buffs.CorruptedTraitBuff>(), refresh); break;
			}
		}
	}
}

namespace DragonBallAscension.Content.Buffs
{
	// Alias Players namespace safely (prevents the "Players doesn't exist" cascade)
	using DBAPlayers = global::DragonBallAscension.Players;

	public abstract class BaseTraitBuff : ModBuff
	{
		protected abstract DBAPlayers.DBATrait Trait { get; }

		public override string Texture =>
			"Terraria/Images/Buff_" + DBAPlayers.Traits.GetDef(Trait).IconBuffId;

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			var d = DBAPlayers.Traits.GetDef(Trait);
			buffName = d.Name;
			tip = DBAPlayers.Traits.GetTraitEffectsText(Trait);
		}
	}

	public class LegendaryTraitBuff      : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.Legendary; }
	public class ProdigyTraitBuff        : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.Prodigy; }
	public class WarriorsSpiritTraitBuff : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.WarriorsSpirit; }
	public class BattleGeniusTraitBuff   : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.BattleGenius; }
	public class IronWillTraitBuff       : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.IronWill; }
	public class DraconicTraitBuff       : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.Draconic; }
	public class CorruptedTraitBuff      : BaseTraitBuff { protected override DBAPlayers.DBATrait Trait => DBAPlayers.DBATrait.Corrupted; }
}
