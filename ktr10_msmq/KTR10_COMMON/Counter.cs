using System;
using System.IO;
using System.Text;

namespace KTR10_COMMON
{
    public static class Counter
    {
        public static KT.WOSA.CIM.XfsCimDefine.WFSCIMCASHINFO_dim? CIMCashUnits;
        /// <summary>
        /// Set Null if CIM Cash Unit is changed
        /// </summary>
        public static KT.WOSA.CDM.XfsCdmDefine.WFSCDMCUINFO_Dim? CDMCashUnits;

        public static void SetCashUnits(KT.WOSA.CIM.XfsCimDefine.WFSCIMCASHINFO_dim? input1)
        {
            if (input1.HasValue)
            {
                CIMCashUnits = input1;
                CDMCashUnits = null;
            }
        }

        public static void SetCashUnits(KT.WOSA.CDM.XfsCdmDefine.WFSCDMCUINFO_Dim? input1)
        {
            if (input1.HasValue)
            {
                CDMCashUnits = input1;
            }
        }

        public static int GetCashInRetractCounter(bool UseCache)
        {
            int ret = 0;

            if (UseCache && CIMCashUnits.HasValue)
            {
                //USE CACHE
            }
            else
            {
                KTR10_CIM.Xfs30Api xfs30api = new KTR10_CIM.Xfs30Api();
                CIMCashUnits = xfs30api.WFS_INF_CIM_CASH_UNIT_INFO();
                xfs30api.Close();
            }

            if (CIMCashUnits.HasValue)
            {
                foreach (var item in CIMCashUnits.Value.lppCashInDim)
                {
                    KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINType UnitType = KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINTypeConvert(item.fwType);

                    if (UnitType == KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINType.WFS_CIM_TYPERETRACTCASSETTE)
                    {
                        ret += (int)item.ulCount;
                    }
                }
            }

            return ret;
        }
    }
}
