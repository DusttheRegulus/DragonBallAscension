using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.DataStructures;   // <-- ADD THIS
using DragonBallAscension.NPCs;

namespace DragonBallAscension.Systems
{
    public class DBATestingMentorSystem : ModSystem
    {
        private bool _spawnAttemptedThisWorldLoad;

        public override void OnWorldLoad()
        {
            _spawnAttemptedThisWorldLoad = false;
        }

        public override void PostWorldGen()
        {
            TrySpawnMentor();
        }

        public override void PostUpdateWorld()
        {
            if (!_spawnAttemptedThisWorldLoad && !Main.gameMenu)
            {
                _spawnAttemptedThisWorldLoad = true;
                TrySpawnMentor();
            }
        }

        private static void TrySpawnMentor()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int type = ModContent.NPCType<DBATestingMentorNPC>();
            if (NPC.AnyNPCs(type))
                return;

            int tileX = Main.spawnTileX;
            int tileY = Main.spawnTileY;

            int maxScan = 80;
            while (maxScan-- > 0 && tileY < Main.maxTilesY - 10)
            {
                if (WorldGen.SolidTile(tileX, tileY) || WorldGen.SolidTile(tileX, tileY + 1))
                    break;

                tileY++;
            }

            Vector2 spawnPos = new Vector2(tileX * 16f, (tileY - 2) * 16f);

            int npcIndex = NPC.NewNPC(
                new EntitySource_Misc("DBA_TestingMentorSpawn"),
                (int)spawnPos.X,
                (int)spawnPos.Y,
                type
            );

            if (npcIndex >= 0 && Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: npcIndex);
        }
    }
}
