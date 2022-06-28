/*
【概要】
VIVEコントローラで前後左右の移動と回転をするスクリプト

左手：タッチパッドの入力で『上下左右』を判定し↑←↓→の移動をする
右手：タッチパッドの入力で『左右』を判定し右回転と左回転をする
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerScript3_VR : MonoBehaviour
{
    public SteamVR_Input_Sources lefthandType;
    public SteamVR_Input_Sources righthandType;
    public SteamVR_Action_Boolean teleport;
    public SteamVR_Action_Vector2 direction;
    public Transform _VRCamera;
    public float _Movespeed = 3f;
    public float _Rotspeed = 5f;

    private SteamVR_Action_Boolean Trigger = SteamVR_Actions._default.InteractUI;

    public static Rigidbody PlayerRb { set; get; }

    const float PlayerSpeed = 20.0f;
    public static int PlayerVectMag;
    public static float Axel;
    [SerializeField] float MoveSpeed = 50f;

    public float Dir_y = 0;
    [SerializeField] GameObject gameManagerOb;
    ManagerScript manager;

    SteamVR_TrackedController Tracker;

    private void Start()
    {
        PlayerRb = gameObject.transform.root.GetComponent<Rigidbody>();

        PlayerVectMag = 0;
        Axel = 0.0f;

        manager = gameManagerOb.gameObject.GetComponent<ManagerScript>();

        Tracker = gameObject.GetComponent<SteamVR_TrackedController>();
    }

    void Update()
    {
        if (CheckGrabRight())
        {
            Move();

            if (Trigger.GetState(SteamVR_Input_Sources.RightHand))
            {
                HalfMove();
            }


        }
        else if (Trigger.GetState(SteamVR_Input_Sources.RightHand))
        {
            Brake();
        }
        else
        {
            Stop();
        }

        if (CheckGrabLeft())
        {
            Rotate();
        }

        Debug.Log(" Axel = " + Axel);
        Debug.Log(" PlayerRb.drag = " + PlayerRb.drag);
        manager.PlayerRbDrag = PlayerRb.drag;
        manager.Axel = Axel;

        Debug.Log("Tracker.Tracker1Posision.x :" + Tracker.Tracker1Posision.x);
        Debug.Log("Tracker.Tracker1Posision.y :" + Tracker.Tracker1Posision.y);

    }

    void FixedUpdate()
    {
        PlayerRb.AddForce(transform.root.forward * PlayerSpeed * Axel, ForceMode.Force);
        //PlayerRb.AddForce(Tracker.transform.root.forward * PlayerSpeed * Axel, ForceMode.Force);
        PlayerVectMag = Mathf.FloorToInt(PlayerRb.velocity.magnitude);

        if (Tracker.Tracker1Posision.x <= -0.5)
        {
            //左回転
            PlayerRb.transform.Rotate(Vector3.down * _Rotspeed * Time.deltaTime, Space.World);
        }
        else if (Tracker.Tracker1Posision.x >= 0.5)
        {
            //右回転
            PlayerRb.transform.Rotate(Vector3.up * _Rotspeed * Time.deltaTime, Space.World);
        }
    
        if (Tracker.Tracker1Posision.y <= -0.5)
        {
            //下回転
            PlayerRb.transform.Rotate(Vector3.right * _Rotspeed * Time.deltaTime, Space.Self);
        }
        else if (Tracker.Tracker1Posision.y >= 0.5)
        {
            //上回転
            PlayerRb.transform.Rotate(-Vector3.right * _Rotspeed * Time.deltaTime, Space.Self);
        }

        Debug.Log("Tracker.Tracker1Posision.x :" + Tracker.Tracker1Posision.x);
        Debug.Log("Tracker.Tracker1Posision.y :" + Tracker.Tracker1Posision.y);

    }

    private void Move()
    {
        Axel = MoveSpeed;
        PlayerRb.drag = 5;
        Debug.Log("PadOn");
    }

    private void HalfMove()
    {
        Axel = MoveSpeed / 2;
        PlayerRb.drag = 5;
        Debug.Log("HalfOn");
    }

    private void Stop()
    {
        Axel = 0f;
        PlayerRb.drag = 3;
        Debug.Log("StopNow");
    }

    private void Brake()
    {
        Axel = 0.0f;
        Debug.Log("TriggerOn");
    }

    private void Rotate()
    {
        // HMD(=カメラ)位置を中心として左右に回転する
        // 左右の判定はCheckDirectionRight()で計算する
        this.transform.RotateAround(_VRCamera.position, Vector3.up, _Rotspeed * CheckDirectionLeft());
    }

    private bool CheckGrabLeft()
    {
        return teleport.GetState(lefthandType);
    }
    private bool CheckGrabRight()
    {
        return teleport.GetState(righthandType);
    }

    private Vector3 CheckDirectionRight()
    {
        // タッチパッドのタッチ位置をVector2で取得
        Vector2 dir = direction.GetAxis(righthandType);
        Dir_y = dir.y;

        manager.Dir_y = Dir_y;

        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
        {
            // Y方向の絶対値の方が大きければ、HMD(=カメラ)に対して前か後ろ方向を返す
            return Mathf.Sign(dir.y) * Vector3.RotateTowards(new Vector3(0f, 0f, 1f), _VRCamera.forward, 360f, 360f);
        }
        else
        {
            // X方向の絶対値の方が大きければ、HMD(=カメラ)に対して右か左方向を返す
            return Mathf.Sign(dir.x) * Vector3.RotateTowards(new Vector3(1f, 0f, 0f), _VRCamera.right, 360f, 360f);
        }
    }


    private float CheckDirectionLeft()
    {
        // タッチパッドのタッチ位置をVector2で取得
        Vector2 dir = direction.GetAxis(lefthandType);
        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
        {
            // Y方向の絶対値の方が大きければ、回転量=0を返す
            return 0f;
        }
        else
        {
            // X方向の絶対値の方が大きければ、回転量= 1か -1を返す
            return Mathf.Sign(dir.x);
        }
    }
}