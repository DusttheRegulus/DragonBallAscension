using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DragonBallAscension.Players;

namespace DragonBallAscension.Content.Items.Weapons
{
	public class FocusedKiBeam_Test : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.LaserRifle}";

		// Drain while charging (per second)
		private const float ChargeKiDrainPerSecond = 35f;

		// Rapid fire baseline
		private const int FireUseTime = 4;

		// Per-shot Ki cost by stage (0..3). Stage 0 is NOT free so you can't fire with 0 Ki.
		private static readonly int[] FireKiCostPerShot = { 1, 2, 3, 4 };

		// Damage multiplier by stage (0..3)
		private static readonly float[] DamageMultByStage = { 0.80f, 1.00f, 1.25f, 1.55f };

		// How fast charging fills the meter (progress from 0..1 to stage up)
		private const float ChargeProgressPerTick = 0.02f; // ~50 ticks per stage

		// Charge decay when idle (handled in KiPlayer too, but we’ll keep behavior sane even if that changes)
		private const float IdleDecayPerTick = 0.00f; // leave 0 since KiPlayer already decays it

		// Local accumulator so we can drain fractional Ki smoothly while charging
		private float _chargeKiDrainAcc = 0f;

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;

			Item.damage = 14;
			Item.DamageType = DamageClass.Magic; // placeholder
			Item.knockBack = 1f;
			Item.crit = 0;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = FireUseTime;
			Item.useAnimation = FireUseTime;
			Item.autoReuse = true;
			Item.noMelee = true;

			Item.shoot = ProjectileID.PurpleLaser; // placeholder beam-ish projectile
			Item.shootSpeed = 16f;

			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(0, 1, 0, 0);

			Item.UseSound = SoundID.Item12;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			var kp = player.GetModPlayer<KiPlayer>();

			// Right click is "charge mode": don't actually shoot on right click.
			if (player.altFunctionUse == 2)
			{
				Item.UseSound = null;
				Item.shoot = ProjectileID.None;
				Item.autoReuse = false;
				Item.useTime = 2;
				Item.useAnimation = 2;

				// Allow starting charge if you have any Ki (or infinite)
				return kp.InfiniteKi || kp.Ki > 0;
			}

			// Left click is firing mode.
			Item.UseSound = SoundID.Item12;
			Item.shoot = ProjectileID.PurpleLaser;
			Item.autoReuse = true;
			Item.useTime = FireUseTime;
			Item.useAnimation = FireUseTime;

			int stage = kp.BeamChargeStage;
			if (stage < 0) stage = 0;
			if (stage > 3) stage = 3;

			int cost = FireKiCostPerShot[stage];
			return kp.HasKi(cost);
		}

		public override void HoldItem(Player player)
		{
			var kp = player.GetModPlayer<KiPlayer>();

			// Detect actual right mouse held (this is the important fix)
			bool rightHeld = Main.mouseRight && !Main.mouseRightRelease;

			// Detect left mouse held (firing is still handled by Shoot, but we can flag intent)
			bool leftHeld = Main.mouseLeft && !Main.mouseLeftRelease;

			// CHARGING: right mouse held
			if (rightHeld)
			{
				// If we have no Ki, we can't charge (unless infinite)
				if (!kp.InfiniteKi && kp.Ki <= 0)
					return;

				kp.BeamCharging = true;

				// Drain Ki while charging
				if (!kp.InfiniteKi)
				{
					_chargeKiDrainAcc += ChargeKiDrainPerSecond / 60f;
					if (_chargeKiDrainAcc >= 1f)
					{
						int drain = (int)_chargeKiDrainAcc;
						_chargeKiDrainAcc -= drain;

						// If can't pay, stop progressing charge
						if (!kp.TryConsumeKi(drain))
							return;
					}
				}
				else
				{
					_chargeKiDrainAcc = 0f;
				}

				// Build charge (up to stage 3)
				if (kp.BeamChargeStage < 3)
				{
					kp.BeamChargeProgress += ChargeProgressPerTick;
					if (kp.BeamChargeProgress >= 1f)
					{
						kp.BeamChargeProgress = 0f;
						kp.BeamChargeStage++;

						// Tiny feedback ping
						SoundEngine.PlaySound(SoundID.Item4, player.Center);
					}
				}

				return;
			}

			// Not charging this tick
			_chargeKiDrainAcc = 0f;

			// Optional: if you ever want decay handled here instead of KiPlayer, turn IdleDecayPerTick on.
			if (IdleDecayPerTick > 0f && !leftHeld)
			{
				if (kp.BeamChargeStage > 0 || kp.BeamChargeProgress > 0f)
				{
					kp.BeamChargeProgress -= IdleDecayPerTick;
					if (kp.BeamChargeProgress <= 0f)
					{
						if (kp.BeamChargeStage > 0)
						{
							kp.BeamChargeStage--;
							kp.BeamChargeProgress = 1f;
						}
						else kp.BeamChargeProgress = 0f;
					}
				}
			}
		}

		public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
			Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var kp = player.GetModPlayer<KiPlayer>();

			// Ensure this is left click (right click sets shoot none, but we hard-guard anyway)
			if (player.altFunctionUse == 2)
				return false;

			int stage = kp.BeamChargeStage;
			if (stage < 0) stage = 0;
			if (stage > 3) stage = 3;

			int cost = FireKiCostPerShot[stage];

			// If you can't pay, do not fire
			if (!kp.TryConsumeKi(cost))
				return false;

			kp.BeamFiring = true;

			float stageMult = DamageMultByStage[stage];
			int scaledDamage = (int)(damage * kp.EffectiveAtkMult * stageMult);

			// Fire the placeholder “beam stream”
			Projectile.NewProjectile(source, position, velocity, ProjectileID.PurpleLaser, scaledDamage, knockback, player.whoAmI);

			return false;
		}
	}
}
