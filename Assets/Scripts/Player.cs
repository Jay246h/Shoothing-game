using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int score;
    public int life;
    public float speed;
    public int power;
    public int maxpower;
    public float bulletspeed;
    public float maxShotDelay;
    public float curShotDelay;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;
    public bool isHit;

    public GameObject bullotObjA;
    public GameObject bullotObjB;
    public GameManager manager;
    public GameObject boomEffect;

    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        move();
        fire();
        Reload();
    }
    void move()
    {
       float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || (Input.GetButtonUp("Horizontal"))){
            anim.SetInteger("Input", (int)h);
        }
    }
    void fire()
    {
        if (!Input.GetButton("Fire1"))
        {
            return;
        }
        if(curShotDelay < maxShotDelay)
        {
            return;
        }
        switch (power)
        {
            case 1: //1¹ß
                GameObject bullet = Instantiate(bullotObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*bulletspeed,ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = Instantiate(bullotObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                GameObject bulletL = Instantiate(bullotObjA, transform.position+ Vector3.left * 0.1f, transform.rotation);
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * bulletspeed, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * bulletspeed, ForceMode2D.Impulse);


                break;
            case 3:
                GameObject bulletRR = Instantiate(bullotObjA, transform.position + Vector3.right * 0.4f, transform.rotation);
                GameObject bulletCC = Instantiate(bullotObjB, transform.position, transform.rotation);
                GameObject bulletLL = Instantiate(bullotObjA, transform.position + Vector3.left * 0.4f, transform.rotation);
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * bulletspeed, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * bulletspeed, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * bulletspeed, ForceMode2D.Impulse);
                break;
        }


        curShotDelay = 0;
    }
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
               
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;
            isHit = true;
            life--;
            manager.UpdateLifeIcon(life);
            if(life == 0)
            {
                manager.gameOver();
            }
            else
            {
                manager.RespawnPlayer();
            }
            
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;

                

                case "Power":
                    if(power == maxpower)
                    {
                        score += 500;
                    }
                    else
                    {
                        power++;
                    }
                    break;
               case "Boom":
                    boomEffect.SetActive(true);
                    Invoke("OffBoomEffect", 4f);

                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    for(int index = 0; index < enemies.Length; index++)
                    {
                        Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
                        enemyLogic.OnHit(1000);
                    }
                    GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
                    for (int index = 0; index < bullets.Length; index++)
                    {
                        Destroy(bullets[index]);
                    }
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }

        }
    }
}
