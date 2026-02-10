using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DragonBallAscension.Content.Projectiles
{
	public class KiBlastProjectile_Test : ModProjectile
	{
		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PurpleLaser}";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ki Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1;

			Projectile.timeLeft = 180;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;

			Projectile.light = 0.4f;
			Projectile.extraUpdates = 0;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 0.3f;
			}

			Projectile.rotation += 0.25f;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 8; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 1.6f;
			}
		}
	}
}
