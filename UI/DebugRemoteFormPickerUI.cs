using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using DragonBallAscension.Players;
using DragonBallAscension.Systems;

namespace DragonBallAscension.UI
{
    public class DebugRemoteFormPickerUI : UIState
    {
        private UIPanel _panel;

        public override void OnInitialize()
        {
            _panel = new UIPanel();
            _panel.SetPadding(10);
            _panel.Width.Set(260f, 0f);
            _panel.Height.Set(170f, 0f);
            _panel.Left.Set(20f, 0f);
            _panel.Top.Set(280f, 0f);

            _panel.Append(new UIText("Select Form", 0.9f));

            float y = 28f;

            AddFormButton("Potential Unleashed", DBAForm.PotentialUnleashed, y); y += 36f;
            AddFormButton("Super Saiyan", DBAForm.SuperSaiyan1, y); y += 36f;

            var close = new UITextPanel<string>("Close", 0.85f);
            close.Width.Set(-0f, 1f);
            close.Height.Set(32f, 0f);
            close.Top.Set(y, 0f);
            close.OnLeftClick += (_, __) => DebugRemoteFormPickerSystem.Toggle(false);

            _panel.Append(close);
            Append(_panel);
        }

        private void AddFormButton(string label, DBAForm form, float top)
        {
            var btn = new UITextPanel<string>(label, 0.85f);
            btn.Width.Set(-0f, 1f);
            btn.Height.Set(32f, 0f);
            btn.Top.Set(top, 0f);
            btn.OnLeftClick += (_, __) =>
            {
                var kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
                kp.SelectedForm = form;
                Main.NewText($"[DBA] Selected form: {label}", Color.LightSkyBlue);
            };
            _panel.Append(btn);
        }
    }
}
