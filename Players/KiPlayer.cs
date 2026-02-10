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

		// --- Progression / Scaling (baseline, low-impact) ---
		public int PowerLevel = 1; // simple progression number for later

		// Base modifiers from Race/Trait (kept as 1.0 by default)
		public float BaseAtkMult = 1f;
		public float BaseDefMult = 1f;
		public float BaseMaxKiMult = 1f;

		// Final computed (Race/Trait/PL/Form combined)
		public float EffectiveAtkMult = 1f;
		public float EffectiveDefMult = 1f;
		public int EffectiveMaxKi = 1000;

		// Internal: track last MaxKi so we can clamp Ki cleanly if MaxKi changes
		private int _lastMaxKi = 1000;

		// --- Beam charge state (for test weapon) ---
		public int BeamChargeStage = 0;          // 0..3
		public float BeamChargeProgress = 0f;    // 0..1 progress toward next stage
		public bool BeamCharging = false;        // set by weapon while right-click held
		public bool BeamFiring = false;          // set by weapon while firing

		// --- Mastery per form (0..1) ---
		public float MasteryPotentialUnleashed = 0f;
		public float MasterySuperSaiyan1 = 0f;

		// Combat gate: only direct hits set this (no DoTs)
		private int _combatTicks = 0; // counts down

		// --- Race/Trait derived scalars ---
		public float ChargeRateMult = 1f;          // affects Ki gain while charging
		public float BaseMoveSpeedBonus = 0f;      // additive to Player.moveSpeed (0.05 = +5%)
		public float PassiveKiRegenPerSecond = 0f; // Android baseline, etc.
		private float _passiveKiRegenAcc = 0f;     // internal accumulator

		public override void Initialize()
		{
			MaxKi = 1000;
			Ki = 0; // start at 0 so charging matters

			_lastMaxKi = MaxKi;
			PowerLevel = 1;

			BeamChargeStage = 0;
			BeamChargeProgress = 0f;
			BeamCharging = false;
			BeamFiring = false;

			MasteryPotentialUnleashed = 0f;
			MasterySuperSaiyan1 = 0f;
			_combatTicks = 0;

			ChargeRateMult = 1f;
			BaseMoveSpeedBonus = 0f;
			PassiveKiRegenPerSecond = 0f;
			_passiveKiRegenAcc = 0f;

			RecomputeStats();
			Ki = (int)MathHelper.Clamp(Ki, 0, MaxKi);
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

			tag["PowerLevel"] = PowerLevel;

			tag["BeamChargeStage"] = BeamChargeStage;
			tag["BeamChargeProgress"] = BeamChargeProgress;

			tag["MasteryPU"] = MasteryPotentialUnleashed;
			tag["MasterySS1"] = MasterySuperSaiyan1;
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

			PowerLevel = tag.ContainsKey("PowerLevel") ? tag.GetInt("PowerLevel") : 1;

			BeamChargeStage = tag.ContainsKey("BeamChargeStage") ? tag.GetInt("BeamChargeStage") : 0;
			BeamChargeProgress = tag.ContainsKey("BeamChargeProgress") ? tag.GetFloat("BeamChargeProgress") : 0f;
			BeamCharging = false;
			BeamFiring = false;

			MasteryPotentialUnleashed = tag.ContainsKey("MasteryPU") ? tag.GetFloat("MasteryPU") : 0f;
			MasterySuperSaiyan1 = tag.ContainsKey("MasterySS1") ? tag.GetFloat("MasterySS1") : 0f;
			_combatTicks = 0;

			ChargeRateMult = 1f;
			BaseMoveSpeedBonus = 0f;
			PassiveKiRegenPerSecond = 0f;
			_passiveKiRegenAcc = 0f;

			_lastMaxKi = MaxKi;

			RecomputeStats();
			Ki = (int)MathHelper.Clamp(Ki, 0, MaxKi);

			if (BeamChargeStage < 0) BeamChargeStage = 0;
			if (BeamChargeStage > 3) BeamChargeStage = 3;
			BeamChargeProgress = MathHelper.Clamp(BeamChargeProgress, 0f, 1f);

			MasteryPotentialUnleashed = MathHelper.Clamp(MasteryPotentialUnleashed, 0f, 1f);
			MasterySuperSaiyan1 = MathHelper.Clamp(MasterySuperSaiyan1, 0f, 1f);
		}

		// --- Weapon helpers ---
		public bool HasKi(int amount) => InfiniteKi || Ki >= amount;

		public bool TryConsumeKi(int amount)
		{
			if (InfiniteKi) return true;
			if (amount <= 0) return true;
			if (Ki < amount) return false;
			Ki -= amount;
			if (Ki < 0) Ki = 0;
			return true;
		}

		private float GetMasteryFor(DBAForm form)
		{
			return form switch
			{
				DBAForm.PotentialUnleashed => MasteryPotentialUnleashed,
				DBAForm.SuperSaiyan1 => MasterySuperSaiyan1,
				_ => 0f
			};
		}

		private void SetMasteryFor(DBAForm form, float value)
		{
			value = MathHelper.Clamp(value, 0f, 1f);
			switch (form)
			{
				case DBAForm.PotentialUnleashed:
					MasteryPotentialUnleashed = value;
					break;
				case DBAForm.SuperSaiyan1:
					MasterySuperSaiyan1 = value;
					break;
			}
		}

		private bool IsIdleForMastery()
		{
			if (Player.controlLeft || Player.controlRight || Player.controlUp || Player.controlDown)
				return false;

			return Player.velocity.LengthSquared() < 0.05f * 0.05f;
		}

		private void RecomputeStats()
		{
			BaseAtkMult = 1f;
			BaseDefMult = 1f;
			BaseMaxKiMult = 1f;

			// Trait
			var tDef = Traits.GetDef(Trait);
			BaseAtkMult *= tDef.AtkMult;
			BaseDefMult *= tDef.DefMult;
			BaseMaxKiMult *= tDef.MaxKiMult;

			// Race
			var rDef = DBARaces.GetDef(Race);
			BaseAtkMult *= rDef.AtkMult;
			BaseDefMult *= rDef.DefMult;
			BaseMaxKiMult *= rDef.MaxKiMult;

			ChargeRateMult = tDef.ChargeRateMult * rDef.ChargeRateMult;
			BaseMoveSpeedBonus = tDef.MoveSpeedBonus + rDef.MoveSpeedBonus;
			PassiveKiRegenPerSecond = rDef.PassiveKiRegenPerSecond;

			int pl = PowerLevel < 1 ? 1 : PowerLevel;

			float plAtk = 1f + (pl - 1) * 0.005f;
			float plDef = 1f + (pl - 1) * 0.003f;
			float plMaxKi = 1f + (pl - 1) * 0.002f;

			float formAtk = 1f;
			float formDef = 1f;

			if (ActiveForm != DBAForm.None)
			{
				float m = GetMasteryFor(ActiveForm);
				formAtk = DBAForms.GetAtkMult(ActiveForm, m);
				formDef = DBAForms.GetDefMult(ActiveForm, m);
			}

			EffectiveAtkMult = BaseAtkMult * plAtk * formAtk;
			EffectiveDefMult = BaseDefMult * plDef * formDef;

			EffectiveMaxKi = (int)(1000f * BaseMaxKiMult * plMaxKi);
			if (EffectiveMaxKi < 0) EffectiveMaxKi = 0;

			MaxKi = EffectiveMaxKi;

			if (MaxKi != _lastMaxKi)
			{
				_lastMaxKi = MaxKi;
				Ki = (int)MathHelper.Clamp(Ki, 0, MaxKi);
			}
		}

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

			// IMPORTANT: compute scalars first so we don’t use “stale” ChargeRateMult etc.
			RecomputeStats();

			// Apply the always-on race/trait buffs via the centralized helpers
			DBARaces.ApplyBuff(Player, Race);
			Traits.ApplyBuff(Player, Trait);

			if (_combatTicks > 0)
				_combatTicks--;

			if (InfiniteKi)
			{
				Ki = MaxKi;
				_passiveKiRegenAcc = 0f;
			}

			// Charge-only KI gain (no passive regen)
			if (!InfiniteKi && IsCharging && Ki < MaxKi)
			{
				int gain = (int)(2f * ChargeRateMult);
				if (gain < 1) gain = 1;

				Ki += gain;
				if (Ki > MaxKi) Ki = MaxKi;

				_chargeDustTimer++;
				if (_chargeDustTimer >= 3)
				{
					_chargeDustTimer = 0;
					var pos = Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 10f));
					var d = Terraria.Dust.NewDustPerfect(pos, 58, new Vector2(0f, -0.8f), 120, Color.White, 1.1f);
					d.noGravity = true;
				}
			}

			// Passive Ki regen (Android baseline, etc.)
			if (!InfiniteKi && PassiveKiRegenPerSecond > 0f && Ki < MaxKi)
			{
				_passiveKiRegenAcc += PassiveKiRegenPerSecond / 60f;
				if (_passiveKiRegenAcc >= 1f)
				{
					int add = (int)_passiveKiRegenAcc;
					_passiveKiRegenAcc -= add;

					Ki += add;
					if (Ki > MaxKi) Ki = MaxKi;
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

				// Passive mastery gain while transformed + idle (very slow)
				if (IsIdleForMastery())
				{
					var def = DBAForms.GetDef(ActiveForm);
					float mastery = GetMasteryFor(ActiveForm);

					float maxAllowed = (_combatTicks > 0) ? 1f : def.TrainingCap;

					if (mastery < maxAllowed)
					{
						mastery += def.IdleMasteryGainPerSecond / 60f;
						if (mastery > maxAllowed) mastery = maxAllowed;
						SetMasteryFor(ActiveForm, mastery);
					}
				}

				_auraDustTimer++;
				if (_auraDustTimer >= 3)
				{
					_auraDustTimer = 0;
					SpawnFormAuraDust();
				}

				if (!InfiniteKi)
				{
					float m = GetMasteryFor(ActiveForm);
					float drainPerSec = DBAForms.GetKiDrainPerSecond(ActiveForm, m);

					_formDrainAccumulator += drainPerSec / 60f;
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

			// --- Beam charge decay when idle ---
			if (!BeamCharging && !BeamFiring)
			{
				if (BeamChargeStage > 0 || BeamChargeProgress > 0f)
				{
					BeamChargeProgress -= 0.02f;
					if (BeamChargeProgress <= 0f)
					{
						if (BeamChargeStage > 0)
						{
							BeamChargeStage--;
							BeamChargeProgress = 1f;
						}
						else
						{
							BeamChargeProgress = 0f;
						}
					}
				}
			}

			BeamCharging = false;
			BeamFiring = false;

			Ki = (int)MathHelper.Clamp(Ki, 0, MaxKi);
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (EffectiveDefMult > 0f && EffectiveDefMult != 1f)
				modifiers.FinalDamage *= 1f / EffectiveDefMult;
		}

		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			_combatTicks = 6 * 60;
		}

		public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
		{
			_combatTicks = 6 * 60;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ActiveForm == DBAForm.None)
				return;

			if (damageDone <= 0)
				return;

			var def = DBAForms.GetDef(ActiveForm);
			float mastery = GetMasteryFor(ActiveForm);

			if (target.type == NPCID.TargetDummy)
			{
				float cap = def.TrainingCap;
				if (mastery < cap)
				{
					mastery += def.DummyHitMasteryGain;
					if (mastery > cap) mastery = cap;
					SetMasteryFor(ActiveForm, mastery);
				}
				return;
			}

			float maxAllowed = (_combatTicks > 0) ? 1f : def.TrainingCap;

			if (mastery < maxAllowed)
			{
				mastery += def.RealHitMasteryGain;
				if (mastery > maxAllowed) mastery = maxAllowed;
				SetMasteryFor(ActiveForm, mastery);
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			OnHitNPC(target, hit, damageDone);
		}

		public override void PostUpdateRunSpeeds()
		{
			// Make sure our race/trait/form scalars are up to date when this hook runs
			RecomputeStats();

			// Always-on race/trait move speed
			if (BaseMoveSpeedBonus != 0f)
			{
				Player.moveSpeed += BaseMoveSpeedBonus;
				Player.maxRunSpeed *= 1f + BaseMoveSpeedBonus * 0.75f;
	}

	if (ActiveForm == DBAForm.None)
		return;

	float m = GetMasteryFor(ActiveForm);
	float bonus = DBAForms.GetMoveSpeedBonus(ActiveForm, m);

	Player.moveSpeed += bonus;
	Player.maxRunSpeed *= 1f + bonus * 0.75f;
}



		private void ApplyControlledFlight()
		{
			Player.gravity = 0f;
			Player.maxFallSpeed = 0f;
			Player.noFallDmg = true;
			Player.fallStart = (int)(Player.position.Y / 16f);

			Player.wingTime = Player.wingTimeMax;
			Player.rocketTime = Player.rocketTimeMax;

			float speed = 8f;
			float accel = 0.35f;

			bool left = Player.controlLeft;
			bool right = Player.controlRight;
			bool up = Player.controlUp;
			bool down = Player.controlDown;

			if (left || right)
			{
				float targetX = (right ? 1f : 0f) + (left ? -1f : 0f);
				targetX *= speed;
				Player.velocity.X = MathHelper.Lerp(Player.velocity.X, targetX, accel);
			}
			else
			{
				Player.velocity.X = 0f;
			}

			if (up || down)
			{
				float targetY = (down ? 1f : 0f) + (up ? -1f : 0f);
				targetY *= speed;
				Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, targetY, accel);
			}
			else
			{
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

			target.X = MathHelper.Clamp(target.X, 16f, Main.maxTilesX * 16f - 16f);
			target.Y = MathHelper.Clamp(target.Y, 16f, Main.maxTilesY * 16f - 16f);

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

			if (!InfiniteKi)
			{
				Ki -= InstantTransmissionKiCost;
				if (Ki < 0) Ki = 0;
			}

			SoundEngine.PlaySound(SoundID.Item8, Player.Center);
			for (int i = 0; i < 18; i++)
			{
				int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror);
				Main.dust[d].velocity *= 1.8f;
				Main.dust[d].noGravity = true;
			}

			Player.Teleport(tryPos, TeleportationStyleID.RodOfDiscord);
			Player.velocity = Vector2.Zero;

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, tryPos.X, tryPos.Y, TeleportationStyleID.RodOfDiscord);

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

			float m = GetMasteryFor(ActiveForm);
			float atk = DBAForms.GetAtkMult(ActiveForm, m);
			float def = DBAForms.GetDefMult(ActiveForm, m);
			float drain = DBAForms.GetKiDrainPerSecond(ActiveForm, m);

			return $"FormBoost ATKx{atk:0.00} DEFx{def:0.00} Drain {drain:0.0}/s Mastery {(m * 100f):0.0}%";
		}

		public string GetTraitEffectsText() => Traits.GetTraitEffectsText(Trait);
		public string GetRaceEffectsText() => DBARaces.GetRaceEffectsText(Race);
	}
}
