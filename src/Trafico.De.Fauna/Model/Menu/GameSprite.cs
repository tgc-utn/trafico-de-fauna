using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    ///     Utilidad para dibujar texturas con transformaciones 2D
    /// </summary>
    public class GameSprite
    {
        private TGCVector2 position;

        private float rotation;

        private TGCVector2 rotationCenter;

        private TGCVector2 scaling;

        private TGCVector2 scalingCenter;


        public GameSprite()
        {
            initialize();
        }

        /// <summary>
        ///     The transformation matrix.
        /// </summary>
        public TGCMatrix TransformationMatrix { get; private set; }

        /// <summary>
        ///     The source rectangle to be drawn from the bitmap.
        /// </summary>
        public Rectangle SrcRect { get; set; }

        /// <summary>
        ///     Texture for the sprite.
        /// </summary>
        public TGCTexture Texture { get; set; }

        /// <summary>
        ///     The color of the sprite.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     The sprite position in the 2D plane.
        /// </summary>
        public TGCVector2 Position
        {
            get => position;
            set
            {
                position = value;
                UpdateTransformationMatrix();
            }
        }

        /// <summary>
        ///     The angle of rotation in radians.
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                UpdateTransformationMatrix();
            }
        }

        /// <summary>
        ///     The position of the centre of rotation
        /// </summary>
        public TGCVector2 RotationCenter
        {
            get => rotationCenter;
            set
            {
                rotationCenter = value;
                UpdateTransformationMatrix();
            }
        }

        /// <summary>
        ///     The position of the centre of scaling
        /// </summary>
        public TGCVector2 ScalingCenter
        {
            get => scalingCenter;
            set
            {
                scalingCenter = value;
                UpdateTransformationMatrix();
            }
        }

        /// <summary>
        ///     The scaling factors in the x and y axes.
        /// </summary>
        public TGCVector2 Scaling
        {
            get => scaling;
            set
            {
                scaling = value;
                UpdateTransformationMatrix();
            }
        }

        /// <summary>
        ///     Habilitar sprite
        /// </summary>
        public bool Enabled { get; set; }

        private void initialize()
        {
            //Set the identity matrix.
            TransformationMatrix = TGCMatrix.Identity;

            //Set an empty rectangle to indicate the entire bitmap.
            SrcRect = Rectangle.Empty;

            //Initialize transformation properties.
            position = new TGCVector2(0, 0);
            scaling = new TGCVector2(1, 1);
            scalingCenter = new TGCVector2(0, 0);
            rotation = 0;
            rotationCenter = new TGCVector2(0, 0);


            Color = Color.White;
            Enabled = true;
        }

        private void UpdateTransformationMatrix()
        {
            TransformationMatrix =
                TGCMatrix.Transformation2D(scalingCenter, 0, scaling, rotationCenter, rotation, position);
        }

        /// <summary>
        ///     Liberar recursos
        /// </summary>
        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
