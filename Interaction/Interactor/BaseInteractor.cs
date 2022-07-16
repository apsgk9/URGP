using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
public  abstract class BaseInteractor<T> : MonoBehaviour where T:IInteractable
{
    protected List<T> FindInteractables(Vector3 origin,float initCheckRadius,LayerMask inputLayerMask,float inputRaycastedCheckRadius,float inputAdditionalCheckRadius)
    {
        List<T> InteractablesOnFieldList= new List<T>();

        /*
            Checklist
            1.) Primary Check if interactables are within a certain distance calculated using a sphere sphere
            2.) Check if interactables are isWithinSpecificBoundaries.
            3.) Check if interactables are within are not blocked by colliders. (So it doesn't go through walls)
                - Check if interactable is within the sphere that is casted
        */

        //Primary Check if interactables are within the sphere
        if (Physics.CheckSphere(origin, initCheckRadius))
        {
            Collider[] hitColliders = Physics.OverlapSphere(origin, initCheckRadius);
            foreach (var hitCollider in hitColliders)
            {
                var interactable = hitCollider.GetComponent<T>();
                if (interactable != null)
                {
                    var normalizedDeltaToInteractable = (hitCollider.transform.position - origin).normalized;
                    //Change this to differentiate how interactables within a certain area should be seperated
                    bool WithinSpecifiedBoundaries = isWithinSpecificBoundaries(origin,hitCollider.transform.position,normalizedDeltaToInteractable);

                    if (!WithinSpecifiedBoundaries)
                        continue;

                    //Check if interactables are within are not blocked by colliders.
                    var newDistance = Vector3.Distance(origin, hitCollider.transform.position);
                    RaycastHit hit;
                    if (Physics.SphereCast(origin, inputRaycastedCheckRadius, normalizedDeltaToInteractable, out hit, newDistance, inputLayerMask, QueryTriggerInteraction.UseGlobal))
                    {                        
                        //Check if interactable is within the sphere that is casted.
                        Vector3 stoppedPosition = origin + normalizedDeltaToInteractable * hit.distance;
                        Collider[] limitedColliders = Physics.OverlapSphere(stoppedPosition, inputRaycastedCheckRadius + inputAdditionalCheckRadius);

                        foreach (var limCol in limitedColliders)
                        {
                            if (limCol == hitCollider)
                            {
                                //  HandleInteractableFound(interactable,hitCollider);
                                InteractablesOnFieldList.Add(interactable);
                                break;
                            }
                        }
                    }
                }
            }
        }
        return InteractablesOnFieldList;
    }

    //Angle
    protected abstract bool isWithinSpecificBoundaries(Vector3 origin,Vector3 hitPosition,Vector3 normalizedDeltaToInteractable);
}
}