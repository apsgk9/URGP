using UnityEngine;

namespace Controller
{
    public interface ICharacterController : IController
    {
        float MovementHorizontal();
        float MovementVertical();
        bool IsRunning();
        bool IsThereMovement();
        bool AttemptingToJump();
        bool Attack();
        void ResetAttack();

        //Gets the Transform from which the character movement will be reference from.
        //As for players, it will be based from the camera they are viewing from.
        Transform ViewTransform();
    }
}