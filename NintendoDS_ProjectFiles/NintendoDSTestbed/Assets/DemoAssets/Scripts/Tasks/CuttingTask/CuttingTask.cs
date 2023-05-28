using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

[Serializable]
struct Keys{
    public Sprite Arrow;
    public Sprite Letter;
    public GameObject Ingredient;
    public int Health;
    public int IngredientNum;
}

public class CuttingTask : MonoBehaviour
{
    public Slider ProgressBar;
    public Slider TaskProgressBar;
    public GameObject TaskManager;

    Dictionary<int, Keys> IngredientKeys = new Dictionary<int, Keys>();

    public Image ArrowSign;
    public Image LetterSign;
    public Sprite Checkmark;

    public List<GameObject> IngredientCatalog = new List<GameObject>();

    public RectTransform SpawnBarTop;
    public RectTransform SpawnBarBottom;
    public RectTransform DespawnBarTop;
    public RectTransform DespawnBarBottom;

    List<GameObject> ActiveIngredients = new List<GameObject>();
    List<int> ActiveIngredientNums = new List<int>();
    List<int> CutIngredientNums = new List<int>();

    public int _totalIngredientsNeeded;
    public int _initSpawnCount = 3;
    public float _spawnDelay = 1;

    float _spawnTimer;
    float _totalSpawned;
    int _currentTask = 0;
    int _cutIngredients;
    int _targetIngredient;
    bool _taskSet;
    bool _arrowButtonPress;
    bool _letterButtonPress;

    private void Start()
    {
        _totalSpawned = _initSpawnCount;
        NewSpawn(SpawnBarTop);
        _initSpawnCount--;
        SwitchButtonSigns();
    }

    public void Update()
    {
        if (_cutIngredients < _totalSpawned)
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                string Input = CheckDpadInput();
                Keys x;
                x.Arrow = null; x.Ingredient = null; x.Letter = null;
                IngredientKeys.TryGetValue(ActiveIngredientNums[0], out x);
                if (Input == x.Arrow.name && !_arrowButtonPress)
                {
                    _arrowButtonPress = true;
                    ArrowSign.sprite = Checkmark;
                    StartCoroutine(SubHealthWaitTimer());
                }
            }

            if (Input.anyKeyDown)
            {
                string Input = GetButtonInput();
                Keys x;
                x.Arrow = null; x.Ingredient = null; x.Letter = null;
                IngredientKeys.TryGetValue(ActiveIngredientNums[0], out x);
                if (Input == x.Letter.name && !_letterButtonPress)
                {
                    _letterButtonPress = true;
                    LetterSign.sprite = Checkmark;
                    StartCoroutine(SubHealthWaitTimer());
                }
            }

            if (_taskSet == false)
            {
                SwitchButtonSigns();
            }
            

            if (_letterButtonPress && _arrowButtonPress)
            {
                TaskProgressBar.value += 1 / _totalSpawned;
                _cutIngredients += 1;
                CutIngredientNums.Add(_currentTask);
                ActiveIngredientNums.Remove(_currentTask);
                Keys _key;
                _key.Arrow = null;
                IngredientKeys.TryGetValue(_currentTask, out _key);
                ActiveIngredients.RemoveAt(_currentTask);
                IngredientKeys.Remove(_currentTask);
                if (_currentTask < _totalSpawned)
                {
                    if (ActiveIngredientNums.Count == 0)
                    {
                        _currentTask = 0;
                    }
                    else
                    {
                        _currentTask = ActiveIngredientNums[0];
                    }
                }
                else
                {
                    _currentTask = 0;
                }
                SwitchButtonSigns();
                _letterButtonPress = false;
                _arrowButtonPress = false;
            }

