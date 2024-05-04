using UnityEngine;

namespace OkapiKitV2
{
    public static class OkapiHelpers 
    {

        public static string GetLayerString(LayerMask mask)
        {
            string ret = "";

            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    if (ret == "") ret += "[";
                    else ret += ",";
                    ret += LayerMask.LayerToName(i);
                }
            }
            if (ret != "") ret += "]";
            else ret = "[UNDEFINED]";

            return ret;
        }
    }
}
