using static AnimatorStateMachineEnums;

[System.Serializable]
public struct BooleanParameter
{    
    public string ParameterName;
    public  bool SetTo;
    public SetAt SetAt;
    public BooleanParameter(string inputParameterName,bool inputSetTo,SetAt inputSetAt)
    {
        ParameterName=inputParameterName;
        SetTo=inputSetTo;
        SetAt=inputSetAt;
    }
}
