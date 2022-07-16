using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Pause Game")]
public class GameStateIsPausedNode : Unit
{
   [DoNotSerialize] // No need to serialize ports.
   public ControlInput inputTrigger; //Adding the ControlInput port variable

   [DoNotSerialize] // No need to serialize ports.
   public ControlOutput outputTrigger;//Adding the ControlOutput port variable.
   
  [DoNotSerialize] // No need to serialize ports
  public ValueInput myValueA; // Adding the ValueInput variable for myValueA

   protected override void Definition()
   {
        //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
        inputTrigger = ControlInput("", (flow) =>
       {
           SetPauseState(flow);
           return outputTrigger;
       });
       //Making the ControlOutput port visible and setting its key.
       outputTrigger = ControlOutput("");


      //Making the myValueA input value port visible, setting the port label name to myValueA and setting its default value to Hello.
      myValueA = ValueInput<bool>("ShouldPause", false);

      Succession(inputTrigger, outputTrigger);
   }

    private void SetPauseState(Flow flow)
    {
        if(GameState.isApplicationQuitting)
            return;
        GameState.isPaused = flow.GetValue<bool>(myValueA);
    }
    
}
