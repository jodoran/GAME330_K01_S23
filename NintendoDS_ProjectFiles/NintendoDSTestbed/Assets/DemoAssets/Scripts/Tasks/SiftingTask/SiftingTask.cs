using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiftingTask : MonoBehaviour
{
    public GameObject TaskManager;
    public Slider ProgressBar;
    public AudioSource ShakeSound;
    public AudioSource MissSound;

    public Slider TimeBar;
    public ParticleSystem PowderDispersalPaticleEffect;
    public float _spawnRadius;
    public int _spawnCount;
    public GameObject _powderObject;
    public GameObject _rightArrow;
    public GameObject _leftArrow;

    public float _timeBarSuccessMin;
    public float _decreaseSpeed;

    private int _clearCounter;
    private string _bumperKey = "Left";
    private bool _shaking;
    private float _timer;
    private bool _started = false;

    private void Start()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Spawner();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("LeftBumper"))
        {
            if(_bumperKey == "Left")
            {
                if (!_started)
                {
                    _started = true;
                }

                Destroy(transform.GetChild(1).GetChild(0).gameObject);
                Vector3 pos = gameObject.transform.localPosition;

                if (!_shaking)
                {
                    _shaking = true;
                    ShakeSound.Play();
                }
                _timer = 0;

                Vector3 childPos = transform.GetChild(1).GetChild(0).localPosition;
                childPos.z = 0;
                childPos.y -= 30;
                PowderDispersalPaticleEffect.transform.localPosition = childPos;

                pos.x -= 5;
                gameObject.transform.localPosition = pos;
                _bumperKey = "Right";
                _clearCounter++;

                if(TimeBar.value <= _timeBarSuccessMin)
                {
                    ProgressBar.value -= 0.05f;
                    MissSound.Play();
                }
                else if (TaskManager.GetComponent<TaskManager>().HealingEnabled)
                {
                    ProgressBar.value += 0.05f;
                }

                PowderDispersalPaticleEffect.Play();
                _leftArrow.GetComponent<Animator>().enabled = true;
                _rightArrow.GetComponent<Animator>().enabled = false;
                TimeBar.value = 1;
            }
        }
        if (Input.GetButtonDown("RightBumper"))
        {
            if (_bumperKey == "Right")
            {
                if (!_started)
                {
                    _started = true;
                }

                Destroy(transform.GetChild(1).GetChild(0).gameObject);
                Vector3 pos = gameObject.transform.localPosition;

                if (!_shaking)
                {
                    _shaking = true;
                    ShakeSound.Play();
                }
                _timer = 0;

                Vector3 childPos = transform.GetChild(1).GetChild(0).localPosition;
                childPos.z = 0;
                childPos.y -= 30;
                PowderDispersalPaticleEffect.transform.localPosition = childPos;

                pos.x += 5;
                gameObject.transform.localPosition = pos;
                _bumperKey = "Left";
                _clearCounter++;

                if (TimeBar.value <= _timeBarSuccessMin)
                {
                    ProgressBar.value -= 0.05f;
                    MissSound.Play();
                }
                else if (TaskManager.GetComponent<TaskManager>().HealingEnabled)
                {
                    ProgressBar.value += 0.05f;
                }

                PowderDispersalPaticleEffect.Play();
                _rightArrow.GetComponent<Animator>().enabled = true;
                _leftArrow.GetComponent<Animator>().enabled = false;
                TimeBar.value = 1;
            }
        }

        if(_clearCounter >= _spawnCount)
        {
            _shaking = false;
            ShakeSound.Stop();
            TaskManager.GetComponent<TaskManager>().TaskComplete = true;
        }

        if (_started)
        { TimeBar.value -= Time.deltaTime / _decreaseSpeed; }

        if (_shaking)
        {
            _timer += Time.deltaTime;
            if (_timer >= 3f)
            {
                ShakeSound.Stop();
                _shaking = false;
                _timer = 0;
            }
        }
    }

    void Spawner()
    {
        float _xValue = Random.Range(_spawnRadius, -_spawnRadius);
        float _yValue = Random.Range(_spawnRadius, -_spawnRadius);
        GameObject _powder = Instantiate(_powderObject, new Vector3(_xValue, _yValue, 1), Quaternion.identity, transform.GetChild(1));
        _powder.GetComponent<RectTransform>().localPosition = new Vector3(_xValue, _yValue, 1);
    }
}
