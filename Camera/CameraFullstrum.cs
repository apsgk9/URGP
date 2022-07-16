using UnityEngine;

[ExecuteAlways]
public class CameraFullstrum : MonoBehaviour
{
    public bool Mono = false;
    public bool StereoLeft = false;
    public bool StereoRight = false;
    void Update()
    {
        // this example shows the different camera frustums when using asymmetric projection matrices (like those used by OpenVR).

        var camera = GetComponent<Camera>();
        if (camera == null)
            return;
        Vector3[] frustumCorners = new Vector3[4];
        if (Mono)
        {
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

            for (int i = 0; i < 4; i++)
            {
                var worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
                Debug.DrawRay(camera.transform.position, worldSpaceCorner, Color.blue);
            }
        }
        if (StereoLeft)
        {
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Left, frustumCorners);

            for (int i = 0; i < 4; i++)
            {
                var worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
                Debug.DrawRay(camera.transform.position, worldSpaceCorner, Color.green);
            }
        }

        if (StereoRight)
        {
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Right, frustumCorners);

            for (int i = 0; i < 4; i++)
            {
                var worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
                Debug.DrawRay(camera.transform.position, worldSpaceCorner, Color.red);
            }
        }

    }
}