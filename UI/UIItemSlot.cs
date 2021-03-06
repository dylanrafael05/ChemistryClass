﻿using System;
using ChemistryClass.ModUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace ChemistryClass.UI {
    public class UIItemSlot : UIElement {

        public static readonly Color drawColor = new Color(200, 200, 200, 200);

        private Item _item;
        public Item Item {
            get => _item;
            set => SetItem(value);
        }
        internal Predicate<Item> validItem = i => true;
        internal bool allowAir = true;

        internal const float margin = 0.4f;

        internal bool tossContentsOnClose = false;
        internal bool Visible {

            get => _visible;
            set {

                if (!value && _visible && tossContentsOnClose) TossContents();
                _visible = value;

            }

        }

        internal Texture2D backTexture;

        private bool _visible = true;
        private readonly int _context;
        private readonly float _scale;

        public UIItemSlot(int context = ItemSlot.Context.BankItem, float scale = 1f, bool tossContentsOnClose = false, Texture2D backTexture = null) {

            _context = context;
            _scale = scale;

            this.tossContentsOnClose = tossContentsOnClose;

            Item = new Item();
            Item.SetDefaults();

            this.backTexture = backTexture ?? Main.inventoryBack9Texture;

            Width.Set(this.backTexture.Width * scale, 0f);
            Height.Set(this.backTexture.Height * scale, 0f);

        }

        public void TossContents() {

            if (Item.IsAir) return;

            Main.LocalPlayer.QuickSpawnClonedItem(Item, Item.stack);
            Item = new Item();
            Item.SetDefaults();

        }

        public override void OnDeactivate() {

            if (tossContentsOnClose) TossContents();

        }

        public bool SetItem(Item item) {

            if (validItem(item) || (item.IsAir && allowAir)) {

                this._item = item;
                return true;

            } else return false;

        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {

            if (!Visible) return;

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            if ( ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface ) {

                Main.LocalPlayer.mouseInterface = true;
                if (validItem(Main.mouseItem) || (Main.mouseItem.IsAir && allowAir))
                    ItemSlot.Handle(ref _item, _context);

            }

            spriteBatch.Draw(backTexture, GetDimensions().ToRectangle(), null, drawColor, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            ItemSlot.Draw(spriteBatch, ref _item, ItemSlot.Context.MouseItem, GetDimensions().ToRectangle().TopLeft());

            Main.inventoryScale = oldScale;

        }

    }
}
