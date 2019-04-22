using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Floor_PlanarReflection : MonoBehaviour
{
    // The script is quite long, most of the code deals with oblique-clipped
    // projection matrices and whatnot. The code is very similar to
    // what ReflectionRenderTexture script in Pro Standard Assets does,
    // but I included everything into a single file to avoid confusion and
    // dependencies.
    public int renderTextureSize;

    public float clipPlaneOffset;

    public bool disablePixelLights;

    private RenderTexture renderTexture;

    private int restorePixelLightCount;

    private Camera sourceCamera; // The camera we are going to reflect

    public virtual void Start()
    {
        this.renderTexture = new RenderTexture(this.renderTextureSize, this.renderTextureSize, 16);
        this.renderTexture.isPowerOfTwo = true;
        this.gameObject.AddComponent<Camera>();
        Camera cam = this.GetComponent<Camera>();
        Camera mainCam = Camera.main;
        cam.targetTexture = this.renderTexture;
        cam.clearFlags = mainCam.clearFlags;
        cam.backgroundColor = mainCam.backgroundColor;
        cam.nearClipPlane = mainCam.nearClipPlane;
        cam.farClipPlane = mainCam.farClipPlane;
        cam.fieldOfView = mainCam.fieldOfView;
        this.GetComponent<Renderer>().material.SetTexture("_ReflectionTex", this.renderTexture);
    }

    public virtual void Update()
    {
        Matrix4x4 scaleOffset = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
        this.GetComponent<Renderer>().material.SetMatrix("_ProjMatrix", ((scaleOffset * Camera.main.projectionMatrix) * Camera.main.worldToCameraMatrix) * this.transform.localToWorldMatrix);
    }

    public virtual void OnDisable()
    {
        UnityEngine.Object.Destroy(this.renderTexture);
    }

    public virtual void LateUpdate()
    {
         // Use main camera for reflection
        this.sourceCamera = Camera.main;
        // Figure out if we can do reflection/refraction
        if (!this.sourceCamera)
        {
            Debug.Log("Reflection rendering requires that a Camera that is tagged \"MainCamera\"! Disabling reflection.");
            this.GetComponent<Camera>().enabled = false;
        }
        else
        {
            this.GetComponent<Camera>().enabled = true;
        }
    }

    public virtual void OnPreCull()
    {
        this.sourceCamera = Camera.main;
        if (this.sourceCamera)
        {
             // find out the reflection plane: position and normal in world space
            Vector3 pos = this.transform.position;
            Vector3 normal = this.transform.up;
            // need to reflect the source camera around reflection plane
            float d = -Vector3.Dot(normal, pos) - this.clipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
            Matrix4x4 reflection = Floor_PlanarReflection.CalculateReflectionMatrix(reflectionPlane);
            this.GetComponent<Camera>().worldToCameraMatrix = this.sourceCamera.worldToCameraMatrix * reflection;
            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = this.CameraSpacePlane(pos, normal);
            this.GetComponent<Camera>().projectionMatrix = Floor_PlanarReflection.CalculateObliqueMatrix(this.sourceCamera.projectionMatrix, clipPlane);
        }
        else
        {
            this.GetComponent<Camera>().ResetWorldToCameraMatrix();
        }
    }

    public virtual void OnPreRender()
    {
         // we need to revert backface culling
        GL.invertCulling = true;
        if (this.disablePixelLights)
        {
            this.restorePixelLightCount = QualitySettings.pixelLightCount;
        }
    }

    public virtual void OnPostRender()
    {
         // restore the backface culling
        GL.invertCulling = false;
        if (this.disablePixelLights)
        {
            QualitySettings.pixelLightCount = this.restorePixelLightCount;
        }
    }

    // Given position/normal of the plane, calculates plane in camera space.
    public virtual Vector4 CameraSpacePlane(Vector3 pos, Vector3 normal)
    {
        Vector3 offsetPos = pos + (normal * this.clipPlaneOffset);
        Matrix4x4 m = this.GetComponent<Camera>().worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Extended sign: returns -1, 0 or 1 based on sign of a
    public static float sgn(float a)
    {
        if (a > 0f)
        {
            return 1f;
        }
        if (a < 0f)
        {
            return -1f;
        }
        return 0f;
    }

    // Adjusts the given projection matrix so that near plane is the given clipPlane
    // clipPlane is given in camera space. See article in GPG5.
    public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = default(Vector4);
        q.x = (Floor_PlanarReflection.sgn(clipPlane.x) + projection[8]) / projection[0];
        q.y = (Floor_PlanarReflection.sgn(clipPlane.y) + projection[9]) / projection[5];
        q.z = -1f;
        q.w = (1f + projection[10]) / projection[14];
        Vector4 c = clipPlane * (2f / Vector4.Dot(clipPlane, q));
        projection[2] = c.x;
        projection[6] = c.y;
        projection[10] = c.z + 1f;
        projection[14] = c.w;
        return projection;
    }

    // Calculates reflection matrix around the given plane
    public static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        Matrix4x4 reflectionMat = default(Matrix4x4);
        reflectionMat.m00 = 1 - ((2 * plane[0]) * plane[0]);
        reflectionMat.m01 = (-2 * plane[0]) * plane[1];
        reflectionMat.m02 = (-2 * plane[0]) * plane[2];
        reflectionMat.m03 = (-2 * plane[3]) * plane[0];
        reflectionMat.m10 = (-2 * plane[1]) * plane[0];
        reflectionMat.m11 = 1 - ((2 * plane[1]) * plane[1]);
        reflectionMat.m12 = (-2 * plane[1]) * plane[2];
        reflectionMat.m13 = (-2 * plane[3]) * plane[1];
        reflectionMat.m20 = (-2 * plane[2]) * plane[0];
        reflectionMat.m21 = (-2 * plane[2]) * plane[1];
        reflectionMat.m22 = 1 - ((2 * plane[2]) * plane[2]);
        reflectionMat.m23 = (-2 * plane[3]) * plane[2];
        reflectionMat.m30 = 0;
        reflectionMat.m31 = 0;
        reflectionMat.m32 = 0;
        reflectionMat.m33 = 1;
        return reflectionMat;
    }

    public Floor_PlanarReflection()
    {
        this.renderTextureSize = 256;
        this.clipPlaneOffset = 0.01f;
        this.disablePixelLights = true;
    }

}