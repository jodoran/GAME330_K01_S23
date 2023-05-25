using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientGravity : MonoBehaviour
{
    public float _gravity;
    public int _ingredientNum;
    public GameObject Source;

    RectTransform _objectRectTransform;
    float _yValue = 0;

    private void Start()
    {
        _objectRectTransform = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.down * _gravity * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Bowl")
        {
            Source.GetComponent<NewAddingTask>().IngredientEnterBowl(gameObject);
        }
    }

}
