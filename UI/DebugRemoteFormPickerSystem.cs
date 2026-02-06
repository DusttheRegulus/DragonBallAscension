using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using DragonBallAscension.UI;

namespace DragonBallAscension.Systems
{
    public class DebugRemoteFormPickerSystem : ModSystem
    {
        internal static UserInterface UIInterface;
        internal static DebugRemoteFormPickerUI UIState;
        internal static bool Visible;

        public override void Load()
        {
            if (Main.dedServ) return;
            UIInterface = new UserInterface();
            UIState = new DebugRemoteFormPickerUI();
        }

        public override void UpdateUI(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Visible)
                UIInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));
            if (idx != -1)
            {
                layers.Insert(idx, new LegacyGameInterfaceLayer(
                    "DragonBallAscension: DebugRemoteFormPicker",
                    () =>
                    {
                        if (Visible)
                            UIInterface?.Draw(Main.spriteBatch, new Microsoft.Xna.Framework.GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        public static void Toggle(bool? on = null)
        {
            Visible = on ?? !Visible;
            if (Visible)
                UIInterface?.SetState(UIState);
            else
                UIInterface?.SetState(null);
        }
    }
}
