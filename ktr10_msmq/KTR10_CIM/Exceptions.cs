using KT.WOSA.CIM;
using System.Collections.Generic;
using System;

namespace KTR10_CIM.Exceptions
{
    public class CashInRollbackException : BaseException
    {
        public XfsCimDefine.WFSCIMCASHINFO_dim Param1 { get; set; }

        public CashInRollbackException(XfsCimDefine.WFSCIMCASHINFO_dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }
    
    public class CashInStatusException : BaseException
    {
        public XfsCimDefine.WFSCIMCASHINSTATUS_dim Param1 { get; set; }

        public CashInStatusException(XfsCimDefine.WFSCIMCASHINSTATUS_dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class BankNoteTypeException : BaseException
    {
        public XfsCimDefine.WFSCIMNOTETYPELIST_dim Param1 { get; set; }

        public BankNoteTypeException(XfsCimDefine.WFSCIMNOTETYPELIST_dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CurrencyExponentException : BaseException
    {
        public List<XfsCimDefine.WFSCIMCURRENCYEXP> Param1 { get; set; }

        public CurrencyExponentException(List<XfsCimDefine.WFSCIMCURRENCYEXP> Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CashUnitInfoException : BaseException
    {
        public XfsCimDefine.WFSCIMCASHINFO_dim Param1 { get; set; }

        public CashUnitInfoException(XfsCimDefine.WFSCIMCASHINFO_dim Param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class CapabilityException : BaseException
    {
        public XfsCimDefine.WFSCIMCAPS Param1 { get; set; }

        public CapabilityException(XfsCimDefine.WFSCIMCAPS param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }
    
    public class GetItemsInfoException : BaseException
    {
        public XfsCimDefine.WFSCIMITEMSINFO_dim Param1 { get; set; }

        public GetItemsInfoException(XfsCimDefine.WFSCIMITEMSINFO_dim param1, int exceptionCode)
            : base(exceptionCode)
        {
            this.Param1 = Param1;
        }
    }

    public class StatusException : BaseException
    {
        public XfsCimDefine.WFSCIMSTATUS_dim Param1 { get; set; }

        public StatusException(XfsCimDefine.WFSCIMSTATUS_dim param1, int exceptionCode)
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
                return Enum.IsDefined(typeof(KTR10_CIM.Xfs30Definitions.General.GeneralErrorCodes), ExceptionCode);
            }
        }

        public KTR10_CIM.Xfs30Definitions.General.GeneralErrorCodes XfsGeneralError
        {
            get
            {
                return (KTR10_CIM.Xfs30Definitions.General.GeneralErrorCodes)Enum.ToObject(typeof(KTR10_CIM.Xfs30Definitions.General.GeneralErrorCodes), ExceptionCode);
            }
        }

        public bool IsXfsCIMError
        {
            get
            {
                return Enum.IsDefined(typeof(KTR10_CIM.Xfs30Definitions.CIM.CIMErrorCodes), ExceptionCode);
            }
        }

        public KTR10_CIM.Xfs30Definitions.CIM.CIMErrorCodes XfsCIMError
        {
            get
            {
                return (KTR10_CIM.Xfs30Definitions.CIM.CIMErrorCodes)Enum.ToObject(typeof(KTR10_CIM.Xfs30Definitions.CIM.CIMErrorCodes), ExceptionCode);
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