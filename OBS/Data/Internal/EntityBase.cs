using System;
using Microsoft.DirectX;
using iSee.External.Data.Raw;
using iSee.External.Utils;

namespace iSee.External.Data.Internal
{
    public abstract class EntityBase
    {
        #region // storage

        /// <summary>
        /// Address base of entity.
        /// </summary>
        public IntPtr AddressBase { get; protected set; }

        /// <summary>
        /// Life state (true = dead, false = alive).
        /// </summary>
        public bool LifeState { get; protected set; }

        /// <summary>
        /// Health points.
        /// </summary>
        public int Health { get; protected set; }

        /// <inheritdoc cref="Team"/>
        public Team Team { get; protected set; }

        /// <summary>
        /// Model origin (in world).
        /// </summary>
        public Vector3 Origin { get; private set; }

        #endregion

        #region // routines

        /// <summary>
        /// Is entity alive?
        /// </summary>
        public virtual bool IsAlive()
        {
            return AddressBase != IntPtr.Zero &&
                   !LifeState &&
                   Health > 0 &&
                   (Team == Team.Terrorists || Team == Team.CounterTerrorists);
        }

        /// <summary>
        /// Read <see cref="AddressBase"/>.
        /// </summary>
        protected abstract IntPtr ReadAddressBase(CSGOProcess CSGOProcess);

        /// <summary>
        /// Update data from process.
        /// </summary>
        public virtual bool Update(CSGOProcess CSGOProcess)
        {
            AddressBase = ReadAddressBase(CSGOProcess);
            if (AddressBase == IntPtr.Zero)
            {
                return false;
            }

            LifeState = CSGOProcess.Process.Read<bool>(AddressBase + Offsets.m_lifeState);
            Health = CSGOProcess.Process.Read<int>(AddressBase + Offsets.m_iHealth);
            Team = CSGOProcess.Process.Read<int>(AddressBase + Offsets.m_iTeamNum).ToTeam();
            Origin = CSGOProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecOrigin);

            return true;
        }

        #endregion
    }
}
