using System;
using iSee.External.Data;
using iSee.External.Gfx;

namespace iSee
{
    /// <summary>
    /// Main program.
    /// </summary>
    public class Program :
        System.Windows.Application,
        IDisposable
    {
        #region 

        /// <summary />
        [STAThread]
        public static void Main() => new Program().Run();

        #endregion

        #region // storage

        /// <inheritdoc cref="CSGOProcess"/>
        private CSGOProcess CSGOProcess { get; set; }

        /// <inheritdoc cref="CSGOData"/>
        private CSGOData CSGOData { get; set; }

        /// <inheritdoc cref="Overlay"/>
        private Overlay Overlay { get; set; }

        /// <inheritdoc cref="Graphics"/>
        private Graphics Graphics { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        public Program()
        {
            Startup += (sender, args) => Ctor();
            Exit += (sender, args) => Dispose();
        }

        /// <summary />
        public void Ctor()
        {
            CSGOProcess = new CSGOProcess();
            CSGOData = new CSGOData(CSGOProcess);
            Overlay = new Overlay(CSGOProcess);
            Graphics = new Graphics(Overlay, CSGOProcess, CSGOData);

            CSGOProcess.Start();
            CSGOData.Start();
            Overlay.Start();
            Graphics.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Graphics.Dispose();
            Graphics = default;

            Overlay.Dispose();
            Overlay = default;

            CSGOData.Dispose();
            CSGOData = default;

            CSGOProcess.Dispose();
            CSGOProcess = default;
        }

        #endregion
    }
}
