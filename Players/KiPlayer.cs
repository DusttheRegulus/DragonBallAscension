using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.Audio;
using DragonBallAscension.Content.Buffs;

namespace DragonBallAscension.Players
{
	public class KiPlayer : ModPlayer
	{
		public int MaxKi = 1000;
		public int Ki = 0;

		public bool InfiniteKi = false;

		public DBARace Race = DBARace.Human;
		public DBATrait Trait = DBATrait.Legendary;

		public DBAForm SelectedForm = DBAForm.PotentialUnleashed;
		public DBAForm ActiveForm = DBAForm.None;

		public float KiBarX = 520f;
		public float KiBarY = 80f;

		public bool FlightMode = false;

		// Flight / IT tuning (placeholder values for v0.5)
		private const float FlightKiDrainPerSecond = 40f; // drains while flying
		private const int InstantTransmissionKiCost = 60;

		// Charging state (set by input)
		public bool IsCharging = false;

		// Internal timers
		private float _formDrainAccumulator = 0f;
		private int _chargeDustTimer = 0;

		// Aura timer (throttle particles)
		private int _auraDustTimer = 0;

		// Flight drain accumulator
		private float _flightDrainAccumulator = 0f;

		public override void Initialize()
		{
			MaxKi = 1000;
			Ki = 0; // start at 0 so charging matters
		}

		public override void SaveData(TagCompound tag)
		{
			tag["MaxKi"] = MaxKi;
			tag["Ki"] = Ki;
			tag["InfiniteKi"] = InfiniteKi;

			tag["Race"] = (int)Race;
			tag["Trait"] = (int)Trait;

			tag["SelectedForm"] = (int)SelectedForm;
			tag["ActiveForm"] = (int)ActiveForm;

			tag["KiBarX"] = KiBarX;
			tag["KiBarY"] = KiBarY;

			tag["FlightMode"] = FlightMode;
		}

		public override void LoadData(TagCompound tag)
		{
			MaxKi = tag.GetInt("MaxKi");
			Ki = tag.GetInt("Ki");
			InfiniteKi = tag.GetBool("InfiniteKi");

			Race = (DBARace)tag.GetInt("Race");
			Trait = (DBATrait)tag.GetInt("Trait");

			SelectedForm = (DBAForm)tag.GetInt("SelectedForm");
			ActiveForm = (DBAForm)tag.GetInt("ActiveForm");

			KiBarX = tag.ContainsKey("KiBarX") ? tag.GetFloat("KiBarX") : 520f;
			KiBarY = tag.ContainsKey("KiBarY") ? tag.GetFloat("KiBarY") : 80f;

			FlightMode = tag.GetBool("FlightMode");

			ClampKi();
		}

		private void ClampKi()
		{
			if (MaxKi < 0) MaxKi = 0;
			if (Ki < 0) Ki = 0;
			if (Ki > MaxKi) Ki = MaxKi;
		}

		/// <summary>
		/// Handle flight movement here so it overrides vanilla movement (prevents "slow-fall" feel).
		/// </summary>
		public override void PreUpdateMovement()
		{
			if (Player.dead || !FlightMode)
				return;

			ApplyControlledFlight();
		}

		public override void PostUpdate()
		{
			if (Player.dead)
				return;

			if (InfiniteKi)
				Ki = MaxKi;

			// Charge-only KI gain (no passive regen)
			if (!InfiniteKi && IsCharging && Ki < MaxKi)
			{
				Ki += 2; // noticeable for testing
				if (Ki > MaxKi) Ki = MaxKi;

				// Simple aura-ish dust while charging
				_chargeDustTimer++;
				if (_chargeDustTimer >= 3)
				{
					_chargeDustTimer = 0;
					var pos = Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 10f));
					var d = Terraria.Dust.NewDustPerfect(pos, 58, new Vector2(0f, -0.8f), 120, Color.White, 1.1f);
					d.noGravity = true;
				}
			}

			// Flight: drain Ki while enabled
			if (FlightMode)
			{
				if (!InfiniteKi)
				{
					_flightDrainAccumulator += FlightKiDrainPerSecond / 60f;
					if (_flightDrainAccumulator >= 1f)
					{
						int drain = (int)_flightDrainAccumulator;
						_flightDrainAccumulator -= drain;

						Ki -= drain;
						if (Ki <= 0)
						{
							Ki = 0;
							FlightMode = false;
							_flightDrainAccumulator = 0f;
						}
					}
				}
				else
				{
					_flightDrainAccumulator = 0f;
				}
			}
			else
			{
				_flightDrainAccumulator = 0f;
			}

