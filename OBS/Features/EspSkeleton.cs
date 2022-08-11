using System.Drawing;
using iSee.External.Data.Internal;
using iSee.External.Data.Raw;
using iSee.External.Gfx;
using iSee.External.Utils;
using Microsoft.DirectX;
using Graphics = iSee.External.Gfx.Graphics;

namespace iSee.Features
{
    /// <summary>
    /// ESP Skeleton.
    /// </summary>
    public static class EspSkeleton
    {
        /// <summary>
        /// Draw ,.
        /// </summary>
        public static void Draw(Graphics graphics)
        {
            foreach (var entity in graphics.CSGOData.Entities)
            {
                // validate
                if (!entity.IsAlive() || entity.AddressBase == graphics.CSGOData.Player.AddressBase)
                {
                    continue;
                }

                // draw
                var color = entity.Team == Team.Terrorists ? Color.Gold : Color.DodgerBlue;
                if(entity.Team != graphics.CSGOData.Player.Team)
                {
                    Draw(graphics, entity, color);
                }
            }
            Draw(graphics, GetPositionScreen(graphics));
        }

        public static void Draw(Graphics graphics, Entity entity, Color color)
        {
            for (var i = 0; i < entity.StudioHitBoxSet.numhitboxes; i++)
            {
                var (from, to) = entity.Skeleton[i];
                var hitbox = entity.StudioHitBoxes[i];
                if (hitbox.bone < 0 || hitbox.bone > Offsets.MAXSTUDIOBONES)
                {
                    return;
                }

                if (i == 0)
                {
                    graphics.DrawText(entity.Health.ToString(), Color.Red, entity.BonesPos[from], entity.BonesPos[to]);
                }

                if (hitbox.radius > 0)
                {
                    DrawHitBoxCapsule(graphics, entity, i, color);
                }
            }
        }

        /// <summary>
        /// Draw hitbox as capsule.
        /// </summary>
        private static void DrawHitBoxCapsule(Graphics graphics, Entity entity, int hitBoxId, Color color)
        {
            var hitbox = entity.StudioHitBoxes[hitBoxId];
            var matrixBoneModelToWorld = entity.BonesMatrices[hitbox.bone];

            var bonePos0World = matrixBoneModelToWorld.Transform(hitbox.bbmin);
            var bonePos1World = matrixBoneModelToWorld.Transform(hitbox.bbmax);

            graphics.DrawCapsuleWorld(color, bonePos0World, bonePos1World, hitbox.radius, 6, 2);
        }

        /// <summary>
        /// Get aim crosshair in screen space.
        /// </summary>
        public static Vector3 GetPositionScreen(Graphics graphics)
        {
            var screenSize = graphics.CSGOProcess.WindowRectangleClient.Size;
            var aspectRatio = (double)screenSize.Width / screenSize.Height;
            var player = graphics.CSGOData.Player;
            var fovY = ((double)player.Fov).DegreeToRadian();
            var fovX = fovY * aspectRatio;
            var punchX = ((double)player.AimPunchAngle.X * Offsets.weapon_recoil_scale).DegreeToRadian();
            var punchY = ((double)player.AimPunchAngle.Y * Offsets.weapon_recoil_scale).DegreeToRadian();
            var pointClip = new Vector3
            (
                (float)(-punchY / fovX),
                (float)(-punchX / fovY),
                0
            );
            return player.MatrixViewport.Transform(pointClip);
        }

        /// <summary>
        /// Draw aim crosshair in screen space.
        /// </summary>
        private static void Draw(Graphics graphics, Vector3 pointScreen)
        {
            const int radius = 3;
            var color = System.Drawing.Color.Red;
            graphics.Device.DrawPolyline(new[] { pointScreen - new Vector3(radius, 0, 0), pointScreen + new Vector3(radius + 1, 0, 0), }, color);
            graphics.Device.DrawPolyline(new[] { pointScreen - new Vector3(0, radius, 0), pointScreen + new Vector3(0, radius + 1, 0), }, color);
        }
    }
}
