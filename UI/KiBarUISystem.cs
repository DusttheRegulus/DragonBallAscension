using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace DragonBallAscension.UI
{
	public class KiBarUISystem : ModSystem
	{
		private UserInterface ui;
		private KiBarUI state;

		public override void Load()
		{
			if (Main.dedServ)
				return;

			ui = new UserInterface();
			state = new KiBarUI();
			state.Activate();
			ui.SetState(state);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			ui?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
		{
			int idx = layers.FindIndex(l => l.Name.Equals("Vanilla: Resource Bars"));
			if (idx == -1)
				return;

			layers.Insert(idx + 1, new LegacyGameInterfaceLayer(
				"DragonBallAscension: Ki Bar",
				() =>
				{
					ui.Draw(Main.spriteBatch, new GameTime());
					return true;
				},
				InterfaceScaleType.UI
			));
		}
	}
}
