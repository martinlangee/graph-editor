namespace GraphEditor.MyNodes
{
    // Sample for the use of the type of a signal which is assigned to each connector of a node.
    // It can be used to build a logic determining which connector is allowed to be linked to which other connector.
    public enum SignalType
    {
        Digital,
        Analog,
        Integer,
        PWM
    }
}
