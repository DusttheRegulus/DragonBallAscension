using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using DragonBallAscension.Players;

namespace DragonBallAscension.Systems
{
	public class DBARemoteUISystem : ModSystem
	{
		private static UserInterface _ui;
		private static RemoteUIState _state;
		private static bool _visible;

		public override void Load()
		{
			if (Main.dedServ)
				return;

			_ui = new UserInterface();
			_state = new RemoteUIState();
			_state.Activate();
		}

		public override void Unload()
		{
			_ui = null;
			_state = null;
			_visible = false;
		}

		public static void Toggle()
		{
			if (Main.dedServ || _ui == null || _state == null)
				return;

			_visible = !_visible;

			if (_visible)
				_ui.SetState(_state);
			else
				_ui.SetState(null);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (!_visible || _ui == null)
				return;

			_ui.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
		{
			if (!_visible || _ui?.CurrentState == null)
				return;

			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"DBA: Remote UI",
					delegate
					{
						_ui.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI
				));
			}
		}

		private class RemoteUIState : UIState
		{
			private UIPanel _panel;

			public override void OnInitialize()
			{
				_panel = new UIPanel();
				_panel.Width.Set(320f, 0f);
				_panel.Height.Set(220f, 0f);
				_panel.Left.Set(40f, 0f);
				_panel.Top.Set(140f, 0f);
				_panel.BackgroundColor = new Color(20, 20, 30, 200);

				Append(_panel);

				var title = new UIText("Debug Ki Remote");
				title.Left.Set(12f, 0f);
				title.Top.Set(10f, 0f);
				_panel.Append(title);

				float y = 44f;

				_panel.Append(MakeButton("Toggle Infinite Ki", y, () =>
				{
					var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
					kp.InfiniteKi = !kp.InfiniteKi;
					if (kp.InfiniteKi) kp.Ki = kp.MaxKi;
				}));

				y += 42f;

				_panel.Append(MakeButton("Select Form: Potential Unleashed", y, () =>
				{
					Main.LocalPlayer.GetModPlayer<KiPlayer>().SelectedForm = DBAForm.PotentialUnleashed;
				}));

				y += 34f;

				_panel.Append(MakeButton("Select Form: Super Saiyan 1", y, () =>
				{
					Main.LocalPlayer.GetModPlayer<KiPlayer>().SelectedForm = DBAForm.SuperSaiyan1;
				}));

				y += 34f;

				_panel.Append(MakeButton("Select Form: None", y, () =>
				{
					Main.LocalPlayer.GetModPlayer<KiPlayer>().SelectedForm = DBAForm.None;
				}));

				y += 42f;

				_panel.Append(MakeButton("Pick Race (cycle)", y, () =>
				{
					var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
					int next = ((int)kp.Race + 1) % System.Enum.GetValues(typeof(DBARace)).Length;
					kp.Race = (DBARace)next;
				}));
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				// Important: stop clicks from affecting the game while menu is open.
				if (ContainsPoint(Main.MouseScreen))
					Main.LocalPlayer.mouseInterface = true;
			}

			private static UIElement MakeButton(string text, float top, System.Action onClick)
			{
				var btn = new UITextPanel<string>(text);
				btn.Width.Set(296f, 0f);
				btn.Height.Set(28f, 0f);
				btn.Left.Set(12f, 0f);
				btn.Top.Set(top, 0f);

				btn.OnLeftClick += (_, __) => onClick();

				return btn;
			}
		}
	}
}
