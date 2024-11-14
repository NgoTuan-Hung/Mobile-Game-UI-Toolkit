using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Camera camera;
    CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private MainView mainView;
    GameUIManager gameUIManager;
    GameObject cameraToPlayer;
    Vector2 inverseY = new Vector2(1, -1);
    [SerializeField] private float swipeSpeedY = 0.1f;
    [SerializeField] private float swipeSpeedX = 0.1f;
    [SerializeField] private float moveSpeed = 0.1f;
    private void Start() 
    {
        camera = Camera.main;
        cinemachineFreeLook = FindAnyObjectByType<CinemachineFreeLook>();
        mainView.InstantiateAndHandleHealthBar(transform, camera);
        gameUIManager = FindAnyObjectByType<GameUIManager>();
        cameraToPlayer = transform.Find("CameraToPlayer").gameObject;
        gameUIManager.MainView.joyStickMoveEvent += (vector) => 
        {
            vector.Scale(inverseY);
            Move(vector);
        };

        gameUIManager.mainViewSwipeDelegate += (vector) => 
        {
            cinemachineFreeLook.m_YAxis.Value -= vector.y * swipeSpeedY;
            cinemachineFreeLook.m_XAxis.Value -= vector.x * swipeSpeedX;
        };
    }

    void Move(Vector2 direction)
    {
        transform.position += cameraToPlayer.transform.TransformDirection(new Vector3(direction.x, 0, direction.y)) * Time.deltaTime * moveSpeed;
    }

    private void FixedUpdate() 
    {
        cameraToPlayer.transform.rotation = Quaternion.LookRotation
        (
            new Vector3(transform.position.x - camera.transform.position.x, 0, transform.position.z - camera.transform.position.z),
            Vector3.up
        );
    }
}
