using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DragonBallAscension.Players;
using DragonBallAscension.Systems;

namespace DragonBallAscension.Content.Items.Debug
{
	public class DebugKiRemote : ModItem
	{
		public override string Texture => "Terraria/Images/Item_" + ItemID.WireKite;

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 15;
			Item.useAnimation = 15;

			Item.rare = ItemRarityID.Blue;
			Item.value = 0;
			Item.UseSound = SoundID.Item4;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool? UseItem(Player player)
		{
			KiPlayer kp = player.GetModPlayer<KiPlayer>();

			// Right click: print status
			if (player.altFunctionUse == 2)
			{
				Main.NewText(
					$"[DBA] Ki {kp.Ki}/{kp.MaxKi} | Race {DBARaces.GetRaceName(kp.Race)} | Trait {Traits.GetTraitName(kp.Trait)} | " +
					$"Form {DBAForms.GetFormName(kp.ActiveForm)} | Selected {DBAForms.GetFormName(kp.SelectedForm)} | Infinite {(kp.InfiniteKi ? "ON" : "OFF")}\n" +
					$"{kp.GetFormBoostText()} | {kp.GetTraitEffectsText()} | {kp.GetRaceEffectsText()}",
					Color.LightSteelBlue
				);
				return true;
			}

			// Left click: toggle the NEW 3-button remote menu
			if (player.HeldItem.type == Type)
				DebugRemoteMenuSystem.Toggle();

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
