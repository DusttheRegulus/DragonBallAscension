using Terraria;
using Terraria.ModLoader;

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
            tip =
                "Ki drain: (drains until mastered)\n" +
                "Attack boost: (placeholder)\n" +
                "Defense boost: (placeholder)";
        }
    }
}
