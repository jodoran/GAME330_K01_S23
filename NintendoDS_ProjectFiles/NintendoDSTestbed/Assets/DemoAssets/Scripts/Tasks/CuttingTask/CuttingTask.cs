using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public AudioSource CuttingSoundEffect1;
    public AudioSource CuttingSoundEffect2;
    public AudioSource FallSound;

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
    public float _fallspeed;

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

    bool down = false;
    public void Update()
    {
        string _dpadInput = CheckDpadInput();
        string _buttonInput = GetButtonInput();


        if (_cutIngredients < _totalSpawned - 1)
        {
            float _horizontalAxisValue = Input.GetAxisRaw("Horizontal");
            float _verticalAxisValue = Input.GetAxisRaw("Vertical");
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || (!down && (_horizontalAxisValue != 0 || _verticalAxisValue != 0)))
            {
                Keys x;
                x.Arrow = null; x.Ingredient = null; x.Letter = null;
                if(IngredientKeys.TryGetValue(_currentTask, out x)){

                    if (_dpadInput == x.Arrow.name)
                    {
                        _arrowButtonPress = true;
                        ArrowSign.sprite = Checkmark;
                        StartCoroutine(SubHealthWaitTimer());

                        if (_letterButtonPress && _arrowButtonPress)
                        {
                            SetNewKey();
                        }
                    }
                }
            }
            if (_horizontalAxisValue != 0 || _verticalAxisValue != 0)
            {
                down = true;
            }
            else
            {
                down = false;
            }

            if (Input.anyKeyDown)
            {
                Keys x;
                x.Arrow = null; x.Ingredient = null; x.Letter = null;
                if (IngredientKeys.TryGetValue(_currentTask, out x))
                {
                    if (_buttonInput == x.Letter.name)
                    {
                        _letterButtonPress = true;
                        LetterSign.sprite = Checkmark;
                        StartCoroutine(SubHealthWaitTimer());

                        if (_letterButtonPress && _arrowButtonPress)
                        {
                            SetNewKey();
                        }
                    }
                }
            }

            if (_taskSet == false)
            {
                SwitchButtonSigns();
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

    void SetNewKey()
    {
        TaskProgressBar.value += 1 / _totalSpawned;
        _cutIngredients += 1;
        if (_cutIngredients! < _totalSpawned)
        {
            CutIngredientNums.Add(_currentTask);
            ActiveIngredientNums.Remove(_currentTask);
            Keys _key;
            _key.Arrow = null;
            IngredientKeys.TryGetValue(_currentTask, out _key);
            ActiveIngredients.Remove(_key.Ingredient);
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
    }

    IEnumerator SubHealthWaitTimer()
    {
        Keys _key;
        _key.Ingredient = null;
        _key = IngredientKeys[_targetIngredient];
        while (_key.Ingredient == null)
        {
            try
            {
                _key = IngredientKeys[_targetIngredient];
            }
            catch(KeyNotFoundException) { }
            yield return new WaitForSeconds(0.000001f);
        }
        _key.Health--;
        if (_key.Health == 1)
        {
            CuttingSoundEffect1.Play();

        }
        else if (_key.Health == 0)
        {
            CuttingSoundEffect2.Play();
        }

        if (TaskManager.GetComponent<TaskManager>().HealingEnabled)
        {
            ProgressBar.value += 0.05f;
        }

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

    // Called on update to check if any objects need moved to the other screen
    void CheckActiveIngredientsOverlap()
    {
        // Runs through each active ingredient
        for (int i = ActiveIngredients.Count-1; i >= 0; i--)
        {
            // Checks to make sure the object isn't null
            if (ActiveIngredients[i] != null)
            {
                // Checks y position of object and objects parent to know whether the object needs respawned and to which screen
                Transform _ingredient = ActiveIngredients[i].GetComponent<Transform>();
                if (_ingredient.localPosition.y <= -110 && _ingredient.parent == DespawnBarTop.parent)
                {
                    // Spawns object on bottom screen removing & destroying the old object from the active ingredients
                    // Spawns new object -> Removes old object from active list -> Destroys old object -> Removes objects assigned number from active ingredients numbers to reassign it to the new object
                    RespawnIngredient(SpawnBarBottom, _ingredient);
                    ActiveIngredients.RemoveAt(i);
                    Destroy(_ingredient.gameObject);
                    ActiveIngredientNums.RemoveAt(i);
                }
                else if (_ingredient.localPosition.y <= -110 && _ingredient.parent == DespawnBarBottom.parent)
                {
                    // Spawns object on top screen removing & destroying the old object from the active ingredients
                    // Spawns new object -> Removes old object from active list -> Destroys old object -> Removes objects assigned number from active ingredients numbers to reassign it to the new object
                    FallSound.Play();
                    SpawnIngredient(SpawnBarTop);
                    ProgressBar.value -= 0.05f;
                    ActiveIngredients.RemoveAt(i);
                    Destroy(_ingredient.gameObject);
                    ActiveIngredientNums.RemoveAt(i);
                }
            }

        }
    }

    // Initial spawn used when a new ingredient is being created for the first time
    void NewSpawn(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _indgredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[1];
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

    // Top screen spawn used to spawn in an ingredient on the top screen
    void SpawnIngredient(RectTransform Bar)
    {
        float _maxX = Bar.GetChild(0).position.x;
        float _minX = Bar.GetChild(1).position.x;

        float x = UnityEngine.Random.Range(_minX, _maxX);
        int _indgredientNum = UnityEngine.Random.Range(0, IngredientCatalog.Count);

        GameObject _ingredient = IngredientCatalog[1];
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
                if(IngredientKeys.TryGetValue(i, out _key)){
                    _spawnObject.GetComponent<IngredientData>().SetValues(_key.Letter.name, _key.Arrow.name, _key.Health, _key.IngredientNum);
                    _key.Ingredient = _spawnObject;
                    IngredientKeys[i] = _key;
                }
            }
        }
    }

    // Bottom screen spawn used to spawn in an ingredient on the bottom screen
    void RespawnIngredient(RectTransform Bar, Transform Ingredient)
    {

        float x = Ingredient.localPosition.x;
        int _indgredientNum = Ingredient.GetComponent<IngredientData>()._ingredientNum;

        GameObject _ingredient = IngredientCatalog[1];
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
                if( IngredientKeys.TryGetValue(i, out _key))
                {
                    _spawnObject.GetComponent<IngredientData>().SetValues(_key.Letter.name, _key.Arrow.name, _key.Health, _key.IngredientNum);
                    _key.Ingredient = _spawnObject;
                    IngredientKeys[i] = _key;
                }

            }
        }
    }

    // Takes in ingredients information from IngredientData on object and creates a key value in the IngredientKeys dictionary
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
    
    // Swaps the current input signs according to the active ingredients set ArrowKey and LetterKey
    // to know what buttons to check for in completing the task
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

    // Returns the input type from the d-pad and key buttons A,B,X,Y
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

        return null;
    }

    string GetButtonInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            return "B";
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return "A";
        }
        if (Input.GetButtonDown("Fire3"))
        {
            return "Y";
        }
        if (Input.GetButtonDown("Jump"))
        {
            return "X";
        }
        return null;
    }

}
