using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Move Setting")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Camera Setting")]
    public float sensitivity = 100.0f; // 滑鼠靈敏度
    public float maxYAngle = 80.0f;

    [Header("Wall Setting")] 
    public LayerMask wallLayer;

    [Header("Hunt List")]
    public List<EnemyAction> huntList = new List<EnemyAction>();

    public Animator weaponAnimator;

    public GameObject bloodEffect;

    private bool isGrounded;
    private bool isWallLock;

    private Rigidbody rig;
    private Vector2 currentRotation;


    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        CameraController();
        PlayerMovement();

        WallSlide();

        SortHuntList();

        if (Input.GetMouseButtonDown(1))
        {
            ScreenEffect.instance.LensDistortionFadeIn();

            DOTween.To(() => transform.position, x => transform.position = x, huntList[0].GetAttackPosition().position,
                0.2f).SetEase(Ease.Linear).onComplete += delegate
            {
                Destroy(Instantiate(bloodEffect, transform.position, bloodEffect.transform.rotation) , 5);

                var g = huntList[0].gameObject;
                
                RemoveHuntList(huntList[0]);
                Destroy(g);
                
                ScreenEffect.instance.CameraShake();
                weaponAnimator.Play("Attack");
                
                rig.velocity = Vector3.zero;
                rig.AddForce(Vector3.up * 10 , ForceMode.Impulse);
            };
            
            Camera.main.transform.LookAt(huntList[0].GetAttackPosition());
            
            
        }

        
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void WallSlide()
    {
        var rightSide = Physics.CheckSphere(transform.position + transform.right * 0.2f, 0.2f , wallLayer);
        var leftSide = Physics.CheckSphere(transform.position + -transform.right * 0.2f, 0.2f , wallLayer);

        if ((rightSide || leftSide) && !isWallLock)
        {
            var value = rightSide ? 10 : -10;
            
            Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation,Quaternion.Euler(new Vector3(0 , 0 , value)) , 0.1f);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rig.AddForce(transform.right * 20 * -value, ForceMode.Force);
                rig.AddForce(Vector3.up * 5 , ForceMode.Impulse);

                StartCoroutine(StartTickWallCheck());
                
                isWallLock = true;
            }

            rig.velocity = new Vector3(rig.velocity.x , -0.15f , rig.velocity.z);
            rig.useGravity = false;
        }
        else
        {
            rig.useGravity = true;
            Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation,Quaternion.Euler(new Vector3(0 , 0 , 0)) , 0.1f);
        }
        
    }

    private void CameraController()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // 計算鏡頭的旋轉角度
        currentRotation.x += mouseX;
        currentRotation.y -= mouseY;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        // 將旋轉應用於鏡頭
        transform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0.0f);
    }

    private void PlayerMovement()
    {
        if(isWallLock)
            return;
        
        // 玩家移動
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        move *= moveSpeed;
        move.y = rig.velocity.y;
        rig.velocity = move;

        // 玩家跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawWireSphere( transform.position + transform.right * 0.2f , 0.05f);
        Gizmos.DrawWireSphere(transform.position + -transform.right * 0.2f , 0.05f);
    }

    public void AddHuntList(EnemyAction target)
    {
        var targetCheck = huntList.Contains(target);

        if (!targetCheck)
        {
            huntList.Add(target);
        }
    }

    public void RemoveHuntList(EnemyAction target)
    {
        huntList.Remove(target);
    }

    public void SortHuntList()
    {
        if(huntList.Count == 0)
            return;

        List<float> enemyDistance = new List<float>();

        foreach (var target in huntList)
        {
            var dis = Vector3.Distance(transform.position, target.transform.position);
            
            enemyDistance.Add(dis);
        }

        var n = enemyDistance.Count;
        
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (enemyDistance[j] > enemyDistance[j + 1])
                {
                    (enemyDistance[j], enemyDistance[j + 1]) = (enemyDistance[j + 1], enemyDistance[j]);
                    (huntList[j], huntList[j + 1]) = (huntList[j + 1], huntList[j]);
                }
            }
        }
    }

    private IEnumerator StartTickWallCheck()
    {
        yield return new WaitForSeconds(0.2f);
        isWallLock = false;
        yield return null;
    }
}