using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class NewAddingTask : MonoBehaviour, IDSTapListener
{
    public Slider ProgressBar;
    public Slider TaskProgressBar;
    public GameObject TaskManager;

    public AudioSource CollectSound;
    public AudioSource BadSound;
    public AudioSource FallSound;

    public RectTransform PadModeButton;
    public RectTransform CursorModeButton;
    public Image Bowl;
    public float _bowlBounds;

    public List<GameObject> IngredientCatalog = new List<GameObject>();

    public RectTransform SpawnBarTop;
    public RectTransform SpawnBarBottom;
    public RectTransform DespawnBarTop;
    public RectTransform DespawnBarBottom;

    List<GameObject> ActiveIngredients = new List<GameObject>();

    public float _movementSpeed;
    public int _totalIngredientsNeeded;
    public int _initSpawnCount = 3;
    public float _spawnDelay = 1;

    bool CursorMode = false;
    bool PadMode = true;
    float _bowlX = 0;
    float _spawnTimer;

    private void Start()
    {
        SpawnIngredient(SpawnBarTop);
        _initSpawnCount--;
    }

    public void Update()
    {
        if (PadMode)
        {
            float axisValue = Input.GetAxisRaw("Horizontal");
            bool rightInput = false;
            bool leftInput = false;
            rightInput = axisValue > 0.1f;
            leftInput = axisValue < -0.1f;

            if (Input.GetButton("Horizontal"))
            {
                if (rightInput && _bowlX < _bowlBounds)
                {
                    const float SNAP_SPEED = 24.0f;
                    _bowlX += _movementSpeed;
                    Vector2 currPosition = Bowl.rectTransform.anchoredPosition;
                    currPosition.x += (_bowlX - currPosition.x) * SNAP_SPEED * Time.deltaTime;
                    Bowl.rectTransform.anchoredPosition = currPosition;
                }
                if (leftInput && _bowlX > -_bowlBounds)
                {
                    const float SNAP_SPEED = 24.0f;
                    _bowlX -= _movementSpeed;
                    Vector2 currPosition = Bowl.rectTransform.anchoredPosition;
                    currPosition.x += (_bowlX - currPosition.x) * SNAP_SPEED * Time.deltaTime;
                    Bowl.rectTransform.anchoredPosition = currPosition;
                }
            }
        }

        if(_initSpawnCount > 0)
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer > _spawnDelay)
            {
                _initSpawnCount--;
                SpawnIngredient(SpawnBarTop);
                _spawnTimer = 0;
            }
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition) 
    {
        if (CursorMode)
        {
            MovePlateWithCursor(Bowl.GetComponent<RectTransform>(), tapPosition, 120);
        }
    }

    public void OnScreenDrag(Vector2 tapPosition)
    {
        if (CursorMode)
        {
            MovePlateWithCursor(Bowl.GetComponent<RectTransform>(), tapPosition, 120);
        }
    }

    public void OnScreenTapUp(Vector2 tapPosition) { }


    private void FixedUpdate()
    {
        if (ActiveIngredients.Count > 0)
        {
            CheckActiveIngredientsOverlap();
        }

        if(TaskProgressBar.value >= 1)
        {
            TaskManager.GetComponent<TaskManager>().TaskComplete = true;
        }
    }

    void CheckActiveIngredientsOverlap()
    {
        for (int i = 0; i < ActiveIngredients.Count; i++)
        {
            Transform _ingedient = ActiveIngredients[i].GetComponent<Transform>();
            if (_ingedient.localPosition.y <= -110 && _ingedient.parent == DespawnBarTop.parent)
            {
                Destroy(ActiveIngredients[i]);
                ActiveIngredients.RemoveAt(i);
                RespawnIngredient(SpawnBarBottom, _ingedient);
            }
            else if (_ingedient.localPosition.y <= -110 && _ingedient.parent == DespawnBarBottom.parent)
            {
                if (_ingedient.GetComponent<IngredientGravity>().GoodOrBad) {
                    FallSound.Play();
                }

                Destroy(ActiveIngredients[i]);
                ActiveIngredients.RemoveAt(i);
                SpawnIngredient(SpawnBarTop);
            }
        }
    }

    void SpawnIngredient(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _ingredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[_ingredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 0), Quaternion.identity, Bar.parent);
        _spawnObject.GetComponent<IngredientGravity>()._ingredientNum = _ingredientNum;

        ActiveIngredients.Add(_spawnObject);
    }

    void RespawnIngredient(RectTransform Bar, Transform Ingredient)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = Ingredient.localPosition.x;
        int _ingredientNum = Ingredient.GetComponent<IngredientGravity>()._ingredientNum;

        GameObject _ingredient = IngredientCatalog[_ingredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 1), Quaternion.identity, Bar.parent);
        _spawnObject.transform.localPosition = new Vector3(x, _spawnObject.transform.localPosition.y, 1);
        _spawnObject.GetComponent<IngredientGravity>().Source = gameObject;

        ActiveIngredients.Add(_spawnObject);
    }

    public void IngredientEnterBowl(GameObject _ingredient, bool GoodORBad)
    {
        Destroy(_ingredient);
        if (GoodORBad)
        {
            CollectSound.Play();
            TaskProgressBar.value += 1f / _totalIngredientsNeeded;
            if (TaskManager.GetComponent<TaskManager>().HealingEnabled)
            {
                ProgressBar.value += 0.05f;
            }
        }
        else
        {
            ProgressBar.value -= 0.05f;
            BadSound.Play();
        }
        ActiveIngredients.Remove(_ingredient);
        SpawnIngredient(SpawnBarTop);

    }

    void MovePlateWithCursor(RectTransform plate, Vector3 cursorPosition, float Xoffset) //Sets plate to position of the cursor
    {
        // Gets Mouse position
        Vector3 currPosition = plate.anchoredPosition;
        currPosition.x += (cursorPosition.x - (currPosition.x + Xoffset));

        // Sets plate rect transform to that position
        plate.anchoredPosition = currPosition;
    }
}
