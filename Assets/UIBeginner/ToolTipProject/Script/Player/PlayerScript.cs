using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Camera camera;
    [SerializeField] private MainView mainView;
    private void Start() 
    {
        camera = Camera.main;
        mainView.InstantiateAndHandleHealthBar(transform, camera);
    }

    private void FixedUpdate() 
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        transform.position += new Vector3(horizontal, 0, vertical) * Time.deltaTime * 5f;
    }
}
