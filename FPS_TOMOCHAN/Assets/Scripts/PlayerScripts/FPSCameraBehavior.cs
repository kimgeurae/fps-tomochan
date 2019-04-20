using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraBehavior : MonoBehaviour
{

    Vector2 mouseLook;
    Vector2 smoothV;
    public float mouseSensitivity = 5.0f;
    public float smoothing = 2.0f;

    GameObject _player;
    Camera _fpscam;

    // Start is called before the first frame update
    void Start()
    {
        _player = this.transform.parent.gameObject;
        _fpscam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();    
    }

    void Rotate()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        md = Vector2.Scale(md, new Vector2(mouseSensitivity * smoothing, mouseSensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -87f, 87f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        _player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, _player.transform.up);
    }

    public void ApplyRecoil(float vRecoilAmount, float hRecoilAmount, bool xdirection)
    {
        mouseLook.y += vRecoilAmount;
        if (xdirection)
            mouseLook.x += hRecoilAmount;
        else
        {
            mouseLook.x -= hRecoilAmount;
        }
    }

    public void AnimateCameraFOV()
    {
        StopCoroutine("CameraFOVEffect");
        StartCoroutine("CameraFOVEffect");
    }

    IEnumerator CameraFOVEffect()
    {
        _fpscam.fieldOfView = 60;
        for (int i = 0; i < 2; i++)
        {
            _fpscam.fieldOfView = _fpscam.fieldOfView - 1;
            yield return new WaitForSeconds(0.0005f);
        }
        for (int g = 0; g < 2; g++)
        {
            _fpscam.fieldOfView = _fpscam.fieldOfView + 1;
            yield return new WaitForSeconds(0.0005f);
        }
    }
}
