using System.Drawing;
using System.Windows.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using iSee.External.Data;
using iSee.Features;
using iSee.External.Utils;

namespace iSee.External.Gfx
{
    public class Graphics :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(Graphics);

        /// <inheritdoc cref="Overlay" />
        private Overlay Overlay { get; set; }

        /// <inheritdoc cref="CSGOProcess" />
        public CSGOProcess CSGOProcess { get; private set; }

        /// <inheritdoc cref="CSGOData" />
        public CSGOData CSGOData { get; private set; }

        /// <inheritdoc cref="Device" />
        public Device Device { get; private set; }

        /// <inheritdoc cref="Microsoft.DirectX.Direct3D.Font" />
        public Microsoft.DirectX.Direct3D.Font FontVerdana8 { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public Graphics(Overlay Overlay, CSGOProcess CSGOProcess, CSGOData CSGOData)
        {
            Overlay = Overlay;
            CSGOProcess = CSGOProcess;
            CSGOData = CSGOData;

            InitDevice();
            FontVerdana8 = new Microsoft.DirectX.Direct3D.Font(Device, new System.Drawing.Font("Verdana", 8.0f, FontStyle.Regular));
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            FontVerdana8.Dispose();
            FontVerdana8 = default;
            Device.Dispose();
            Device = default;

            CSGOData = default;
            CSGOProcess = default;
            Overlay = default;
        }

        #endregion

        #region // routines

        private void InitDevice()
        {
            var parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindow = Overlay.Window,
                MultiSampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = Overlay.Window.Width,
                BackBufferHeight = Overlay.Window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = DepthFormat.D16,
                PresentationInterval = PresentInterval.Immediate, // turn off v-sync
            };

            Device.IsUsingEventHandlers = true;
            Device = new Device(0, DeviceType.Hardware, Overlay.Window, CreateFlags.HardwareVertexProcessing, parameters);
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!CSGOProcess.IsValid)
            {
                return;
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // set render state
                Device.RenderState.AlphaBlendEnable = true;
                Device.RenderState.AlphaTestEnable = false;
                Device.RenderState.SourceBlend = Blend.SourceAlpha;
                Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                Device.RenderState.Lighting = false;
                Device.RenderState.CullMode = Cull.None;
                Device.RenderState.ZBufferEnable = true;
                Device.RenderState.ZBufferFunction = Compare.Always;

                // clear scene
                Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(0, 0, 0, 0), 1, 0);

                // render scene
                Device.BeginScene();
                Render();
                Device.EndScene();

                // flush to screen
                Device.Present();
            }, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Render graphics.
        /// </summary>
        private void Render()
        {
            DrawWindowBorder();
            EspSkeleton.Draw(this);
        }

        /// <summary>
        ///  Draw window border.
        /// </summary>
        private void DrawWindowBorder()
        {
            this.DrawPolylineScreen(new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(CSGOProcess.WindowRectangleClient.Width - 1, 0, 0),
                new Vector3(CSGOProcess.WindowRectangleClient.Width - 1, CSGOProcess.WindowRectangleClient.Height - 1, 0),
                new Vector3(0, CSGOProcess.WindowRectangleClient.Height - 1, 0),
                new Vector3(0, 0, 0),
            }, Color.LawnGreen);
        }

        /// <inheritdoc cref="Player.MatrixViewProjectionViewport"/>
        public Vector3 TransformWorldToScreen(Vector3 value)
        {
            return CSGOData.Player.MatrixViewProjectionViewport.Transform(value);
        }

        #endregion
    }
}
