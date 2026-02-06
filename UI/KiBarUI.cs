using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using DragonBallAscension.Players;

namespace DragonBallAscension.UI
{
	public class KiBarUI : UIState
	{
		private DraggableKiPanel panel;

		public override void OnInitialize()
		{
			panel = new DraggableKiPanel();
			Append(panel);
		}

		private class DraggableKiPanel : UIPanel
		{
			private bool dragging;
			private Vector2 dragOffset;

			// We only pull saved position once, after a player exists.
			private bool initializedFromPlayer;

			public DraggableKiPanel()
			{
				Width.Set(240f, 0f);
				Height.Set(44f, 0f);

				BackgroundColor = new Color(0, 0, 0, 140);
				BorderColor = new Color(255, 255, 255, 40);

				// Safe default position (does NOT touch Main.LocalPlayer here)
				Left.Set(520f, 0f);
				Top.Set(80f, 0f);

				OnLeftMouseDown += StartDrag;
				OnLeftMouseUp += EndDrag;
			}

			private void TryInitFromPlayer()
			{
				if (initializedFromPlayer)
					return;

				// LocalPlayer can be null/invalid during early loading screens.
				if (Main.gameMenu || Main.LocalPlayer is null)
					return;

				// Get the saved position from the player and apply it.
				var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
				Left.Set(kp.KiBarX, 0f);
				Top.Set(kp.KiBarY, 0f);
				Recalculate();

				initializedFromPlayer = true;
			}

			private void StartDrag(UIMouseEvent evt, UIElement listeningElement)
			{
				dragging = true;
				dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
			}

			private void EndDrag(UIMouseEvent evt, UIElement listeningElement)
			{
				dragging = false;

				// Only save if we have a valid player.
				if (Main.gameMenu || Main.LocalPlayer is null)
					return;

				var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
				kp.KiBarX = Left.Pixels;
				kp.KiBarY = Top.Pixels;
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				TryInitFromPlayer();

				if (dragging)
				{
					Vector2 mouse = Main.MouseScreen;
					Left.Set(mouse.X - dragOffset.X, 0f);
					Top.Set(mouse.Y - dragOffset.Y, 0f);
					Recalculate();
				}

				// Keep within screen bounds
				float maxX = Main.screenWidth - Width.Pixels;
				float maxY = Main.screenHeight - Height.Pixels;

				if (Left.Pixels < 0) Left.Set(0, 0f);
				if (Top.Pixels < 0) Top.Set(0, 0f);
				if (Left.Pixels > maxX) Left.Set(maxX, 0f);
				if (Top.Pixels > maxY) Top.Set(maxY, 0f);
			}

			protected override void DrawSelf(SpriteBatch spriteBatch)
			{
				base.DrawSelf(spriteBatch);

				// If we're at menu or player isn't ready, just don't draw numbers.
				if (Main.gameMenu || Main.LocalPlayer is null)
					return;

				var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();

				CalculatedStyle dims = GetInnerDimensions();
				Vector2 barPos = new Vector2(dims.X + 10f, dims.Y + 18f);

				int barWidth = 220;
				int barHeight = 14;

				float pct = kp.MaxKi <= 0 ? 0f : (float)kp.Ki / kp.MaxKi;
				pct = MathHelper.Clamp(pct, 0f, 1f);

				Utils.DrawBorderString(
					spriteBatch,
					$"Ki {kp.Ki}/{kp.MaxKi}",
					new Vector2(dims.X + 10f, dims.Y - 2f),
					Color.White
				);

				DrawRect(spriteBatch, barPos, barWidth, barHeight, new Color(0, 0, 0, 200));
				DrawRect(spriteBatch, barPos + new Vector2(2, 2), (int)((barWidth - 4) * pct), barHeight - 4, new Color(120, 170, 255, 220));
			}

			private static void DrawRect(SpriteBatch sb, Vector2 pos, int w, int h, Color color)
			{
				Texture2D tex = TextureAssets.MagicPixel.Value;
				sb.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, w, h), color);
			}
		}
	}
}
