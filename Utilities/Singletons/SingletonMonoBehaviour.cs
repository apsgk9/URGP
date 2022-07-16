
using UnityEngine;
public class SingletonMonoBehaviour : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        applicationIsQuitting=false;
    }
    protected static bool applicationIsQuitting = false;

    virtual protected void OnApplicationQuit()
    {
        applicationIsQuitting=true;
    }

}