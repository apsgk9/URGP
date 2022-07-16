public class NonLocomotionStateFactory
{
    CharacterAction _context;

    public NonLocomotionStateFactory(CharacterAction currentContext)
    {
        _context = currentContext;
    }

    public NonLocomotionBaseState Grounded()
    {
        return new NonLocomotionGroundedState(_context, this);
    }

    public NonLocomotionBaseState Airborne()
    {
        return new NonLocomotionAirborneState(_context, this);
    }

    public NonLocomotionBaseState Rising()
    {
        return new NonLocomotionRisingState(_context, this);
    }

    public NonLocomotionBaseState Falling()
    {
        return new NonLocomotionFallingState(_context, this);
    }
}

