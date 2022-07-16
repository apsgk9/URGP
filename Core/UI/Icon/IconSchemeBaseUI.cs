using UnityEngine;

public abstract class IconSchemeBaseUI : MonoBehaviour
{

    virtual protected void OnEnable()
    {
        UserInput.Instance.CurrentControllerTypeHasChange+=ChangeInControllerType;
        ChangeInControllerType(UserInput.Instance.CurrentUserControllerType);
    }
    
    virtual protected void OnDisable()
    {
        UserInput.Instance.CurrentControllerTypeHasChange-=ChangeInControllerType;
    }

    abstract protected void ChangeInControllerType(UserControllerType NewType);
}
