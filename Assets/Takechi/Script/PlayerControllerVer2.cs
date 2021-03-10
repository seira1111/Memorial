using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerVer2 : MonoBehaviour
{
    [SerializeField]
    PlayerParameter playerParameter;
    private Vector3 baseAngle;   //初期角度
    private float jumpPower = 0.0f;
    private bool isJumpRise = false;    //上昇中ジャンプフラグ
    private float jumpTimeCounter;  //ジャンプカウント
    private bool isGround = false;   //地面フラグ
    private float Direction = 0.0f; //方向
    private Rigidbody rb;
    private LayerMask layerMask;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        baseAngle = transform.eulerAngles;

        rb.constraints =
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // カーソルキーの入力を取得
        //右移動
        if (Input.GetKey(KeyCode.D))
        {
            Direction = 1.0f;

        }
        //左移動
        else if (Input.GetKey(KeyCode.A))
        {
            Direction = -1.0f;
        }
        //押してないとき
        else
        {
            Direction = 0.0f;
        }

        //向きを変える
        if (Direction > 0.0f)
        {
            //右向き
            transform.eulerAngles = baseAngle;
        }
        else if (Direction < 0.0f)
        {
            //左向き
            transform.eulerAngles = new Vector3(0.0f, baseAngle.y + 180.0f, 0.0f);
        }

        //地面にいるとき
        if (isGround)
        {
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpTimeCounter = playerParameter.JumpTime;
                isJumpRise = true;
                isGround = false;
                jumpPower = playerParameter.BaseJumpPower;
            }

            //横移動
            rb.velocity = new Vector3(Direction * playerParameter.MoveSpeed , rb.velocity.y, 0.0f);

        }
        //地面にいない
        else
        {
            //下降中
            if (!isJumpRise)
            {
                //重力の値設定
                rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.y * playerParameter.GravityPower, 0.0f);
            }
            //空中時の横移動
            rb.velocity = new Vector3(Direction * playerParameter.JumpMoveSpeed, rb.velocity.y, 0.0f);
        }
        //押してる間上昇
        if (Input.GetKey(KeyCode.Space) && isJumpRise)
        {
            //上昇時間内
            if (jumpTimeCounter > 0)
            {
                //上昇
                rb.velocity = new Vector3(rb.velocity.x, 1.0f * jumpPower, 0.0f);
                jumpTimeCounter -= Time.deltaTime;
            }
            //上昇時間外
            else
            {
                isJumpRise = false;
            }
        }
        //離した場合上昇を中止
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpRise = false;

        }

    }


    //入った時
    private void OnCollisionEnter(Collision collision)
    {

        if (1 << collision.gameObject.layer != layerMask)
        {
            //真ん中
            if (Physics.Linecast(transform.position + Vector3.down * 0.8f, transform.position + Vector3.down * 1.2f))
            {
                isGround = true;
                return;
            }
            //始点
            var start = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.4f, 0.0f);
            //終点
            var end = new Vector3(start.x, start.y - 0.8f, 0.0f);
            //右
            if (Physics.Linecast(start, end))
            {
                isGround = true;
                return;
            }
            //始点
            start = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.4f, 0.0f);
            //終点
            end = new Vector3(start.x, start.y - 0.8f, 0.0f);
            //左
            if (Physics.Linecast(start, end))
            {
                isGround = true;
                return;
            }
        }

    }
    //入っている間
    private void OnCollisionStay(Collision collision)
    {
        //プレイヤーレイヤ以外
        if (1 << collision.gameObject.layer != layerMask)
        {//上昇中の時
            if (isJumpRise)
            {
                //真ん中
                if (Physics.Linecast(transform.position + Vector3.down * 0.8f, transform.position + Vector3.down * 1.2f))
                {
                    isGround = true;
                    //Debug.Log("真ん中");
                    return;
                }
                var start = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.4f, 0.0f);
                var end = new Vector3(start.x, start.y - 0.8f, 0.0f);
                //右
                if (Physics.Linecast(start, end))
                {
                    isGround = true;
                    return;
                }
                start = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.4f, 0.0f);
                end = new Vector3(start.x, start.y - 0.8f, 0.0f);
                //左
                if (Physics.Linecast(start, end))
                {
                    isGround = true;
                    return;
                }
            }
        }
    }
    //出たとき
    private void OnCollisionExit(Collision collision)
    {
        //　地面から降りた時の処理
        //　Fieldレイヤーのゲームオブジェクトから離れた時
        if (1 << collision.gameObject.layer != layerMask)
        {
            //下向きにレイヤーを飛ばしFieldレイヤーと接触しなければ地面から離れたと判定する
            if (!Physics.Linecast(transform.position + Vector3.down * 0.8f, transform.position + Vector3.down * 1.1f))
            {
                isGround = false;
            }

        }
    }

}
