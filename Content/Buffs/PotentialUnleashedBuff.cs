using Terraria;
using Terraria.ModLoader;

namespace DragonBallAscension.Content.Buffs
{
    public class PotentialUnleashedBuff : ModBuff
    {
        // Reuse vanilla icon: Frostbite (Buff_324)
        public override string Texture => "Terraria/Images/Buff_324";

        public override void SetStaticDefaults()
        {
            // DisplayName / Description can also be set via localization files later.
            Main.buffNoTimeDisplay[Type] = true;   // show as “active” rather than timed
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;         // don’t persist after quitting/reloading
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Keep it “sticky” while form is active by refreshing duration externally.
            // You can also put minor visual hooks here if desired.
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            buffName = "Potential Unleashed";
            tip =
                "Ki drain: (drains until mastered)\n" +
                "Attack boost: (placeholder)\n" +
                "Defense boost: (placeholder)";
        }
    }
}
