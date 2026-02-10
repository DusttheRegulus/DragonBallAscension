using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DragonBallAscension.Players;


namespace DragonBallAscension.Content.Buffs
{
    public class SuperSaiyanBuff : ModBuff
    {
        // Reuse vanilla icon: Hellfire (Buff_323)
        public override string Texture => "Terraria/Images/Buff_323";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            buffName = "Super Saiyan";

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

            // Only show details if this form is actually active
            if (kp.ActiveForm != DBAForm.SuperSaiyan1)
            {
                tip =
                    "Ki drain scales down with mastery\n" +
                    "Attack, defense, and movement speed scale up with mastery";
                return;
            }

            float m = kp.MasterySuperSaiyan1;
            float atk = DBAForms.GetAtkMult(DBAForm.SuperSaiyan1, m);
            float def = DBAForms.GetDefMult(DBAForm.SuperSaiyan1, m);
            float drain = DBAForms.GetKiDrainPerSecond(DBAForm.SuperSaiyan1, m);
            float move = DBAForms.GetMoveSpeedBonus(DBAForm.SuperSaiyan1, m);

            tip =
                $"Mastery: {(m * 100f):0.0}%\n" +
                $"Ki drain: {drain:0.0}/s\n" +
                $"Attack: x{atk:0.00}\n" +
                $"Defense: x{def:0.00}\n" +
                $"Move speed: +{(move * 100f):0.0}%";
        }
    }
}
