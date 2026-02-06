using Terraria.ModLoader;

namespace DragonBallAscension.Systems
{
	public class DBAKeybinds : ModSystem
	{
		public static ModKeybind TransformKey { get; private set; }
		public static ModKeybind PowerDownKey { get; private set; }
		public static ModKeybind ChargeKey { get; private set; }

		public static ModKeybind ToggleFlightKey { get; private set; }
		public static ModKeybind InstantTransmissionKey { get; private set; }

		public override void Load()
		{
			TransformKey = KeybindLoader.RegisterKeybind(Mod, "Transform", "X");
			PowerDownKey = KeybindLoader.RegisterKeybind(Mod, "Power Down", "V");
			ChargeKey = KeybindLoader.RegisterKeybind(Mod, "Charge Ki", "C");

			ToggleFlightKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Flight", "Q");
			InstantTransmissionKey = KeybindLoader.RegisterKeybind(Mod, "Instant Transmission", "I");
		}

		public override void Unload()
		{
			TransformKey = null;
			PowerDownKey = null;
			ChargeKey = null;

			ToggleFlightKey = null;
			InstantTransmissionKey = null;
		}
	}
}
