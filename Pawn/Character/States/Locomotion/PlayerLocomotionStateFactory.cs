public class PlayerLocomotionStateFactory
{
  Character _context;

  public PlayerLocomotionStateFactory(Character currentContext) {
    _context = currentContext;
  }

  public PlayerLocomotionBaseState Idle(){
    return new PlayerIdleState(_context, this);
  }
  public PlayerLocomotionBaseState Walk(){
    return  new PlayerWalkState(_context, this);
  }
  public PlayerLocomotionBaseState Run(){
    return new PlayerRunState(_context, this);
  } 
  public PlayerLocomotionBaseState Sprint(){
    return new PlayerSprintState(_context, this);
  }
 public PlayerLocomotionBaseState Airborne()
  {
    return new PlayerAirborneState(_context, this);
  }
  
  public PlayerLocomotionBaseState Rising(){
    return new PlayerRisingState(_context, this);
  }
   public PlayerLocomotionBaseState Falling(){
    return new PlayerFallingState(_context, this);
  }
  public PlayerLocomotionBaseState Jump(){
    return new PlayerJumpState(_context, this);
  }
  public PlayerLocomotionBaseState Grounded(){
    return new PlayerGroundedState(_context, this);
  }
}

