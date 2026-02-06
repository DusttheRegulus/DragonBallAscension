using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using DragonBallAscension.Systems;

namespace DragonBallAscension.Players
{
	public class DBAInputSystem : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			var kp = Player.GetModPlayer<KiPlayer>();

			// Charging only while key is held
			kp.IsCharging = DBAKeybinds.ChargeKey != null && DBAKeybinds.ChargeKey.Current;

			// Transform / Power down
			if (DBAKeybinds.TransformKey != null && DBAKeybinds.TransformKey.JustPressed)
				kp.TryTransform();

			if (DBAKeybinds.PowerDownKey != null && DBAKeybinds.PowerDownKey.JustPressed)
				kp.PowerDown();

			// Flight toggle
			if (DBAKeybinds.ToggleFlightKey != null && DBAKeybinds.ToggleFlightKey.JustPressed)
				kp.ToggleFlight();

			// Instant Transmission
			if (DBAKeybinds.InstantTransmissionKey != null && DBAKeybinds.InstantTransmissionKey.JustPressed)
				kp.TryInstantTransmission();
		}
	}
}
