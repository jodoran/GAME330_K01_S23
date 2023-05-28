using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class IngredientData : MonoBehaviour
{
    public Sprite HalfHealthSprite;
    public GameObject SplitParticle;

    public float _gravity;
    public int _ingredientNum;
    public GameObject Source;

    public string _letterKey;
    public string _arrowKey;
    public int _health = 2;

    Image _arrowSign;
    Image _letterSign;
    float _yValue = 0;

    private void Start()
    {
        _arrowSign = Source.GetComponent<CuttingTask>().ArrowSign;
        _letterSign = Source.GetComponent<CuttingTask>().LetterSign;
    }

    public void SetValues(string LetterKey, string ArrowKey, int Health, int IngredientNum)
    {
        _letterKey = LetterKey;
        _arrowKey = ArrowKey;
        _health = Health;
        _ingredientNum = IngredientNum;
    }

    public void NewIngredient()
    {
        SetRandomKeys();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _gravity * Time.deltaTime, Space.Self);

        if(_health == 1)
        {
            this.GetComponent<SpriteRenderer>().sprite = HalfHealthSprite;
        }
        else if (_health == 0)
        {
            Instantiate(SplitParticle, new Vector3(transform.position.x, transform.position.y, 11f), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void SetRandomKeys()
    {
        _letterKey = GetKey("A", "B", "X", "Y");
        _arrowKey = GetKey("RIGHT", "DOWN", "LEFT", "UP");
        Sprite _arrowSprite = null;
        Sprite _letterSprite = null;

        Sprite[] all = Resources.LoadAll<Sprite>("Tasks/CuttingTask/arrowButtons");
        foreach (var s in all)
        {
            if (s.name == _arrowKey)
            {
                _letterSprite = s;
            }
        }

        all = Resources.LoadAll<Sprite>("Tasks/CuttingTask/keyButtons");
        foreach (var s in all)
        {
            if (s.name == _letterKey)
            {
                _arrowSprite = s;
            }
        }
        Source.GetComponent<CuttingTask>().AddIngredientKey(_ingredientNum, _letterSprite, _arrowSprite, gameObject, _health);
    }

    string GetKey(string option1, string option2, string option3, string option4)
    {
        int x = UnityEngine.Random.Range(0, 3);
        switch (x)
        {
            case 0:
                return option1;
                break;
            case 1:
                return option2;
                break;
            case 2:
                return option3;
                break;
            case 3:
                return option4;
                break;
        }
        return null;
    }

}
