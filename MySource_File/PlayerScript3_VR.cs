using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerScript3_VR : MonoBehaviour
{
    // コントローラー操作関連の変数
    public SteamVR_Input_Sources lefthandType;
    public SteamVR_Input_Sources righthandType;
    public SteamVR_Action_Boolean teleport;
    public SteamVR_Action_Vector2 direction;
    public Transform _VRCamera;
    public float _Rotspeed = 5f;

    [SerializeField] Transform E;
    private Quaternion offsetRotation;
    // トリガー入力に使用する変数
    private SteamVR_Action_Boolean Trigger = SteamVR_Actions._default.InteractUI;

    public static Rigidbody CameraRb { set; get; }
    [SerializeField] Rigidbody PlayerRb;

    // プレイヤーのステータス関係の変数
    [SerializeField] float PlayerSpeed = 20.0f;
    public static int PlayerVectMag;
    public static float Axel;
    [SerializeField] float MoveSpeed = 15.0f;

    public float Dir_y = 0;
    [SerializeField] GameObject gameManagerOb;
    ManagerScript manager;

    // Tracker関連の変数
    [SerializeField] VelocityEstimator TrackerVe;
    float AbsoluVecX, AbsoluVecY, AbsoluVecZ;

    // コントローラー関連の変数
    [SerializeField] GameObject Controller;
    [SerializeField] VelocityEstimator ControllerVector;

    // 箒のオブジェクト
    [SerializeField] GameObject Broom;





    

    private void Start()
    {
        CameraRb = this.gameObject.transform.GetComponent<Rigidbody>();

        PlayerVectMag = 0;
        Axel = 0.0f;

        manager = gameManagerOb.gameObject.GetComponent<ManagerScript>();

        //Vector = GetComponent<VelocityEstimator>();

        offsetRotation = Quaternion.Inverse(E.transform.rotation) * transform.rotation;
    }

    void Update()
    {
        transform.rotation = E.transform.rotation * offsetRotation;

        // コントローラーの加速度を各変数に代入
        AbsoluVecX = ControllerVector.GetAngularVelocityEstimate().x;
        AbsoluVecY = ControllerVector.GetAngularVelocityEstimate().y;
        AbsoluVecZ = ControllerVector.GetAngularVelocityEstimate().z;

        // 加速度の値が負の場合、マイナスを掛けて絶対値にする
        if (AbsoluVecX < 0.0f)
        {
            AbsoluVecX *= -1;
        }
        if (AbsoluVecY < 0.0f)
        {
            AbsoluVecY *= -1;
        }
        if (AbsoluVecZ < 0.0f)
        {
            AbsoluVecZ *= -1;
        }

        Debug.Log(" AbsoluVecX = " + AbsoluVecX + " AbsoluVecY = " + AbsoluVecY + " AbsoluVecZ = " + AbsoluVecZ);

        if (AbsoluVecX >= 1.0f && AbsoluVecZ >= 1.0f)
        {
            Stop();
        }
        else
        {
            Move();
        }

    }

    void FixedUpdate()
    {
        //CameraRigを随時箒の方向に向けて移動、PlayerRigも同様に連動して移動

        CameraRb.AddForce(Broom.transform.up * PlayerSpeed * Axel, ForceMode.Force);
        //PlayerRb.AddForce(Broom.transform.up * PlayerSpeed * Axel, ForceMode.Force);

        PlayerVectMag = Mathf.FloorToInt(CameraRb.velocity.magnitude);

    }

    private void Move()
    {
        Axel = MoveSpeed;
        CameraRb.drag = 1;
        Debug.Log("PadOn");
    }

    private void Stop()
    {
        Axel = 0f;
        CameraRb.drag = 2;
        Debug.Log("StopNow");
    }

}