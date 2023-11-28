using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    /// Utilidad para dibujar texturas con transformaciones 2D
    /// </summary>
    public class GameSprite
    {
        Rectangle srcRect;
        TGCMatrix transformationMatrix;

        /// <summary>
        /// The transformation matrix.
        /// </summary>
        public TGCMatrix TransformationMatrix
        {
            get { return transformationMatrix; }

        }

        /// <summary>
        /// The source rectangle to be drawn from the bitmap.
        /// </summary>
        public Rectangle SrcRect
        {
            get { return srcRect; }
            set { srcRect = value; }
        }


        TgcTexture texture;

        /// <summary>
        /// Texture for the sprite.
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        Color color;

        /// <summary>
        /// The color of the sprite.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private TGCVector2 position;

        /// <summary>
        /// The sprite position in the 2D plane.
        /// </summary>
        public TGCVector2 Position
        {
            get { return position; }
            set { position = value; UpdateTransformationMatrix(); }
        }

        private float rotation;

        /// <summary>
        /// The angle of rotation in radians.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; UpdateTransformationMatrix(); }
        }

        private TGCVector2 rotationCenter;

        /// <summary>
        /// The position of the centre of rotation
        /// </summary>
        public TGCVector2 RotationCenter
        {
            get { return rotationCenter; }
            set { rotationCenter = value; UpdateTransformationMatrix(); }
        }

        private TGCVector2 scalingCenter;

        /// <summary>
        /// The position of the centre of scaling
        /// </summary>
        public TGCVector2 ScalingCenter
        {
            get { return scalingCenter; }
            set { scalingCenter = value; UpdateTransformationMatrix(); }
        }

        private TGCVector2 scaling;

        /// <summary>
        /// The scaling factors in the x and y axes.
        /// </summary>
        public TGCVector2 Scaling
        {
            get { return scaling; }
            set { scaling = value; UpdateTransformationMatrix(); }
        }

        bool enabled;
        /// <summary>
        /// Habilitar sprite
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public GameSprite()
        {
            initialize();
        }

        private void initialize()
        {
            //Set the identity matrix.
            transformationMatrix = TGCMatrix.Identity;

            //Set an empty rectangle to indicate the entire bitmap.
            srcRect = Rectangle.Empty;

            //Initialize transformation properties.
            position = new TGCVector2(0, 0);
            scaling = new TGCVector2(1, 1);
            scalingCenter = new TGCVector2(0, 0);
            rotation = 0;
            rotationCenter = new TGCVector2(0, 0);


            color = Color.White;
            enabled = true;
        }

        private void UpdateTransformationMatrix()
        {
            this.transformationMatrix = TGCMatrix.Transformation2D(scalingCenter, 0, scaling, rotationCenter, rotation, position);
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            texture.dispose();
        }
    }

}
