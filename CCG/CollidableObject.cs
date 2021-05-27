using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class CollidableObject
    {
        protected KeyboardState keyState;
        protected KeyboardState prevKeyState;

        protected Texture2D texture;
        protected Vector2 position;
        protected float rotation;
        protected float rotationVelocity;
        protected Vector2 origin;
        protected Color[] textureData;

        protected Vector2 boundsMin;
        protected Vector2 boundsMax;

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Color[] TextureData
        {
            get { return this.textureData; }
        }


        public Vector2 Position
        {
            get { return position; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public CollidableObject(Texture2D texture)
        {
            LoadTexture(texture);

            boundsMin = new Vector2(0, 0);
            boundsMax = new Vector2(Globals.screenWidth - texture.Width, Globals.screenHeight - texture.Height);
        }

        public void LoadTexture(Texture2D texture)
        {
            this.texture = texture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
        }

        public void MoveUp(float moveBy)
        {
            position.Y -= moveBy;
        }

        public void MoveDown(float moveBy)
        {
            position.Y += moveBy;
        }

        public void MoveLeft(float moveBy)
        {
            position.X -= moveBy;
        }

        public void MoveRight(float moveBy)
        {
            position.X += moveBy;
        }

        public void Rotate(float rotateBy)
        {
            rotation += rotateBy;
        }

        public virtual void Update()
        {

        }

        public virtual void Reset()
        {

        }

        public virtual void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, Color.White);
        }

        public float UpdateAngleToPalyer()
        {
            return (float)Math.Atan2(Mouse.GetState().Y - position.Y, Mouse.GetState().X - position.X);
        }

        public Rectangle Rect
        {
            get { return new Rectangle(0, 0, Texture.Width, Texture.Height); }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                                        Matrix.CreateRotationZ(Rotation) *
                                        Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            }
        }

        public Rectangle AxisAlignedRectangle
        {
            get { return new Rectangle((int)position.X - texture.Width / 2, (int)position.Y - texture.Height / 2, texture.Width, texture.Height); } 
        }

        public Rectangle BoundingRectangle
        {
            get { return CalculateBoundingRectangle(Rect, Transform); }
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
    }
}
