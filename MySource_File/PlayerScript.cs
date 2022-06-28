using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
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
    const float PlayerSpeed = 20.0f;
    public static Rigidbody PlayerRb;
    public static float RotateAngle;
    public static float RotateSpeed;
    public static int PlayerVectMag;
    public static float Axel;

    public bool brake_on { get; set; } = false;

    [SerializeField] float AnglSpeed = 20;
    [SerializeField] float MoveSpeed = 50;

    [SerializeField] GameObject camera = null;
    [SerializeField] GameObject gameMnagerOb;
    //ItemManageScript itemManager;
    ManagerScript manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameMnagerOb.GetComponent<ManagerScript>();
        //itemManager = gameMnagerOb.GetComponent<ItemManageScript>();
        PlayerRb = GetComponent<Rigidbody>();

        //camera = transform.Find("Main Camera").gameObject;

        // 各ステータスをセットアップ
        RotateAngle = 45f;
        RotateSpeed = 45f;
        PlayerVectMag = 0;
        Axel = 0.0f;

        Vector3 worldAngle = transform.eulerAngles;
        worldAngle.x = AnglSpeed;
        worldAngle.y = AnglSpeed;
        worldAngle.z = AnglSpeed;
    }

    // Update is called once per frame
    void Update()
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
            //transform.rotation = rota_p * rota_left;
            transform.Rotate(Vector3.down * RotateSpeed * Time.deltaTime, Space.World);
        }

        // 右回りの処理
        if (Input.GetKey(KeyCode.L))
        {
            //transform.rotation = rota_p * rota_right;
            transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime, Space.World);
        }

        // 目線を上に上げる
        if (Input.GetKey(KeyCode.K))
        {
            // transform.rotation = rota_p * rota_up;
            transform.Rotate(Vector3.right * RotateAngle * Time.deltaTime, Space.Self);
        }

        // 目線を下に下げる
        if (Input.GetKey(KeyCode.I))
        {
            // transform.rotation = rota_p * rota_down;
            transform.Rotate(-Vector3.right * RotateAngle * Time.deltaTime, Space.Self);
        }


        // 変更情報をプレイヤーに反映
        // PlayerRb.AddForce(transform.forward * PlayerSpeed * Axel, ForceMode.Force);
        // PlayerVectMag = Mathf.FloorToInt(PlayerRb.velocity.magnitude);

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

    void FixedUpdate()
    {
        PlayerRb.AddForce(transform.forward * PlayerSpeed * Axel, ForceMode.Force);
        PlayerVectMag = Mathf.FloorToInt(PlayerRb.velocity.magnitude);
    }
}