using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewHeight;

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }
    void Update()
    {
        Move();
        Scrolling();


    }
    void Move()
    {
        Vector3 curpos = transform.position;
        Vector3 nextpos = Vector3.down * speed * Time.deltaTime;
        transform.position = curpos + nextpos;

    }
    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1))
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = startIndexSave - 1 == -1 ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
