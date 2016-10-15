using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Optic_Coma
{
    public class LightV1
    {
        public bool IsVisible;
        public Color Color;
        public float LightRadius;
        public float Power;
        // Position is a vector3 because you can change how high or low the light is to determine its effective power (it dims if its higher)
        public Vector3 Position;
        public LightV1(Color color, float radius, float power, Vector3 pos)
        {
            IsVisible = true;
            Color = color;
            LightRadius = radius;
            Power = power;
            Position = pos;
        }
    }
    public enum LightType
    {
        Point
    }

    public abstract class Light
    {
        protected float _initialPower;

        public Vector3 Position { get; set; }
        public Vector4 Color;

        [ContentSerializerIgnore]
        public float ActualPower { get; set; }

        /// <summary>
        /// The Power is the Initial Power of the Light
        /// </summary>
        public float Power
        {
            get { return _initialPower; }
            set
            {
                _initialPower = value;
                ActualPower = _initialPower;
            }
        }

        public int LightDecay { get; set; }

        [ContentSerializerIgnore]
        public LightType LightType { get; private set; }

        [ContentSerializer(Optional = true)]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the spot direction. The axis of the cone.
        /// </summary>
        /// <value>The spot direction.</value>
        [ContentSerializer(Optional = true)]
        public Vector3 Direction { get; set; }

        protected Light(LightType lightType)
        {
            LightType = lightType;
        }

        public void EnableLight(bool enabled, float timeToEnable)
        {
            // If the light must be turned on
            IsEnabled = enabled;
        }

        /// <summary>
        /// Updates the Light.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!IsEnabled) return;
        }

        /// <summary>
        /// Copy all the base fields.
        /// </summary>
        /// <param name="light">The light.</param>
        protected void CopyBaseFields(Light light)
        {
            light.Color = this.Color;
            light.IsEnabled = this.IsEnabled;
            light.LightDecay = this.LightDecay;
            light.LightType = this.LightType;
            light.Position = this.Position;
            light.Power = this.Power;
        }

        public abstract Light DeepCopy();
    }
    public class PointLight : Light
    {
        public PointLight(): base(LightType.Point)
        {

        }

        public override Light DeepCopy()
        {
            var newLight = new PointLight();
            CopyBaseFields(newLight);

            return newLight;
        }
    }
    public class Lighting
    {
        public VertexPositionColorTexture[] Vertices;
        public VertexBuffer vertexBuffer;

        private EffectTechnique _lightEffectTechniquePointLight;
        private EffectParameter _lightEffectParameterStrength;
        private EffectParameter _lightEffectParameterPosition;
        private EffectParameter _lightEffectParameterConeDirection;
        private EffectParameter _lightEffectParameterLightColor;
        private EffectParameter _lightEffectParameterLightDecay;
        private EffectParameter _lightEffectParameterScreenWidth;
        private EffectParameter _lightEffectParameterScreenHeight;
        private EffectParameter _lightEffectParameterNormapMap;

        private EffectTechnique _lightCombinedEffectTechnique;
        private EffectParameter _lightCombinedEffectParamAmbient;
        private EffectParameter _lightCombinedEffectParamLightAmbient;
        private EffectParameter _lightCombinedEffectParamAmbientColor;
        private EffectParameter _lightCombinedEffectParamColorMap;
        private EffectParameter _lightCombinedEffectParamShadowMap;
        private EffectParameter _lightCombinedEffectParamNormalMap;

        private RenderTarget2D _colorMapRenderTarget;
        private RenderTarget2D _normalMapRenderTarget;
        private RenderTarget2D _shadowMapRenderTarget;

        private Effect _lightEffect1;
        private Effect _lightEffect2;

        private Color _ambientLight = new Color(.1f, .1f, .1f, 1);
        private Vector4 _ambientLightV = new Vector4(.1f, .1f, .1f, 1);
        private float _specularStrength = 1.0f;

        static PresentationParameters pp = Foundation.graphics.GraphicsDevice.PresentationParameters;

        int width = pp.BackBufferWidth;
        int height = pp.BackBufferHeight;

        GraphicsDevice GDevice;

        SurfaceFormat format = pp.BackBufferFormat;

        public Lighting(GraphicsDevice graphicsDevice, Effect l1, Effect l2, Texture2D realtex)
        {
            GDevice = graphicsDevice;
            _colorMapRenderTarget = new RenderTarget2D(GDevice, width, height);
            //_depthMapRenderTarget = new RenderTarget2D(GDevice, width, height);
            _normalMapRenderTarget = new RenderTarget2D(GDevice, width, height);
            _shadowMapRenderTarget = new RenderTarget2D(GDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _lightEffect1 = l1;
            _lightEffect2 = l2;

            Vertices = new VertexPositionColorTexture[4];
            Vertices[0] = new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.White, new Vector2(0, 0));
            Vertices[1] = new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.White, new Vector2(1, 0));
            Vertices[2] = new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.White, new Vector2(0, 1));
            Vertices[3] = new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1));
            vertexBuffer = new VertexBuffer(GDevice, typeof(VertexPositionColorTexture), Vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(Vertices);

            _lightEffectTechniquePointLight = _lightEffect1.Techniques["DeferredPointLight"];
            _lightEffectParameterConeDirection = _lightEffect1.Parameters["coneDirection"];
            _lightEffectParameterLightColor = _lightEffect1.Parameters["lightColor"];
            _lightEffectParameterLightDecay = _lightEffect1.Parameters["lightDecay"];
            _lightEffectParameterNormapMap = _lightEffect1.Parameters["NormalMap"];
            _lightEffectParameterPosition = _lightEffect1.Parameters["lightPosition"];
            _lightEffectParameterScreenHeight = _lightEffect1.Parameters["screenHeight"];
            _lightEffectParameterScreenWidth = _lightEffect1.Parameters["screenWidth"];
            _lightEffectParameterStrength = _lightEffect1.Parameters["lightStrength"];

            _lightCombinedEffectTechnique = _lightEffect2.Techniques["DeferredCombined2"];
            _lightCombinedEffectParamAmbient = _lightEffect2.Parameters["ambient"];
            _lightCombinedEffectParamLightAmbient = _lightEffect2.Parameters["lightAmbient"];
            _lightCombinedEffectParamAmbientColor = _lightEffect2.Parameters["ambientColor"];
            _lightCombinedEffectParamColorMap = _lightEffect2.Parameters["ColorMap"];
            _lightCombinedEffectParamShadowMap = _lightEffect2.Parameters["ShadingMap"];
            _lightCombinedEffectParamNormalMap = _lightEffect2.Parameters["NormalMap"];

        }

        public void Draw
            (
            SpriteBatch spriteBatch, 
            Action<SpriteBatch, Texture2D> DrawScene,
            Action<SpriteBatch, Texture2D> DrawNormals,
            List<Light> LightCollection,
            Texture2D normalMap,
            Texture2D colorMap
            )
        {
            GDevice.SetRenderTarget(_colorMapRenderTarget);

            // Clear all render targets
            GDevice.Clear(Color.Transparent);

            DrawScene(spriteBatch, colorMap);

            GDevice.SetRenderTarget(null);
            GDevice.SetRenderTarget(_normalMapRenderTarget);

            // Clear all render targets
            GDevice.Clear(Color.Transparent);

            DrawNormals(spriteBatch, normalMap);

            // Deactive the render targets to resolve them
            GDevice.SetRenderTarget(null);

            GenerateShadowMap(LightCollection);


            GDevice.Clear(Color.Black);

            // Finally draw the combined Maps onto the screen
            DrawCombinedMaps(spriteBatch);
        }
        
        // Renders lights, then wherever there is not a light, returns a dark texture
        private Texture2D GenerateShadowMap(List<Light> LightCollection)
        {
            GDevice.SetRenderTarget(_shadowMapRenderTarget);
            GDevice.Clear(Color.Transparent);

            foreach (var light in LightCollection)
            {
                if (!light.IsEnabled) continue;

                GDevice.SetVertexBuffer(vertexBuffer);

                // Draw all the light sources
                _lightEffectParameterStrength.SetValue(light.ActualPower);
                _lightEffectParameterPosition.SetValue(light.Position);
                _lightEffectParameterLightColor.SetValue(light.Color);
                _lightEffectParameterLightDecay.SetValue((float)light.LightDecay); // Value between 0.00 and 2.00   
                _lightEffect1.Parameters["specularStrength"].SetValue(_specularStrength);

                if (light.LightType == LightType.Point)
                {
                    _lightEffect1.CurrentTechnique = _lightEffectTechniquePointLight;
                }

                _lightEffectParameterScreenWidth.SetValue((float)GDevice.Viewport.Width);
                _lightEffectParameterScreenHeight.SetValue((float)GDevice.Viewport.Height);
                _lightEffect1.Parameters["ambientColor"].SetValue(_ambientLight.ToVector4());
                _lightEffectParameterNormapMap.SetValue(_normalMapRenderTarget);
                _lightEffect1.Parameters["ColorMap"].SetValue(_colorMapRenderTarget);
                _lightEffect1.CurrentTechnique.Passes[0].Apply();

                // Add Belding (Black background)
                GDevice.BlendState = BlendBlack;

                // Draw some magic
                GDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, 2);
            }

            // Deactive the rander targets to resolve them
            GDevice.SetRenderTarget(null);

            return _shadowMapRenderTarget;
        }
        
        //Combines shadow texture and light textures on the screen
        private void DrawCombinedMaps(SpriteBatch spriteBatch)
        {
            _lightEffect2.CurrentTechnique = _lightCombinedEffectTechnique;
            _lightCombinedEffectParamAmbient.SetValue(1f);
            _lightCombinedEffectParamLightAmbient.SetValue(4f);
            _lightCombinedEffectParamAmbientColor.SetValue(_ambientLightV);
            _lightCombinedEffectParamColorMap.SetValue(_colorMapRenderTarget);
            _lightCombinedEffectParamShadowMap.SetValue(_shadowMapRenderTarget);
            _lightCombinedEffectParamNormalMap.SetValue(_normalMapRenderTarget);
            _lightEffect2.CurrentTechnique.Passes[0].Apply();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, _lightEffect2);
            spriteBatch.Draw(_colorMapRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront);
        }
        public static BlendState BlendBlack = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,

            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One
        };
    }
}
