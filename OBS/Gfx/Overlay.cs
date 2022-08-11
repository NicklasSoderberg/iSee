using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using iSee.External.Data;
using iSee.External.Sys;
using iSee.External.Sys.Data;
using iSee.External.Utils;
using Point = System.Drawing.Point;

namespace iSee.External.Gfx
{
    /// <summary>
    /// Overlay window for graphics.
    /// </summary>
    public class Overlay :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(Overlay);

        /// <inheritdoc />
        protected override TimeSpan ThreadFrameSleep { get; set; } = new TimeSpan(0, 0, 0, 0, 250);

        /// <inheritdoc cref="CSGOProcess"/>
        private CSGOProcess CSGOProcess { get; set; }

        /// <summary>
        /// Physical overlay window.
        /// </summary>
        public Form Window { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public Overlay(CSGOProcess CSGOProcess)
        {
            CSGOProcess = CSGOProcess;

            // create window
            Window = new Form
            {
                Name = "Overlay Window",
                Text = "Overlay Window",
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.None,
                TopMost = true,
                Width = 16,
                Height = 16,
                Left = -32000,
                Top = -32000,
                StartPosition = FormStartPosition.Manual
            };

            Window.Load += (sender, args) =>
            {
                var exStyle = User32.GetWindowLong(Window.Handle, User32.GWL_EXSTYLE);
                exStyle |= User32.WS_EX_LAYERED;
                exStyle |= User32.WS_EX_TRANSPARENT;

                // make the window's border completely transparent
                User32.SetWindowLong(Window.Handle, User32.GWL_EXSTYLE, (IntPtr)exStyle);

                // set the alpha on the whole window to 255 (solid)
                User32.SetLayeredWindowAttributes(Window.Handle, 0, 255, User32.LWA_ALPHA);
            };
            Window.SizeChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.LocationChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.Closed += (sender, args) => System.Windows.Application.Current.Shutdown();

            // show window
            Window.Show();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            Window.Close();
            Window.Dispose();
            Window = default;

            CSGOProcess = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Extend the window frame into the client area.
        /// </summary>
        private void ExtendFrameIntoClientArea()
        {
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1,
            };
            Dwmapi.DwmExtendFrameIntoClientArea(Window.Handle, ref margins);
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            Update(CSGOProcess.WindowRectangleClient);
        }

        /// <summary>
        /// Update position and size.
        /// </summary>
        private void Update(Rectangle windowRectangleClient)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (Window.Location != windowRectangleClient.Location || Window.Size != windowRectangleClient.Size)
                {
                    if (windowRectangleClient.Width > 0 && windowRectangleClient.Height > 0)
                    {
                        // valid
                        Window.Location = windowRectangleClient.Location;
                        Window.Size = windowRectangleClient.Size;
                    }
                    else
                    {
                        // invalid
                        Window.Location = new Point(-32000, -32000);
                        Window.Size = new Size(16, 16);
                    }
                }
            }, DispatcherPriority.Normal);
        }

        #endregion
    }
}
