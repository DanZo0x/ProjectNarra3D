using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPhone : MonoBehaviour
{
    bool _isOpenning = false;
    bool _isClosing = false;

    [SerializeField] Camera _camera;
    Vector3 _originPosition;
    Quaternion _originRotation;

    [SerializeField] Vector3 _goalZoomPosition; //x = 0, y = 2.45, z = 2.37
    [SerializeField] Quaternion _goalZoomRotation; // x = 45, y = 0, z = 0
    [SerializeField] Vector3 _goalDialoguePhonePosition;
    [SerializeField] Quaternion _goalDialoguePhoneRotation;
    [SerializeField] float _time;
    [SerializeField] Transform _panelTouches;
    [SerializeField] AnimationCurve _curveZoom;
    Transform telephone;

    [HideInInspector] public Transform zoomButton;
    
    Renderer phoneBaseRend;
    
    private void Awake()
    {
        _originPosition = _camera.transform.position;
        _originRotation = _camera.transform.rotation;
        telephone = transform.parent.parent;
        _panelTouches = transform.Find("Panel");
        phoneBaseRend = telephone.Find("Telephone_base_base").GetComponent<MeshRenderer>();
        phoneBaseRend.materials[1].SetFloat("_Scale", 1.05f);
        OutlineAllButtons(false);
        zoomButton = transform.Find("PhoneZoomButton");
        zoomButton.gameObject.SetActive(true);
    }

    public void ZoomOnPhone()
    {
        phoneBaseRend.materials[1].SetFloat("_Scale", 0);
        StartCoroutine(ZoomPhoneCoroutine(_goalZoomPosition, _goalZoomRotation));
        _panelTouches.gameObject.SetActive(true);
        OutlineAllButtons(true);

    }

    public IEnumerator ZoomPhoneCoroutine(Vector3 _goalZoomPosition, Quaternion _goalZoomRotation)
    {
        float timer = 0;
        while(timer < _time)
        {
            float percentage = timer / _time;
            _camera.transform.SetPositionAndRotation(Vector3.Lerp(_originPosition, _goalZoomPosition, _curveZoom.Evaluate(percentage)), Quaternion.Lerp(_originRotation, _goalZoomRotation, _curveZoom.Evaluate(percentage)));
            timer += Time.deltaTime;
            yield return null;
        }
        
        
    }

    public IEnumerator DezoomPhoneCoroutine()
    {
        Vector3 currentPosition = _camera.transform.position;
        Quaternion currentRotation = _camera.transform.rotation;
        float timer = 0;
        while (timer < _time)
        {
            float percentage = timer / _time;
            _camera.transform.SetPositionAndRotation(Vector3.Lerp(currentPosition, _originPosition, _curveZoom.Evaluate(percentage)), Quaternion.Lerp(currentRotation, _originRotation, _curveZoom.Evaluate(percentage)));
            timer += Time.deltaTime;
            yield return null;
        }
        phoneBaseRend.materials[1].SetFloat("_Scale", 1.05f);
    }

    public void OutlineAllButtons(bool value)
    {
        float valueScale = 0;
        if(value)
        {
            valueScale = 1.12f;
        }
        foreach(Transform button in telephone.Find("JENPEUXPLUSLESTOUCHES"))
        {

            button.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", valueScale);
        }
    }

    
}
