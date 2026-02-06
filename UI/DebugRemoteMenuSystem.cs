using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using DragonBallAscension.UI;

namespace DragonBallAscension.Systems
{
    public class DebugRemoteMenuSystem : ModSystem
    {
        internal static UserInterface UIInterface;
        internal static DebugRemoteMenuUI UIState;
        internal static bool Visible;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            UIInterface = new UserInterface();
            UIState = new DebugRemoteMenuUI(); // DO NOT Activate here (no LocalPlayer yet)
            Visible = false;
        }

        public override void Unload()
        {
            UIInterface = null;
            UIState = null;
            Visible = false;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Visible || UIInterface == null)
                return;

            UIInterface.Update(gameTime);

            // Allow UI to capture input while open
            if (Main.LocalPlayer != null)
                Main.LocalPlayer.mouseInterface = true;

            // ESC closes (edge-triggered)
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
                Close();
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));
            if (idx != -1)
            {
                layers.Insert(idx, new LegacyGameInterfaceLayer(
                    "DragonBallAscension: DebugRemoteMenu",
                    () =>
                    {
                        if (Visible && UIInterface?.CurrentState != null)
                            UIInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        public static void Open()
        {
            if (Main.dedServ || UIInterface == null || UIState == null)
                return;

            // Safe: Activate only when a world/player exists
            if (Main.gameMenu)
                return;

            if (UIInterface.CurrentState == null)
                UIState.Activate();

            Visible = true;
            UIInterface.SetState(UIState);
        }

        public static void Close()
        {
            Visible = false;
            UIInterface?.SetState(null);
        }

        public static void Toggle(bool? on = null)
        {
            bool next = on ?? !Visible;
            if (next) Open();
            else Close();
        }
    }
}
