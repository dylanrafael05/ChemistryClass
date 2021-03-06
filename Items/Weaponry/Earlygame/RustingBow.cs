﻿using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ChemistryClass.ModUtils;
using Microsoft.Xna.Framework;

namespace ChemistryClass.Items.Weaponry.Earlygame {
    public class RustingBow : ChemistryClassItem {

        public override void SetStaticDefaults() {

            Tooltip.SetDefault(

                "An old bow which has rusted in its age\n" +
                "Inflicts \"Rusted\", which drains enemy life slowly\n" +
                $"and decreases their defense by {ChemistryClassGlobalNPC.rustedDef}"

                );

        }

        public override void SafeSetDefaults() {

            item.damage = 13;
            item.crit = 4;
            item.knockBack = 1;

            item.width = 32;
            item.height = 32;

            item.useTime = 35;
            item.useAnimation = 35;
            item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;

            item.noMelee = true;

            item.rare = 1;
            item.value = Item.buyPrice(0, 0, 15, 0);

            item.useAmmo = AmmoID.Arrow;
            item.shoot = ModContent.ProjectileType<Projectiles.EarlygameFL.RustTippedArrow>();
            item.shootSpeed = 9;

            minutesToDecay = 4f;

            SetRefinementData(

                (ModContent.ItemType<Materials.Earlygame.RustedPowder>(), 1 / 14f)

                );

        }

        public override void AddRecipes() {

            this.SetRecipe(

                            ChemistryClass.BeakerTileID,
                            1,
                            (ModContent.ItemType<Materials.Earlygame.RustedPowder>(), 16),
                            ("\0SilverBar", 5)

            );

        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {

            type = type == ProjectileID.WoodenArrowFriendly ? item.shoot : type;
            return true;

        }

    }
}