			// While transformed: apply buff + aura particles + drain
			if (ActiveForm != DBAForm.None)
			{
				ApplyFormBuffs();

				// Aura particles (throttled)
				_auraDustTimer++;
				if (_auraDustTimer >= 3)
				{
					_auraDustTimer = 0;
					SpawnFormAuraDust();
				}

				// Form drain while transformed
				if (!InfiniteKi)
				{
					var def = DBAForms.GetDef(ActiveForm);

					_formDrainAccumulator += def.KiDrainPerSecond / 60f; // per tick
					if (_formDrainAccumulator >= 1f)
					{
						int drain = (int)_formDrainAccumulator;
						_formDrainAccumulator -= drain;

						Ki -= drain;
						if (Ki <= 0)
						{
							Ki = 0;
							PowerDown();
						}
					}
				}
				else
				{
					_formDrainAccumulator = 0f;
				}
			}
			else
			{
				_formDrainAccumulator = 0f;
				_auraDustTimer = 0;
				ClearFormBuffs();
			}

			ClampKi();
		}

		/// <summary>
		/// True WASD flight.
		/// - No slow-fall: gravity is nulled.
		/// - X and Y are controlled independently (no diagonal normalization).
		/// - If you're not holding W/S, you hard-hover vertically (velocity.Y = 0).
		/// - If you're not holding A/D, you stop horizontally (velocity.X = 0).
		/// </summary>
		private void ApplyControlledFlight()
		{
			// Fully negate gravity / falling
			Player.gravity = 0f;
			Player.maxFallSpeed = 0f;
			Player.noFallDmg = true;
			Player.fallStart = (int)(Player.position.Y / 16f);

			// Stop vanilla wings/rocket logic from interfering
			Player.wingTime = Player.wingTimeMax;
			Player.rocketTime = Player.rocketTimeMax;

			// Tuning knobs
			float speed = 8f;     // flight top speed (per axis)
			float accel = 0.35f;  // how quickly you reach target velocity

			// Axis input (separate, so left/right never creates vertical drift)
			bool left = Player.controlLeft;
			bool right = Player.controlRight;
			bool up = Player.controlUp;
			bool down = Player.controlDown;

			// Horizontal
			if (left || right)
			{
				float targetX = (right ? 1f : 0f) + (left ? -1f : 0f);
				targetX *= speed;
				Player.velocity.X = MathHelper.Lerp(Player.velocity.X, targetX, accel);
			}
			else
			{
				// No A/D: stop dead horizontally
				Player.velocity.X = 0f;
			}

			// Vertical
			if (up || down)
			{
				float targetY = (down ? 1f : 0f) + (up ? -1f : 0f);
				targetY *= speed;
				Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, targetY, accel);
			}
			else
			{
				// No W/S: hard hover vertically (prevents the "sinking while strafing" issue)
				Player.velocity.Y = 0f;
			}
		}

		public void ToggleFlight()
		{
			if (Player.dead)
				return;

			if (FlightMode)
			{
				FlightMode = false;
				_flightDrainAccumulator = 0f;
				return;
			}

			// Require at least some Ki to engage flight
			if (!InfiniteKi && Ki <= 0)
				return;

			FlightMode = true;
			_flightDrainAccumulator = 0f;

			SoundEngine.PlaySound(SoundID.Item25, Player.Center);
		}

		public bool TryInstantTransmission()
		{
			if (Player.dead)
				return false;

			if (!InfiniteKi && Ki < InstantTransmissionKiCost)
				return false;

			Vector2 target = Main.MouseWorld;

			// Clamp to world bounds (pixels)
			target.X = MathHelper.Clamp(target.X, 16f, Main.maxTilesX * 16f - 16f);
			target.Y = MathHelper.Clamp(target.Y, 16f, Main.maxTilesY * 16f - 16f);

			// Find a safe spot: try moving upward in 16px steps if cursor is inside solid tiles.
			Vector2 tryPos = target;
			bool found = false;
			for (int i = 0; i < 30; i++)
			{
				Vector2 topLeft = tryPos - new Vector2(Player.width * 0.5f, Player.height);
				if (!Collision.SolidCollision(topLeft, Player.width, Player.height))
				{
					found = true;
					break;
				}
				tryPos.Y -= 16f;
			}

			if (!found)
				return false;

			// Spend Ki
			if (!InfiniteKi)
			{
				Ki -= InstantTransmissionKiCost;
				if (Ki < 0) Ki = 0;
			}

			// FX: departure
			SoundEngine.PlaySound(SoundID.Item8, Player.Center);
			for (int i = 0; i < 18; i++)
			{
				int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror);
				Main.dust[d].velocity *= 1.8f;
				Main.dust[d].noGravity = true;
			}

			Player.Teleport(tryPos, TeleportationStyleID.RodOfDiscord);
			Player.velocity = Vector2.Zero;

			// Multiplayer sync
			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, tryPos.X, tryPos.Y, TeleportationStyleID.RodOfDiscord);

			// FX: arrival
			SoundEngine.PlaySound(SoundID.Item8, Player.Center);
			for (int i = 0; i < 18; i++)
			{
				int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror);
				Main.dust[d].velocity *= 1.8f;
				Main.dust[d].noGravity = true;
			}

			return true;
		}

		public bool TryTransform()
		{
			if (SelectedForm == DBAForm.None)
				return false;

			if (ActiveForm == SelectedForm)
				return false;

			// Require at least some Ki to transform
			if (!InfiniteKi && Ki <= 0)
				return false;

			ActiveForm = SelectedForm;

			SoundEngine.PlaySound(SoundID.Item4, Player.Center);
			SpawnTransformBurst();
			ApplyFormBuffs();

			return true;
		}

		public void PowerDown()
		{
			if (ActiveForm == DBAForm.None)
				return;

			ActiveForm = DBAForm.None;
			ClearFormBuffs();

			for (int i = 0; i < 10; i++)
			{
				int d = Terraria.Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke);
				Main.dust[d].scale = 1.0f;
				Main.dust[d].velocity *= 1.2f;
			}

			_formDrainAccumulator = 0f;
			_auraDustTimer = 0;
		}

		private void ApplyFormBuffs()
		{
			const int refresh = 2;

			if (ActiveForm == DBAForm.PotentialUnleashed)
				Player.AddBuff(ModContent.BuffType<PotentialUnleashedBuff>(), refresh);

			if (ActiveForm == DBAForm.SuperSaiyan1)
				Player.AddBuff(ModContent.BuffType<SuperSaiyanBuff>(), refresh);
		}

		private void ClearFormBuffs()
		{
			Player.ClearBuff(ModContent.BuffType<PotentialUnleashedBuff>());
			Player.ClearBuff(ModContent.BuffType<SuperSaiyanBuff>());
		}

		private void SpawnTransformBurst()
		{
			if (ActiveForm == DBAForm.PotentialUnleashed)
			{
				for (int i = 0; i < 30; i++)
				{
					int d = Terraria.Dust.NewDust(Player.position, Player.width, Player.height, DustID.Enchanted_Pink);
					Main.dust[d].noGravity = true;
					Main.dust[d].scale = 1.25f;
					Main.dust[d].velocity *= 2.2f;
					Main.dust[d].color = Color.LightSkyBlue;
				}
			}
			else if (ActiveForm == DBAForm.SuperSaiyan1)
			{
				for (int i = 0; i < 30; i++)
				{
					int d = Terraria.Dust.NewDust(Player.position, Player.width, Player.height, DustID.GoldFlame);
					Main.dust[d].noGravity = true;
					Main.dust[d].scale = 1.25f;
					Main.dust[d].velocity *= 2.2f;
					Main.dust[d].color = Color.Gold;
				}
			}
		}

		private void SpawnFormAuraDust()
		{
			var pos = Player.Center + new Vector2(Main.rand.NextFloat(-12f, 12f), Main.rand.NextFloat(-28f, 8f));

			if (ActiveForm == DBAForm.PotentialUnleashed)
			{
				var d = Terraria.Dust.NewDustPerfect(pos, DustID.Enchanted_Pink,
					new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-1.2f, -0.2f)),
					120, Color.LightSkyBlue, 1.05f);
				d.noGravity = true;
			}
			else if (ActiveForm == DBAForm.SuperSaiyan1)
			{
				var d = Terraria.Dust.NewDustPerfect(pos, DustID.GoldFlame,
					new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-1.2f, -0.2f)),
					120, Color.Gold, 1.05f);
				d.noGravity = true;
			}
		}

		public string GetFormBoostText()
		{
			if (ActiveForm == DBAForm.None)
				return "Form: None";

			var def = DBAForms.GetDef(ActiveForm);
			return $"FormBoost ATKx{def.AtkMult:0.00} DEFx{def.DefMult:0.00} Drain {def.KiDrainPerSecond:0.0}/s Mastery {(def.Mastery * 100f):0.0}%";
		}

		public string GetTraitEffectsText() => Traits.GetTraitEffectsText(Trait);
		public string GetRaceEffectsText() => DBARaces.GetRaceEffectsText(Race);
	}
}
