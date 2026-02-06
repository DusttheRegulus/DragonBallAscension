using System;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using DragonBallAscension.Players;
using DragonBallAscension.Systems;

namespace DragonBallAscension.UI
{
    public class DebugRemoteMenuUI : UIState
    {
        private UIPanel _panel;
        private UIText _bodyText;

        public override void OnInitialize()
        {
            _panel = new UIPanel();
            _panel.Width.Set(520f, 0f);
            _panel.Height.Set(260f, 0f);
            _panel.HAlign = 0.22f;
            _panel.VAlign = 0.10f;
            Append(_panel);

            var title = new UIText("Debug Ki Remote");
            title.HAlign = 0.5f;
            title.Top.Set(8f, 0f);
            _panel.Append(title);

            _bodyText = new UIText("Loading...", 0.95f);
            _bodyText.Top.Set(40f, 0f);
            _bodyText.Left.Set(16f, 0f);
            _panel.Append(_bodyText);

            float btnTop = 210f;
            float btnW = 150f;
            float gap = 10f;
            float startX = 16f;

            var btnSelectForm = MakeBottomButton("Select Form", startX, btnTop, btnW);
            btnSelectForm.OnLeftClick += (_, __) =>
            {
                if (!TryGetKiPlayer(out var kp)) return;

                kp.SelectedForm = NextEnumValue(kp.SelectedForm);
                RefreshText();
                Main.NewText($"[DBA] Selected Form: {PrettyEnum(kp.SelectedForm)}", Color.LightGoldenrodYellow);
            };
            _panel.Append(btnSelectForm);

            var btnInfinite = MakeBottomButton("Infinite Ki", startX + btnW + gap, btnTop, btnW);
            btnInfinite.OnLeftClick += (_, __) =>
            {
                if (!TryGetKiPlayer(out var kp)) return;

                kp.InfiniteKi = !kp.InfiniteKi;
                if (kp.InfiniteKi) kp.Ki = kp.MaxKi;

                RefreshText();
                Main.NewText($"[DBA] Infinite Ki: {(kp.InfiniteKi ? "ON" : "OFF")}", Color.LightSkyBlue);
            };
            _panel.Append(btnInfinite);

            var btnClose = MakeBottomButton("Close", startX + (btnW + gap) * 2f, btnTop, btnW);
            btnClose.OnLeftClick += (_, __) => DebugRemoteMenuSystem.Close();
            _panel.Append(btnClose);

            // Do NOT call RefreshText() here (Main.LocalPlayer may not exist during load)
            _bodyText.SetText("[DBA Debug Remote]\n(Waiting for world...)");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update text only when in-world
            if (!Main.gameMenu)
                RefreshText();
        }

        private void RefreshText()
        {
            if (!TryGetKiPlayer(out var kp))
            {
                _bodyText.SetText("[DBA Debug Remote]\n(Waiting for world...)");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("[DBA Debug Remote]");
            sb.AppendLine($"Ki: {kp.Ki}/{kp.MaxKi} {(kp.InfiniteKi ? "(Infinite)" : "")}");
            sb.AppendLine($"Race: {PrettyEnum(kp.Race)}");
            sb.AppendLine($"Trait: {PrettyEnum(kp.Trait)}");
            sb.AppendLine($"Active Form: {PrettyEnum(kp.ActiveForm)}");
            sb.AppendLine($"Selected Form: {PrettyEnum(kp.SelectedForm)}");
            _bodyText.SetText(sb.ToString());
        }

        private static bool TryGetKiPlayer(out KiPlayer kp)
        {
            kp = null;

            if (Main.gameMenu)
                return false;

            if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return false;

            kp = Main.LocalPlayer.GetModPlayer<KiPlayer>();
            return kp != null;
        }

        private static UITextPanel<string> MakeBottomButton(string text, float left, float top, float width)
        {
            var btn = new UITextPanel<string>(text);
            btn.Width.Set(width, 0f);
            btn.Height.Set(36f, 0f);
            btn.Left.Set(left, 0f);
            btn.Top.Set(top, 0f);
            return btn;
        }

        private static T NextEnumValue<T>(T value) where T : struct, Enum
        {
            T[] vals = (T[])Enum.GetValues(typeof(T));
            int idx = Array.IndexOf(vals, value);
            if (idx < 0) return vals.Length > 0 ? vals[0] : value;
            return vals[(idx + 1) % vals.Length];
        }

        private static string PrettyEnum<T>(T value) where T : struct, Enum
        {
            string s = value.ToString();
            if (string.IsNullOrEmpty(s))
                return "None";

            if (string.Equals(s, "None", StringComparison.OrdinalIgnoreCase))
                return "None";

            return SplitCamelCase(s);
        }

        private static string SplitCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            var sb = new StringBuilder(s.Length + 8);
            sb.Append(s[0]);

            for (int i = 1; i < s.Length; i++)
            {
                char c = s[i];
                char prev = s[i - 1];

                if (char.IsUpper(c) && (char.IsLower(prev) || char.IsDigit(prev)))
                    sb.Append(' ');

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
