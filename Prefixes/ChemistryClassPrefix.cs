﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChemistryClass {
    public class ChemistryClassPrefix : ModPrefix {

        public ChemistryClassPrefix() : base() { }

        //Category settings
        public const PrefixCategory prefixCategory = PrefixCategory.Custom;
        public sealed override PrefixCategory Category
            => prefixCategory;

        //Ensure that this cannot roll naturally
        public sealed override bool CanRoll(Item item)
            => false;

        //Ensure that this cannot be overidden.
        public sealed override float RollChance(Item item)
            => 0f;

        public virtual void SetDecayStats(ref float rateMult) {}

    }
}
