using System.Collections.Generic;
using System.Net;

namespace iSee
{
    /// <summary>
    /// https://github.com/frk1/hazedumper/blob/master/csgo.hpp
    /// </summary>
    public static class Offsets
    {
        public const int MAXSTUDIOBONES = 128; // total bones actually used
        public const float weapon_recoil_scale = 2.0f;

        public static int dwClientState;
        public static int dwClientState_ViewAngles;
        public static int dwEntityList;
        public static int dwLocalPlayer;
        public static int dwViewMatrix;

        public static int m_aimPunchAngle;
        public static int m_bDormant;
        public static int m_dwBoneMatrix;
        public static int m_iFOV;
        public static int m_iHealth;
        public static int m_iTeamNum;
        public static int m_lifeState;
        public static int m_pStudioHdr;
        public static int m_vecOrigin;
        public static int m_vecViewOffset;

        /// <summary>
        /// Fetches new offsets from well maintained dumper
        /// <summary />
        static Offsets()
        {
            #region // http fetch for offsets
            var fieldsToFill = typeof(Offsets).GetFields();

            using (WebClient web = new WebClient())
            {
                string data = web.DownloadString("https://raw.githubusercontent.com/frk1/hazedumper/master/csgo.cs");

                for(int i=0; i < fieldsToFill.Length; i++)
                {
                    int ind = data.IndexOf(fieldsToFill[i].Name + " = ") + 3 + fieldsToFill[i].Name.Length;
                    int testind = data.IndexOf(";", ind);
                    string test = data.Substring(ind, testind - ind);

                    if (!int.TryParse(test, out var fieldValue) &&
                    !int.TryParse(test.Substring(2, test.Length - 2), System.Globalization.NumberStyles.HexNumber, null, out fieldValue))
                    {
                        continue;
                    }

                    var fieldInfo = System.Linq.Enumerable.FirstOrDefault(fieldsToFill, fi => fi.Name == fieldsToFill[i].Name && fi.FieldType == typeof(int));
                    fieldInfo?.SetValue(default, fieldValue);
                }   
            }
            
            

        #endregion
    }
    }
}