            if (_initSpawnCount > 0)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer > _spawnDelay)
                {
                    _initSpawnCount--;
                    NewSpawn(SpawnBarTop);
                    _spawnTimer = 0;
                }
            }
        }
        else
        {
            TaskManager.GetComponent<TaskManager>().TaskComplete = true;
        }
    }

    IEnumerator SubHealthWaitTimer()
    {
        Keys _key;
        _key.Ingredient = null;
        _key = IngredientKeys[_targetIngredient];
        while (_key.Ingredient == null)
        {
            _key = IngredientKeys[_targetIngredient];
            yield return new WaitForSeconds(0.000001f);
        }
        _key.Health--;
        _key.Ingredient.GetComponent<IngredientData>()._health = _key.Health;
        IngredientKeys[_targetIngredient] = _key;
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
            if (ActiveIngredients[i] != null)
            {
                Transform _ingredient = ActiveIngredients[i].GetComponent<Transform>();
                if (_ingredient.localPosition.y <= -110 && _ingredient.parent == DespawnBarTop.parent)
                {
                    Destroy(ActiveIngredients[i]);
                    ActiveIngredients.RemoveAt(i);
                    ActiveIngredientNums.RemoveAt(i);
                    print(_ingredient);
                    RespawnIngredient(SpawnBarBottom, _ingredient);
                }
                else if (_ingredient.localPosition.y <= -110 && _ingredient.parent == DespawnBarBottom.parent)
                {
                    Destroy(ActiveIngredients[i]);
                    ActiveIngredients.RemoveAt(i);
                    ActiveIngredientNums.RemoveAt(i);
                    print(_ingredient);
                    SpawnIngredient(SpawnBarTop);
                    ProgressBar.value -= 0.05f;
                }
            }

        }
    }

    void NewSpawn(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _indgredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[_indgredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 0), Quaternion.identity, Bar.parent);
        _spawnObject.GetComponent<IngredientData>().Source = gameObject;

        for (int i = 0; i < _totalSpawned; i++)
        {
            if (!IngredientKeys.ContainsKey(i) && !CutIngredientNums.Contains(i))
            {
                _spawnObject.GetComponent<IngredientData>()._ingredientNum = i;
                ActiveIngredients.Add(_spawnObject);
                ActiveIngredientNums.Add(i);
                break;
            }
        }
        _spawnObject.GetComponent<IngredientData>().NewIngredient();
    }

    void SpawnIngredient(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _indgredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[_indgredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 0), Quaternion.identity, Bar.parent);
        _spawnObject.GetComponent<IngredientData>().Source = gameObject;

        Keys _key;
        _key.Arrow = null;

        for (int i = 0; i < _totalSpawned; i++)
        {

            if (!ActiveIngredientNums.Contains(i) && !CutIngredientNums.Contains(i))
            {
                ActiveIngredientNums.Add(i);
                ActiveIngredients.Add(_spawnObject);
                _spawnObject.GetComponent<IngredientData>()._ingredientNum = i;
                IngredientKeys.TryGetValue(i, out _key);
                _spawnObject.GetComponent<IngredientData>().SetValues(_key.Letter.name, _key.Arrow.name, _key.Health, _key.IngredientNum);
                _key.Ingredient = _spawnObject;
            }
        }
    }


    void RespawnIngredient(RectTransform Bar, Transform Ingredient)
    {

        float x = Ingredient.localPosition.x;
        int _indgredientNum = Ingredient.GetComponent<IngredientData>()._ingredientNum;

        GameObject _ingredient = IngredientCatalog[_indgredientNum];
        GameObject _spawnObject = Instantiate(_ingredient, new Vector3(x, Bar.transform.position.y, 0), Quaternion.identity, Bar.parent);
        _spawnObject.transform.localPosition = new Vector3(x, _spawnObject.transform.localPosition.y, 1);
        _spawnObject.GetComponent<IngredientData>().Source = gameObject;

        Keys _key;
        _key.Arrow = null;

        for (int i = 0; i < _totalSpawned; i++)
        {
            if (!ActiveIngredientNums.Contains(i) && !CutIngredientNums.Contains(i))
            {
                ActiveIngredientNums.Add(i);
                ActiveIngredients.Add(_spawnObject);
                _spawnObject.GetComponent<IngredientData>()._ingredientNum = i;
                IngredientKeys.TryGetValue(i, out _key);
                _spawnObject.GetComponent<IngredientData>().SetValues(_key.Letter.name, _key.Arrow.name, _key.Health, _key.IngredientNum);
                _key.Ingredient = _spawnObject;
            }
        }
    }

    public void AddIngredientKey(int IngredientNum, Sprite ArrowKey, Sprite LetterKey, GameObject Ingredient, int _health)
    {
        if (!IngredientKeys.ContainsKey(IngredientNum))
        {
            Keys _keys;
            _keys.Arrow = ArrowKey;
            _keys.Letter = LetterKey;
            _keys.Ingredient = Ingredient;
            _keys.Health = _health;
            _keys.IngredientNum = IngredientNum;
            IngredientKeys.Add(IngredientNum, _keys);
        }
    }

    public void SwitchButtonSigns()
    {
        Keys _spriteKey;
        if (ActiveIngredientNums.Count > 0)
        {
            IngredientKeys.TryGetValue(ActiveIngredientNums[0], out _spriteKey);
            ArrowSign.sprite = _spriteKey.Arrow;
            LetterSign.sprite = _spriteKey.Letter;
            _targetIngredient = _spriteKey.IngredientNum;

            if (LetterSign.sprite != null && ArrowSign.sprite != null)
            {
                _taskSet = true;
            }
            else
            {
                _taskSet = false;
            }
        }
        else
        {
            _taskSet = false;
        }
    }

    public void IngredientEnterBowl(GameObject _ingredient)
    {
        Destroy(_ingredient);
        ActiveIngredients.Remove(_ingredient);
        SpawnIngredient(SpawnBarTop);
        TaskProgressBar.value += 1f / _totalIngredientsNeeded;
    }

    string CheckDpadInput()
    {
        float _horizontalAxisValue = Input.GetAxisRaw("Horizontal");
        float _verticalAxisValue = Input.GetAxisRaw("Vertical");
        bool rightInput = false;
        bool leftInput = false;
        bool upInput = false;
        bool downInput = false;
        rightInput = _horizontalAxisValue > 0.1f;
        leftInput = _horizontalAxisValue < -0.1f;
        upInput = _verticalAxisValue > 0.1f;
        downInput = _verticalAxisValue < -0.1f;

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            if (rightInput)
            {
                return "RIGHT";
            }
            else if (leftInput)
            {
                return "LEFT";
            }
            else if (upInput)
            {
                return "UP";
            }
            else if (downInput)
            {
                return "DOWN";
            }
        }
        return null;
    }

    string GetButtonInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            return "B";
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            return "A";
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            return "Y";
        }
        else if (Input.GetButtonDown("Jump"))
        {
            return "X";
        }
        return null;
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
