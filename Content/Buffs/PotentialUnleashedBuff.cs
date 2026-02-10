using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DragonBallAscension.Players;


namespace DragonBallAscension.Content.Buffs
{
    public class PotentialUnleashedBuff : ModBuff
    {
        // Reuse vanilla icon: Frostbite (Buff_324)
        public override string Texture => "Terraria/Images/Buff_324";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Kept empty on purpose. KiPlayer is the source of truth.
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            buffName = "Potential Unleashed";

            // Client-side only. On server, Main.LocalPlayer can be invalid.
            if (Main.netMode == NetmodeID.Server || Main.LocalPlayer == null)
            {
                tip =
                    "Ki drain scales down with mastery\n" +
                    "Attack, defense, and movement speed scale up with mastery";
                return;
            }

            var p = Main.LocalPlayer;
            var kp = p.GetModPlayer<KiPlayer>();

            if (kp.ActiveForm != DBAForm.PotentialUnleashed)
            {
                tip =
                    "Ki drain scales down with mastery\n" +
                    "Attack, defense, and movement speed scale up with mastery";
                return;
            }

            float m = kp.MasteryPotentialUnleashed;
            float atk = DBAForms.GetAtkMult(DBAForm.PotentialUnleashed, m);
            float def = DBAForms.GetDefMult(DBAForm.PotentialUnleashed, m);
            float drain = DBAForms.GetKiDrainPerSecond(DBAForm.PotentialUnleashed, m);
            float move = DBAForms.GetMoveSpeedBonus(DBAForm.PotentialUnleashed, m);

            tip =
                $"Mastery: {(m * 100f):0.0}%\n" +
                $"Ki drain: {drain:0.0}/s\n" +
                $"Attack: x{atk:0.00}\n" +
                $"Defense: x{def:0.00}\n" +
                $"Move speed: +{(move * 100f):0.0}%";
        }
    }
}
