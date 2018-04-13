using KT.WOSA.CDM;
using System.Collections.Generic;
using System;

namespace KTR10_CDM.Exceptions
{
    public class TestCashUnitException : BaseException
    {
        public XfsCdmDefine.WFSCDMCUINFO_Dim Param1 { get; set; }

        public TestCashUnitException(XfsCdmDefine.WFSCDMCUINFO_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }
    
    public class SetCashUnitException : BaseException
    {
        public XfsCdmDefine.WFSCDMCUINFO_Dim Param1 { get; set; }

        public SetCashUnitException(XfsCdmDefine.WFSCDMCUINFO_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class EndExchangeException : BaseException
    {
        public XfsCdmDefine.WFSCDMCUINFO_Dim Param1 { get; set; }

        public EndExchangeException(XfsCdmDefine.WFSCDMCUINFO_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class StartExchangeException : BaseException
    {
        public XfsCdmDefine.WFSCDMCUINFO_Dim Param1 { get; set; }

        public StartExchangeException(XfsCdmDefine.WFSCDMCUINFO_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }
    
    public class DispenseException : BaseException
    {
        public XfsCdmDefine.WFSCDMDENOMINATION_Dim Param1 { get; set; }

        public DispenseException(XfsCdmDefine.WFSCDMDENOMINATION_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class DenominateException : BaseException
    {
        public XfsCdmDefine.WFSCDMDENOMINATION_Dim Param1 { get; set; }

        public DenominateException(XfsCdmDefine.WFSCDMDENOMINATION_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class PresentStatusException : BaseException
    {
        public XfsCdmDefine.WFSCDMPRESENTSTATUS_Dim Param1 { get; set; }

        public PresentStatusException(XfsCdmDefine.WFSCDMPRESENTSTATUS_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class MixTableException : BaseException
    {
        public XfsCdmDefine.WFSCDMMIXTABLE_Dim Param1 { get; set; }

        public MixTableException(XfsCdmDefine.WFSCDMMIXTABLE_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class MixTypeException : BaseException
    {
        public List<XfsCdmDefine.WFSCDMMIXTYPE> Param1 { get; set; }

        public MixTypeException(List<XfsCdmDefine.WFSCDMMIXTYPE> Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CurrencyExponentException : BaseException
    {
        public List<XfsCdmDefine.WFSCDMCURRENCYEXP> Param1 { get; set; }

        public CurrencyExponentException(List<XfsCdmDefine.WFSCDMCURRENCYEXP> Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CashUnitInfoException : BaseException
    {
        public XfsCdmDefine.WFSCDMCUINFO_Dim Param1 { get; set; }

        public CashUnitInfoException(XfsCdmDefine.WFSCDMCUINFO_Dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CapabilityException : BaseException
    {
        public XfsCdmDefine.WFSCDMCAPS Param1 { get; set; }

        public CapabilityException(XfsCdmDefine.WFSCDMCAPS param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class StatusException : BaseException
    {
        public XfsCdmDefine.WFSCDMSTATUS_Dim Param1 { get; set; }

        public StatusException(XfsCdmDefine.WFSCDMSTATUS_Dim param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class GeneralException : BaseException
    {
        public GeneralException(int exceptionCode)
            : base(exceptionCode)
        { }
        
        public GeneralException(int exceptionCode, string message)
            : base(exceptionCode, message)
        { }

        public GeneralException(int exceptionCode, string message, System.Exception inner)
            : base(exceptionCode, message, inner)
        { }
    }
    
    public class AsyncMethodCallException : BaseException
    {
        public AsyncMethodCallException(int exceptionCode)
            : base(exceptionCode)
        { }

        public AsyncMethodCallException(int exceptionCode, string message)
            : base(exceptionCode, message)
        { }

        public AsyncMethodCallException(int exceptionCode, string message, System.Exception inner)
            : base(exceptionCode, message, inner)
        { }
    }

    public class BaseException : System.Exception
    {
        private int _ExceptionCode;

        public int ExceptionCode
        {
            get
            {
                return _ExceptionCode;
            }
        }

        public bool IsXfsGeneralError
        {
            get
            {
                return Enum.IsDefined(typeof(KTR10_CDM.Xfs30Definitions.General.GeneralErrorCodes), ExceptionCode);
            }
        }

        public KTR10_CDM.Xfs30Definitions.General.GeneralErrorCodes XfsGeneralError
        {
            get
            {
                return (KTR10_CDM.Xfs30Definitions.General.GeneralErrorCodes)Enum.ToObject(typeof(KTR10_CDM.Xfs30Definitions.General.GeneralErrorCodes), ExceptionCode);
            }
        }
        
        public bool IsXfsCDMError
        {
            get
            {
                return Enum.IsDefined(typeof(KTR10_CDM.Xfs30Definitions.CDM.CDMErrorCodes), ExceptionCode);
            }
        }

        public KTR10_CDM.Xfs30Definitions.CDM.CDMErrorCodes XfsCDMError
        {
            get
            {
                return (KTR10_CDM.Xfs30Definitions.CDM.CDMErrorCodes)Enum.ToObject(typeof(KTR10_CDM.Xfs30Definitions.CDM.CDMErrorCodes), ExceptionCode);
            }
        }


        public BaseException(int exceptionCode)
            : base()
        {
            _ExceptionCode = exceptionCode;
        }
        
        public BaseException(int exceptionCode, string message)
            : base(message)
        {
            _ExceptionCode = exceptionCode;
        }

        public BaseException(int exceptionCode, string message, System.Exception inner)
            : base(message, inner)
        {
            _ExceptionCode = exceptionCode;
        }
    }

}