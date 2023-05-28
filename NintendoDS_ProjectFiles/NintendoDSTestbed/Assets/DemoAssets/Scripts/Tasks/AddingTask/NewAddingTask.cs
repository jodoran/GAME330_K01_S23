using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class NewAddingTask : MonoBehaviour
{
    public Slider ProgressBar;
    public Slider TaskProgressBar;
    public GameObject TaskManager;
    public Image Bowl;
    public float _bowlBounds;

    public List<GameObject> IngredientCatalog = new List<GameObject>();

    public RectTransform SpawnBarTop;
    public RectTransform SpawnBarBottom;
    public RectTransform DespawnBarTop;
    public RectTransform DespawnBarBottom;

    List<GameObject> ActiveIngredients = new List<GameObject>();

    public int _totalIngredientsNeeded;
    public int _initSpawnCount = 3;
    public float _spawnDelay = 1;

    float _bowlX = 0;
    float _spawnTimer;

    private void Start()
    {
        SpawnIngredient(SpawnBarTop);
        _initSpawnCount--;
    }

    public void Update()
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
                _bowlX++;
                Vector2 currPosition = Bowl.rectTransform.anchoredPosition;
                currPosition.x += (_bowlX - currPosition.x) * SNAP_SPEED * Time.deltaTime;
                Bowl.rectTransform.anchoredPosition = currPosition;
            }
            if (leftInput && _bowlX > -_bowlBounds)
            {
                const float SNAP_SPEED = 24.0f;
                _bowlX--;
                Vector2 currPosition = Bowl.rectTransform.anchoredPosition;
                currPosition.x += (_bowlX - currPosition.x) * SNAP_SPEED * Time.deltaTime;
                Bowl.rectTransform.anchoredPosition = currPosition;
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
                Destroy(ActiveIngredients[i]);
                ActiveIngredients.RemoveAt(i);
                SpawnIngredient(SpawnBarTop);
                ProgressBar.value -= 0.05f;
            }
        }
    }

    void SpawnIngredient(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _indgredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[_indgredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 0), Quaternion.identity, Bar.parent);

        ActiveIngredients.Add(_spawnObject);
    }

    void RespawnIngredient(RectTransform Bar, Transform Ingredient)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = Ingredient.localPosition.x;
        int _indgredientNum = Ingredient.GetComponent<IngredientGravity>()._ingredientNum;

        GameObject _ingredient = IngredientCatalog[_indgredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 1), Quaternion.identity, Bar.parent);
        _spawnObject.transform.localPosition = new Vector3(x, _spawnObject.transform.localPosition.y, 1);
        _spawnObject.GetComponent<IngredientGravity>().Source = gameObject;

        ActiveIngredients.Add(_spawnObject);
    }

    public void IngredientEnterBowl(GameObject _ingredient)
    {
        Destroy(_ingredient);
        ActiveIngredients.Remove(_ingredient);
        SpawnIngredient(SpawnBarTop);
        TaskProgressBar.value += 1f/_totalIngredientsNeeded;
    }

    bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2) //Gets if the 1st RectTransform is overlapping the 2nd RectTransform
    {
        // recreates both RectTransforms poitions current position and size
        Rect rect1 = new Rect(rectTrans1.anchoredPosition.x, rectTrans1.anchoredPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.anchoredPosition.x, rectTrans2.anchoredPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        // uses the recreated RectTransforms positions and sizes to check if they are overlapping
        return rect1.Overlaps(rect2);
    }
}
