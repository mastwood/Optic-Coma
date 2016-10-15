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
    public class Light
    {
        public Color Color;
        public float LightRadius;
        public float Power;
        public Vector2 Position;
        public Light(Color color, float radius, float power, Vector2 pos)
        {
            Color = color; LightRadius = radius; Power = power; Position = pos;
        }
    }

    public class Lighting
    {
        private RenderTarget2D _colorMapRenderTarget;
        private RenderTarget2D _depthMapRenderTarget;
        private RenderTarget2D _normalMapRenderTarget;
        private RenderTarget2D _shadowMapRenderTarget;
        private Texture2D _shadowMapTexture;
        private Texture2D _colorMapTexture;
        private Texture2D _normalMapTexture;
        private Texture2D _depthMapTexture;

        private VertexDeclaration _vertexDeclaration;
        private VertexPositionTexture[] _vertices;

        private Effect _lightEffect1;
        private Effect _lightEffect2;

        static PresentationParameters pp = Foundation.graphics.GraphicsDevice.PresentationParameters;

        int width = pp.BackBufferWidth;
        int height = pp.BackBufferHeight;

        GraphicsDevice GDevice;

        SurfaceFormat format = pp.BackBufferFormat;

        public Lighting(GraphicsDevice graphicsDevice)
        {
            GDevice = graphicsDevice;

            _colorMapRenderTarget = new RenderTarget2D(GDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents); 
            _depthMapRenderTarget = new RenderTarget2D(GDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            _normalMapRenderTarget = new RenderTarget2D(GDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            _shadowMapRenderTarget = new RenderTarget2D(GDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _lightEffect1 = ScreenManager.Instance.Content.Load<Effect>("LightingShadow");
            _lightEffect2 = ScreenManager.Instance.Content.Load<Effect>("LightingCombined");

            _vertices = new VertexPositionTexture[4];
            _vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0));
            _vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
            _vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1));
            _vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1));
            _vertexDeclaration = VertexPositionTexture.VertexDeclaration;
        }

        public void Draw()
        {
            // Set the render targets
            GDevice.SetRenderTarget(_colorMapRenderTarget);

            // Clear all render targets
            GDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            // Draw your scene here

            // Reset the render target
            GDevice.SetRenderTarget(null);
            GDevice.SetRenderTarget(_normalMapRenderTarget);

            // Clear all render targets
            GDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            // Draw all your normal maps here

            GDevice.SetRenderTarget(null);
            GDevice.SetRenderTarget(_depthMapRenderTarget);

            // Clear all render targets
            GDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            // Draw all your depth maps here

            // Deactive the rander targets to resolve them
            GDevice.SetRenderTarget(null);

            // Gather all the textures from the Rendertargets
            _colorMapTexture = _colorMapRenderTarget;
            _normalMapTexture = _normalMapRenderTarget;
            _depthMapTexture = _depthMapRenderTarget;
        }
        
        private Texture2D GenerateShadowMap(List<Light> LightCollection)
        {
            GDevice.SetRenderTarget(_shadowMapRenderTarget);
            GDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);
            GDevice.BlendState = BlendState.AlphaBlend;
            GDevice.BlendState = BlendState.AlphaBlend;
            GDevice.DepthStencilState = DepthStencilState.DepthRead;
            // For every light inside the current scene, you can optimize this
            // list to only draw the lights that are visible a.t.m.
            foreach (var light in LightCollection)
            {
                _lightEffect1.CurrentTechnique = _lightEffect1.Techniques["DeferredPointLight"];
                _lightEffect1.Parameters["lightStrength"].SetValue(light.Power);
                _lightEffect1.Parameters["lightPosition"].SetValue(light.Position);
                _lightEffect1.Parameters["lightColor"].SetValue(light.Color.ToVector3());
                _lightEffect1.Parameters["lightRadius"].SetValue(light.LightRadius);

                _lightEffect1.Parameters["screenWidth"].SetValue(GDevice.Viewport.Width);
                _lightEffect1.Parameters["screenHeight"].SetValue(GDevice.Viewport.Height);
                _lightEffect1.Parameters["NormalMap"].SetValue(_normalMapTexture);
                _lightEffect1.Parameters["DepthMap"].SetValue(_depthMapTexture);

                foreach (var pass in _lightEffect1.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Draw the full screen Quad
                    GDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);

                }
            }

            // Deactivate alpha blending...
            GDevice.BlendState = BlendState.Additive;
            // Deactive the rander targets to resolve them
            GDevice.SetRenderTarget(null);

            return _shadowMapRenderTarget;
        }
        
        private void DrawCombinedMaps(float _ambientPower, Color _ambientColor, SpriteBatch spriteBatch)
        {
            _lightEffect2.CurrentTechnique = _lightEffect2.Techniques["DeferredCombined"];
            _lightEffect2.Parameters["ambient"].SetValue(_ambientPower);
            _lightEffect2.Parameters["ambientColor"].SetValue(_ambientColor.ToVector4());

            // This variable is used to boost to output of the light sources when they are combined
            // I found 4 a good value for my lights but you can also make this dynamic if you want
            _lightEffect2.Parameters["lightAmbient"].SetValue(4);
            _lightEffect2.Parameters["ColorMap"].SetValue(_colorMapTexture);
            _lightEffect2.Parameters["ShadingMap"].SetValue(_shadowMapTexture);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            foreach (var pass in _lightEffect2.CurrentTechnique.Passes)
            {
                pass.Apply();

                spriteBatch.Draw(_colorMapTexture, Vector2.Zero, Color.White);
            }
            spriteBatch.End();
        }
    }
}
