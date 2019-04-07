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

    // Start is called before the first frame update
    void Start()
    {
        _player = this.transform.parent.gameObject;
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
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        _player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, _player.transform.up);
    }
}
