using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerVRScript : MonoBehaviour
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
    private Boolean TriggerOn;

    private SteamVR_Action_Boolean Grab = SteamVR_Actions.default_GrabGrip;
    private Boolean GrabGrip;

    // 今回使用する各リジットボディー
    public static Rigidbody CameraRb { set; get; }
    [SerializeField] Rigidbody PlayerRb;

    // プレイヤーのステータス関係の変数
    [SerializeField] float PlayerSpeed = 20.0f;
    public static int PlayerVectMag;
    public static float Axel;
    [SerializeField] float MoveSpeed = 10.0f;
    [SerializeField] float UpSpeed = 10.0f;

    //　ステータス用
    public float Dir_y = 0;
    [SerializeField] ManagerScript manager;

    // Tracker関連の変数
    [SerializeField] VelocityEstimator TrackerVe;
    float AbsoluVecX, AbsoluVecY, AbsoluVecZ;

    // コントローラー関連の変数
    [SerializeField] GameObject Controller;
    [SerializeField] VelocityEstimator ControllerVector;

    // 箒のオブジェクト
    [SerializeField] GameObject Broom;


    //Pause画面入る時に処理を止めるのに必要な宣言
    [SerializeField] MainUIManagerScript mainUIs;


    // 以下キー入力用変数
    // 上下左右各キーの入力情報を格納する変数
    private float v;
    private float h;

    // 回転時にしようする四次元の変数
    private Quaternion Rota_right;
    private Quaternion Rota_left;
    private Quaternion Rota_up;
    private Quaternion Rota_down;
    private Quaternion Rota_p;

    // 回転時にしようするステータスの変数
    public static float RotateAngle;
    public static float RotateSpeed;

    public bool brake_on { get; set; } = false;

    [SerializeField] float AnglSpeed = 20;
    
    [SerializeField] GameObject gameMnagerOb;
    [SerializeField] GameObject camera = null;
    //ItemManageScript itemManager;

    Vector3 Vec;
    [SerializeField] GameObject VecPos;

    float AccelerationValue = 1.5f;
    bool StopOn;

    private void Start()
    {
        CameraRb = this.gameObject.transform.GetComponent<Rigidbody>();

        PlayerVectMag = 0;
        Axel = 0.0f;

        //manager = gameManagerOb.gameObject.GetComponent<ManagerScript>();

        offsetRotation = Quaternion.Inverse(E.transform.rotation) * transform.rotation;

        //itemManager = gameMnagerOb.GetComponent<ItemManageScript>();
        PlayerRb = GetComponent<Rigidbody>();

        // 各ステータスをセットアップ
        RotateAngle = 45f;
        RotateSpeed = 45f;
        PlayerVectMag = 0;
        Axel = 0.0f;
        if (manager.VR_mode) MoveSpeed = 5.0f;
        else                 MoveSpeed = 5.0f;

        Vector3 worldAngle = transform.eulerAngles;
        worldAngle.x = AnglSpeed;
        worldAngle.y = AnglSpeed;
        worldAngle.z = AnglSpeed;

        StopOn = false;

    }

    void Update()
    {

        transform.rotation = E.transform.rotation * offsetRotation;

        // VRモードオンの場合
        if (manager.VR_mode)
        {

            // コントローラーの加速度を各変数に代入
            AbsoluVecX = ControllerVector.GetAngularVelocityEstimate().x;
            AbsoluVecY = ControllerVector.GetAngularVelocityEstimate().y;
            AbsoluVecZ = ControllerVector.GetAngularVelocityEstimate().z;

            Vec = VecPos.transform.position - this.transform.position;

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

            //Debug.Log(" AbsoluVecX = " + AbsoluVecX + " AbsoluVecY = " + AbsoluVecY + " AbsoluVecZ = " + AbsoluVecZ);

            // プレイヤーが箒を振った際、XとZの加速度がVecValue以上の数値になった場合
            if ((AbsoluVecX >= AccelerationValue && AbsoluVecZ >= AccelerationValue) /*|| mainUIs.OnPause == true */ )
            {

                // 更にその際、箒の傾きが1.5fより上であった場合
                if (Vec.y > 1.5f)
                {
                    StopOn = true;
                }

            }
            else if (StopOn == true && Vec.y <= 1.5f)
            {
                // 止まった状態で箒の傾きが1.5f以下であった場合
                StopOn = false;
            }

            if (StopOn == true || mainUIs.OnPause == true || manager.brake_event == true)
            {
                // 停止用関数処理へ移行
                Stop();
            }
            else
            {
                // 移動用関数処理へ移動
                Move();
            }

            TriggerOn = Trigger.GetState(SteamVR_Input_Sources.RightHand);

            if (TriggerOn)
            {
                Debug.Log("TriggerOn");
            }

            GrabGrip = Grab.GetState(SteamVR_Input_Sources.RightHand);

            if (GrabGrip)
            {
                Debug.Log("GrabGrip");
            }

        }
        else 
        {

            // 各入力情報を更新
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

            // 各四次元の変数を更新
            Rota_right = Quaternion.AngleAxis(2, Vector3.up * h);
            Rota_left = Quaternion.AngleAxis(2, -Vector3.up * h);
            Rota_up = Quaternion.AngleAxis(2, Vector3.left * v);
            Rota_down = Quaternion.AngleAxis(2, -Vector3.left * v);
            Rota_p = transform.rotation;

            RotCamera();

            if (manager.autoAccel_mode == true && manager.rise_flag == false)
            {
                Axel = MoveSpeed;
            }
            else if (!manager.autoAccel_mode && manager.rise_flag == false)
            {
                // Wキー(前進)関連の処理
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Axel = MoveSpeed;
                    PlayerRb.drag = 5;
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    Axel = 0f;
                    PlayerRb.drag = 3;
                }
            }


            // Sキー(ブレーキ)関連の処理
            if (Input.GetKey(KeyCode.M))
            {
                brake_on = true;
                //Axel = 0.0f;
            }
            else /*if (Input.GetKeyUp(KeyCode.KeypadEnter))*/
            {
                brake_on = false;
                //PlayerRb.drag = 3;
            }

            // WキーとSキーの入力が同時に行われた場合の処理
            if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.KeypadEnter))
            {
                Axel = MoveSpeed / 2;
                PlayerRb.drag = 5;
            }

            // 左回りの処理
            if (Input.GetKey(KeyCode.J))
            {
                transform.Rotate(Vector3.down * RotateSpeed * Time.deltaTime, Space.World);
            }

            // 右回りの処理
            if (Input.GetKey(KeyCode.L))
            {
                transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime, Space.World);
            }

            // 目線を上に上げる
            if (Input.GetKey(KeyCode.K))
            {
                transform.Rotate(Vector3.right * RotateAngle * Time.deltaTime, Space.Self);
            }

            // 目線を下に下げる
            if (Input.GetKey(KeyCode.I))
            {
                transform.Rotate(-Vector3.right * RotateAngle * Time.deltaTime, Space.Self);
            }


            // 変更情報をプレイヤーに反映
            // PlayerRb.AddForce(transform.forward * PlayerSpeed * Axel, ForceMode.Force);
            // PlayerVectMag = Mathf.FloorToInt(PlayerRb.velocity.magnitude);

        }

        //Debug.Log("Controller.x:" + Controller.transform.rotation.x);
        //Debug.Log("Controller.y:" + Controller.transform.rotation.y);
        //Debug.Log("Controller.z:" + Controller.transform.rotation.z);

        //Debug.Log("ベク：" + Vec);

    }

    void FixedUpdate()
    {
        //CameraRigを随時箒の方向に向けて移動、PlayerRigも同様に連動して移動
        //if (mainUIs.OnPause == false)
        //{
            CameraRb.AddForce(Broom.transform.forward * PlayerSpeed * Axel, ForceMode.Force);
            PlayerRb.AddForce(Broom.transform.forward * PlayerSpeed * Axel, ForceMode.Force);
            PlayerVectMag = Mathf.FloorToInt(CameraRb.velocity.magnitude);
        //}
        //else
        //{
            //CameraRb.AddForce(Broom.transform.forward * PlayerSpeed * 0, ForceMode.Force);
            //PlayerRb.AddForce(Broom.transform.forward * PlayerSpeed * 0, ForceMode.Force);
        //}
        //Debug.Log(PlayerRb.velocity);
    }

    private void Move()
    {
        Axel = MoveSpeed;
        CameraRb.drag = 1;
        Debug.Log("Move");
    }

    private void Stop()
    {
        Axel = 0f;
        CameraRb.drag = 2;
        Debug.Log("StopNow");
    }

    private void SpeedUp()
    {
        Axel = UpSpeed;
        CameraRb.drag = 1;
        Debug.Log("SpeedUp");
    }

    //視線だけ操作
    void RotCamera()
    {

        if (Input.GetAxis("Horizontal") != 0)
        {
            camera.transform.Rotate(Vector3.up * h * RotateSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            camera.transform.Rotate(-Vector3.right * v * RotateAngle * Time.deltaTime, Space.Self);
        }

        //カメラリセット
        if (Input.GetKey(KeyCode.R))
        {
            camera.transform.rotation = transform.rotation;
        }
    }

}