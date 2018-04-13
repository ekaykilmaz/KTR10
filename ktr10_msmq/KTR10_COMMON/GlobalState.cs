
namespace KTR10_COMMON
{
    public static class GlobalState
    {
        public static volatile EMachineState MachineState = EMachineState.None;
    }


    public enum EMachineState
    {
        None,
        CashIn,
        CashOut,
    }
}
