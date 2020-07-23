﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;


namespace ChemistryClass {

    public static class CCUtils {

        //MATHEMATICAL EXTENSIONS
        public static void Clamp(this ref long val, long min, long max)
            => val = val < min ? min : (val > max ? max : val);

        public static void Clamp(this ref float val, float min, float max)
            => val = val < min ? min : (val > max ? max : val);

        public static void Clamp(this ref decimal val, decimal min, decimal max)
            => val = val < min ? min : (val > max ? max : val);

        public static bool Invert(this ref bool val) => val = !val;

        //CONVERSION EXTENSIONS
        public static ChemistryClassPlayer Chemistry(this Player player)
            => player.GetModPlayer<ChemistryClassPlayer>();

        public static ChemistryClassItem Chemistry(this Item item)
            => item.modItem as ChemistryClassItem;

        public static bool IsChemistry(this Item item)
            => item.modItem is ChemistryClassItem;

        public static StyleDimension ToStyleDimension(this float i)
            => new StyleDimension(i, 0);

        //RECTANGLE / UI EXTENSIONS
        public static float MinDimension(this Rectangle i)
            => Math.Min(i.Width, i.Height);
        public static float MinDimension(this Texture2D i)
            => Math.Min(i.Width, i.Height);

        public static float MaxDimension(this Rectangle i)
            => Math.Max(i.Width, i.Height);
        public static float MaxDimension(this Texture2D i)
            => Math.Max(i.Width, i.Height);

        public static void AddMargin(this Rectangle i, int margin) {

            i.X += margin;
            i.Y += margin;
            i.Width  -= margin * 2;
            i.Height -= margin * 2;

        }

        public static void AddMargin(this Rectangle i, float margin)
            => i.AddMargin((int)margin);

        public static void ShrinkBy(this Rectangle i, float margin)
            => new Rectangle(

                (int)(i.Center.X - (i.Width / 2)*(1 - margin)),
                (int)(i.Center.Y - (i.Height / 2)*(1 - margin)),
                (int)(i.Width * (1 - margin)),
                (int)(i.Height * (1 - margin))

                );

        public static bool ContainsMouse<T>(this T uiEl) where T : UIElement
            => uiEl.ContainsPoint(Main.MouseScreen);

        //RECIPE HELPER
        public static void SetRecipe( this ModItem item, int requireTile = TileID.WorkBenches, int amount = 1, params (int, int)[] items ) {

            ModRecipe recipe = new ModRecipe(item.mod);

            recipe.AddTile(requireTile);
            foreach( var ing in items ) {

                recipe.AddIngredient(ing.Item1, ing.Item2);

            }

            recipe.SetResult(item.item.type, amount);

            recipe.AddRecipe();

        }

        //COLOR MIXING
        public static Color MixRGB(this Color one, Color two, float f1 = 1f, float f2 = 1f) {

            double r = Math.Sqrt((one.R * one.R * f1 + two.R * two.R * f2) / (f1 + f2));
            double g = Math.Sqrt((one.G * one.G * f1 + two.G * two.G * f2) / (f1 + f2));
            double b = Math.Sqrt((one.B * one.B * f1 + two.B * two.B * f2) / (f1 + f2));

            return new Color((int)r, (int)g, (int)b, 0);

        }

        public static Color MatchAlpha(this Color one, Color two)
            => new Color(one.R, one.G, one.B, two.A);

        public static Color WithAlpha(this Color one, float a)
            => new Color(one.R, one.G, one.B, a);

        //CONSTS
        public const float PI = (float)Math.PI;
        public const float HALF_PI = PI / 2;
        public const float THIRD_PI = PI / 3;
        public const float QUART_PI = PI / 4;
        public const float TWO_PI = PI * 2;

    }

}