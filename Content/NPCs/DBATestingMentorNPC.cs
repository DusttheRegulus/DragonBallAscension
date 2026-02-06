using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DragonBallAscension.Players;

namespace DragonBallAscension.NPCs
{
    public class DBATestingMentorNPC : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_22"; // Guide

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Guide];
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.aiStyle = 0;
            NPC.friendly = true;

            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 250;

            NPC.knockBackResist = 0f;

            NPC.dontTakeDamage = true;
            NPC.immortal = true;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override bool CanChat() => true;

        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            KiPlayer kp = player.GetModPlayer<KiPlayer>();

            return
                "[DBA Testing Mentor]\n" +
                $"Race: {PrettyEnum(kp.Race)}\n" +
                $"Trait: {PrettyEnum(kp.Trait)}\n" +
                $"Active Form: {PrettyEnum(kp.ActiveForm)}\n" +
                $"Selected Form: {PrettyEnum(kp.SelectedForm)}\n\n" +
                "Button 1: Cycle Trait (testing)\n" +
                "Button 2: Cycle Race (testing)";
        }

        public override void SetChatButtons(ref string button1, ref string button2)
        {
            button1 = "Cycle Trait";
            button2 = "Cycle Race";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;
            KiPlayer kp = player.GetModPlayer<KiPlayer>();

            if (firstButton)
            {
                // Cycle trait safely (no hardcoded enum members)
                kp.Trait = NextEnumValue(kp.Trait);

                Main.npcChatText = $"Trait set to: {PrettyEnum(kp.Trait)}";
            }
            else
            {
                // Cycle race safely (no hardcoded enum members)
                kp.Race = NextEnumValue(kp.Race);

                Main.npcChatText = $"Race set to: {PrettyEnum(kp.Race)}";
            }
        }

        public override void AI()
        {
            NPC.velocity.X *= 0.8f;
            if (NPC.velocity.X is > -0.1f and < 0.1f)
                NPC.velocity.X = 0f;

            NPC.TargetClosest(false);
            NPC.direction = (Main.player[NPC.target].Center.X < NPC.Center.X) ? -1 : 1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 12.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[NPCID.Guide] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        // --- Helpers ---

        private static T NextEnumValue<T>(T value) where T : struct, Enum
        {
            T[] vals = (T[])Enum.GetValues(typeof(T));
            int idx = Array.IndexOf(vals, value);
            if (idx < 0) return vals.Length > 0 ? vals[0] : value;
            return vals[(idx + 1) % vals.Length];
        }

        private static string PrettyEnum<T>(T value) where T : struct, Enum
        {
            // Turns "PotentialUnleashed" into "Potential Unleashed"
            string s = value.ToString();
            if (string.IsNullOrEmpty(s))
                return "None";

            // Common nice-case for "None"
            if (string.Equals(s, "None", StringComparison.OrdinalIgnoreCase))
                return "None";

            return SplitCamelCase(s);
        }

        private static string SplitCamelCase(string s)
        {
            // Inserts spaces before capital letters (basic, good enough for UI text)
            if (string.IsNullOrEmpty(s)) return s;

            System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length + 8);
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
