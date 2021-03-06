﻿using System;
using System.IO;
using ChemistryClass.ModUtils;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Steamworks;
using Terraria.ID;
using log4net.Util;

namespace ChemistryClass.Projectiles {
    public class LightningBase : ModProjectilePlus {

        public override string Texture => "Terraria/Item_0";

        public ProceduralLineSequence sequence = new ProceduralLineSequence();
        public Vector2 currentTVec = Vector2.Zero;
        public float lastTurn = 0f;

        public virtual float Movement => projectile.velocity.Length() * 3f;

        public virtual float TargetFLength => Movement * 5;
        public virtual float TargetSLength => Movement * Main.rand.NextFloat(1.1f, 1.5f);

        public virtual int Width => 8;
        public virtual int CollisionWidth => Width * 2;
        public virtual int TileCollisionWidth => 1;

        public virtual int Penetrate => 0;
        public virtual bool TileCollide => true;

        public virtual int TimeLeft => 300;

        public virtual int DustType => DustID.Electric;
        public virtual float DustChance => 0.03f;

        public virtual bool UseDisco => false;
        public virtual float DiscoMult => 0.2f;

        public virtual bool MakeShockSound => projectile.hostile;

        public static Color LineColor { get; protected set; } = new Color(100, 255, 255, 180);
        public static Color MidColor { get; protected set; } = new Color(255, 255, 255, 200);

        public float Collisions {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public float LastHitTimer {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public virtual void SaferSetDefaults() { }

        public sealed override void SafeSetDefaults() {

            SaferSetDefaults();

            projectile.width = 1;
            projectile.height = 1;

            projectile.tileCollide = false;

            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;

        }

        public sealed override bool ShouldUpdatePosition() => false;
        public sealed override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            float _ = 0;
            return sequence.IsValid? LineSequenceCollision.CheckAAABCollision(targetHitbox, sequence, CollisionWidth, ref _) : false;
        }

        public override void FirstFrame() {

            sequence = new ProceduralLineSequence(projectile.Center);
            currentTVec = projectile.velocity;

            Collisions = 0;
            LastHitTimer = 0;

            projectile.netUpdate = true;

            CritChance = 0;

        }

        public virtual void NewDust(float x, float y, float scale)
            => Dust.NewDust(new Vector2(x, y), 0, 0, DustType, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f), Scale: scale);

        public bool PlotTryDust(int x, int y) {
            if (Main.rand.NextFloat() < DustChance) NewDust(x, y, 0.5f);
            return true;
        }

        public virtual Entity GetTarget() {
            Entity ret = null;
            int retLife = int.MinValue;
            float curDist;
            Vector2 end = sequence.VisualEndpoint;
            foreach (NPC npc in Main.npc) {
                if (!npc.active) continue;
                curDist = npc.Distance(end);
                if (curDist > 400f) continue;
                if (!npc.CanBeChasedBy() || npc.friendly || npc.immortal) continue;
                if (npc.life <= retLife) continue;
                ret = npc;
                retLife = npc.life;
            }
            return ret;
        }

        public sealed override void AI() {

            //GET TARGET
            Entity targ = GetTarget();
            Vector2 targetVec = targ == null ? currentTVec : targ.Center - sequence.Endpoint;

            //HANDLE TILE COLLISIONS IF NECESSARY
            if (TileCollide) {
                if (LineSequenceCollision.CheckTileCollision(sequence, CollisionWidth))
                    Collisions += Penetrate + 1;
            }

            //CALCULATE IF THE LIGHTNING SHOULD CONTINUE TO EXPAND
            bool shouldExpand = Collisions <= Penetrate && ActiveCounter <= TimeLeft;

            //ADVANCE THE SEQUENCE
            sequence.AdvanceSequence(
                Movement,
                TargetFLength,
                () => LineGeneration.Lightning(
                        sequence,
                        TargetSLength,
                        targetVec,
                        ref currentTVec,
                        ref lastTurn
                        ),
                shouldExpand
                );

            //SPAWN DUSTS
            sequence.Plot(Width, PlotTryDust);

            //SPAWN END DUST-BURST
            if(!shouldExpand) {
                for (int i = 0; i < 3; i++) NewDust(sequence.VisualEndpoint.X, sequence.VisualEndpoint.Y, 1f);
            }

            //KILL THE PROJECTILE IF THE SEQUENCE HAS ENDED OR CORRUPTED
            if (!sequence.IsValid) projectile.Kill();

            //INCREMENT THE COLLISION TIMER
            LastHitTimer++;

        }

        public override void SafeSendExtraAI(BinaryWriter writer) {
            if (sequence.IsValid) {
                sequence.Send(writer);
            }
            writer.WriteVector2(currentTVec);
            writer.Write(lastTurn);
        }

        public override void SafeReceiveExtraAI(BinaryReader reader) {
            lastTurn = reader.ReadSingle();
            currentTVec = reader.ReadVector2();
            ProceduralLineSequence seq = ProceduralLineSequence.ReadSeq(reader);
            if (seq.IsValid) {
                sequence = seq;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {

            //DONT DRAW IS THE SEQUENCE CANNOT BE DRAWN
            if (!sequence.IsValid) return false;

            spriteBatch.DrawLineSequence(sequence, Width, -Main.screenPosition, color: UseDisco ? Main.DiscoColor * DiscoMult : LineColor);
            spriteBatch.DrawLineSequence(sequence, Width / 2, -Main.screenPosition, color: MidColor);

            return false;

        }

        public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            if (LastHitTimer > 1) {
                Collisions++;
                if(MakeShockSound) Main.PlaySound(SoundID.Item93.WithVolume(0.8f));
            }
            LastHitTimer = 0;
        }

        public sealed override void OnHitPlayer(Player target, int damage, bool crit) {
            if (LastHitTimer > 1) {
                Collisions++;
                if (MakeShockSound) Main.PlaySound(SoundID.Item93.WithVolume(0.8f));
            }
            LastHitTimer = 0;
        }

    }
}
