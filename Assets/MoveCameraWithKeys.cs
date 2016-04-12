using UnityEngine;
using System.Collections;

public class MoveCameraWithKeys : MonoBehaviour
{
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;

    public KeyCode zoomInKey = KeyCode.Period;
    public KeyCode zoomOutKey = KeyCode.Comma;

    public Camera _camera = null;

    void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
    }//Start

    void Update()
    {
        if(Input.GetKey(upKey))
        {
            transform.position = transform.position + new Vector3(0,1,0);
        }//if
        else if (Input.GetKey(downKey))
        {
            transform.position = transform.position + new Vector3(0, -1, 0);
        }//else if

        if (Input.GetKey(leftKey))
        {
            transform.position = transform.position + new Vector3(-1, 0, 0);
        }//if
        else if (Input.GetKey(rightKey))
        {
            transform.position = transform.position + new Vector3(1, 0, 0);
        }//else if

        if (_camera != null)
        {
            if (Input.GetKey(zoomInKey))
            {
                if (_camera.orthographic)
                    _camera.orthographicSize -= 0.1f;
                else
                    _camera.transform.position = _camera.transform.position + new Vector3(0,0,0.5f);
            }//if
            else if (Input.GetKey(zoomOutKey))
            {
                if (_camera.orthographic)
                    _camera.orthographicSize += 0.1f;
                else
                    _camera.transform.position = _camera.transform.position + new Vector3(0, 0, -0.5f);
            }//else if
        }//if
    }//Update

}//MoveCameraWithKeys
