using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CursorChaserGame
{
    class CollidableObject
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;
        private float rotation;
        private float rotationVelocity;
        private Color[] textureData;
        private Rectangle objectRectangle;
        private float angleToPlayer;

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Texture2D Texture
        {
            get { return this.texture; }
        }

        public Color[] TextureData
        {
            get { return this.textureData; }
        }

        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        public float RotationVelocity
        {
            get { return this.rotationVelocity; }
            set { this.rotationVelocity = value; }
        }

        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public float AngleToPlayer
        {
            get { return this.angleToPlayer; }
            set { this.angleToPlayer = value; }
        }

        public Rectangle ObjectRectangle
        {
            get { return this.objectRectangle; }
            set { this.objectRectangle = value; }
        }

        public Rectangle Rect
        {
            get { return new Rectangle(0, 0, this.Texture.Width, this.Texture.Height); }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-this.Origin, 0.0f)) *
                                        Matrix.CreateRotationZ(this.Rotation) *
                                        Matrix.CreateTranslation(new Vector3(this.Position, 0.0f));
            }
        }

        public Rectangle BoundingRectangle
        {
            get { return CalculateBoundingRectangle(this.Rect, this.Transform); }
        }

        public CollidableObject(Texture2D texture, Vector2 position, float rotationVelocity)
        {
            this.LoadTexture(texture);
            this.position = position;
            this.rotationVelocity = rotationVelocity;
        }

        public void LoadTexture(Texture2D texture)
        {
            this.texture = texture;
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.textureData = new Color[texture.Width * texture.Height];
            this.texture.GetData(this.textureData);
        }

        public void MoveUp(float movementSpeed)
        {
            this.position.Y -= movementSpeed;
            this.UpdateObjectRectangle();
        }

        public void MoveLeft(float movementSpeed)
        {
            this.position.X -= movementSpeed;
            this.UpdateObjectRectangle();
        }

        public void MoveDown(float movementSpeed)
        {
            this.position.Y += movementSpeed;
            this.UpdateObjectRectangle();
        }

        public void MoveRight(float movementSpeed)
        {
            this.position.X += movementSpeed;
            this.UpdateObjectRectangle();
        }

        public void Rotate(float rotateBy)
        {
            this.rotation += rotateBy;
        }

        public void ChangeTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        public void ChangeOrigin(Vector2 origin)
        {
            this.origin = origin;
        }

        public bool IsColliding(CollidableObject collidable)
        {
            bool retval = false;

            if (this.BoundingRectangle.Intersects(collidable.BoundingRectangle))
            {
                if (IntersectPixels(this.Transform, this.Texture.Width, this.Texture.Height, this.TextureData, collidable.Transform, collidable.Texture.Width, collidable.Texture.Height, collidable.TextureData))
                {
                    retval = true;
                }
            }

            return retval;
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public void UpdateObjectRectangle()
        {
            this.ObjectRectangle = new Rectangle((int)this.position.X - this.texture.Width / 2, (int)this.position.Y - this.texture.Height / 2, this.texture.Width, this.texture.Height);
        }

        public void CalculateAngleToPlayer(float playerPosX, float playerPosY)
        {
            this.angleToPlayer = (float)Math.Atan2(playerPosY - this.position.Y, playerPosX - this.position.X);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects mirrorSprite, float opacity)
        {
            spriteBatch.Draw(this.texture, this.position, null, Color.White * opacity, this.rotation, this.origin, 1, mirrorSprite, 0);
        }
    }
}
