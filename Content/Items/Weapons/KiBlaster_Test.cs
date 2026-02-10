using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DragonBallAscension.Players;
using DragonBallAscension.Content.Projectiles;

namespace DragonBallAscension.Content.Items.Weapons
{
	public class KiBlaster_Test : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.SpaceGun}";
		private const int KiCostPerShot = 15;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ki Blaster (Test)");
			// Tooltip.SetDefault("Fires a basic ki blast. Consumes Ki per shot.");
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;

			Item.damage = 20;
			Item.DamageType = DamageClass.Magic;
			Item.knockBack = 2f;
			Item.crit = 0;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.autoReuse = true;
			Item.noMelee = true;

			Item.shoot = ModContent.ProjectileType<KiBlastProjectile_Test>();
			Item.shootSpeed = 12f;

			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 0, 50, 0);

			Item.UseSound = SoundID.Item20;
		}

		public override bool CanUseItem(Player player)
		{
			var kp = player.GetModPlayer<KiPlayer>();
			return kp.HasKi(KiCostPerShot);
		}

		public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
			Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var kp = player.GetModPlayer<KiPlayer>();

			if (!kp.TryConsumeKi(KiCostPerShot))
				return false;

			int scaledDamage = (int)(damage * kp.EffectiveAtkMult);

			int projType = ModContent.ProjectileType<KiBlastProjectile_Test>();
			Projectile.NewProjectile(source, position, velocity, projType, scaledDamage, knockback, player.whoAmI);

			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 20)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
