using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiftingTask : MonoBehaviour
{
    public GameObject TaskManager;
    public Slider ProgressBar;
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
                Destroy(transform.GetChild(1).GetChild(0).gameObject);
                Vector3 pos = gameObject.transform.localPosition;

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
                Destroy(transform.GetChild(1).GetChild(0).gameObject);
                Vector3 pos = gameObject.transform.localPosition;

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
                }

                PowderDispersalPaticleEffect.Play();
                _rightArrow.GetComponent<Animator>().enabled = true;
                _leftArrow.GetComponent<Animator>().enabled = false;
                TimeBar.value = 1;
            }
        }

        if(_clearCounter >= _spawnCount)
        {
            TaskManager.GetComponent<TaskManager>().TaskComplete = true;
        }

        TimeBar.value -= Time.deltaTime / _decreaseSpeed;
    }

    void Spawner()
    {
        float _xValue = Random.Range(_spawnRadius, -_spawnRadius);
        float _yValue = Random.Range(_spawnRadius, -_spawnRadius);
        GameObject _powder = Instantiate(_powderObject, new Vector3(_xValue, _yValue, 1), Quaternion.identity, transform.GetChild(1));
        _powder.GetComponent<RectTransform>().localPosition = new Vector3(_xValue, _yValue, 1);
    }
}
