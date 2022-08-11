using System.Linq;
using iSee.External.Data.Internal;
using iSee.External.Utils;

namespace iSee.External.Data
{
    /// <summary>
    /// Game data retrieved from process.
    /// </summary>
    public class CSGOData :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(CSGOData);

        /// <inheritdoc cref="CSGOProcess"/>
        private CSGOProcess CSGOProcess { get; set; }

        /// <inheritdoc cref="Player"/>
        public Player Player { get; set; }

        /// <inheritdoc cref="Entity"/>
        public Entity[] Entities { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public CSGOData(CSGOProcess CSGOProcess)
        {
            CSGOProcess = CSGOProcess;
            Player = new Player();
            Entities = Enumerable.Range(0, 64).Select(index => new Entity(index)).ToArray();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            Entities = default;
            Player = default;
            CSGOProcess = default;
        }

        #endregion

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!CSGOProcess.IsValid)
            {
                return;
            }

            Player.Update(CSGOProcess);
            foreach (var entity in Entities)
            {
                entity.Update(CSGOProcess);
            }
        }
    }
}
