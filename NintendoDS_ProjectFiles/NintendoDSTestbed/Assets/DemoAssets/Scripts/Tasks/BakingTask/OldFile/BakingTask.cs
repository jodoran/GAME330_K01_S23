using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface State
{
    public void Enter(TaskScreen newTask);
    public void Execute(TaskScreen currentTask, Vector2 tapPosition, string CallFunction);
    public void Exit(TaskScreen pastTask);
}

[Serializable]
public struct TaskScreen
{
    public string TaskName;
    public List<GameObject> EnteryObjects;
    public List<GameObject> ExitObjects;
    [HideInInspector]
    public bool TaskComplete;
    [HideInInspector]
    public string SetTime;
}

public class StateMachine
{
    private State currentState; //Current State
    private TaskScreen currentTask; //Current Task

    public void ChangeState(State newState, TaskScreen newTask)
    {
        if (currentState != null)
            currentState.Exit(currentTask);

        currentTask = newTask;
        currentState = newState;
        currentState.Enter(newTask);
    }

    public void Update(Vector2 tapPosition, string callFunction)
    {
        if (currentState != null) currentState.Execute(currentTask, tapPosition, callFunction);
    }
}

public class StateCommand : StateTasks, State
{
    BakingTask owner;
    StateTasks taskManager = new StateTasks();
    public StateCommand(BakingTask owner) { this.owner = owner; }
    

    public void Enter(TaskScreen newTask)
    {
        Load(newTask);
    }

    public void Execute(TaskScreen currentTask, Vector2 tapPosition, string CallFunction)
    {
        switch (CallFunction)
        {
            case "UP":
                switch (currentTask.TaskName)
                {
                    case "Moving":
                        taskManager.MovingComplete(currentTask, tapPosition);
                        break;
                    case "Placing":
                        taskManager.PlacingComplete(currentTask, tapPosition);
                        break;
                    case "Setting":
                        taskManager.Setting(currentTask, tapPosition);
                        taskManager.SettingComplete(currentTask, tapPosition);
                        break;
                }
                break;
            case "DOWN":
            case "DRAG":
                switch (currentTask.TaskName)
                {
                    case "Moving":
                    taskManager.Moving(currentTask, tapPosition);
                    break;
                    case "Placing":
                    taskManager.Placing(currentTask, tapPosition);
                    break;
                }
                break;
        }
    }

    public void Exit(TaskScreen pastTask)
    {
        Clear(pastTask);
    }
}

public class BakingTask : MonoBehaviour, IDSTapListener
{
    public Slider ProgressBar;
    public Text InstructionText;
    public List<string> Instructions;

    public List<TaskScreen> Tasks = new List<TaskScreen>();
    StateMachine stateMachine = new StateMachine();
    [HideInInspector]
    public TaskScreen currentTask;

    GameObject TaskManager;
    string setTask = "Moving";
    int TaskNum = 0;
    string TimeSet;

    private void Start()
    {
        TaskManager = GameObject.Find("TaskManager");
        currentTask = FindTask();
        stateMachine.ChangeState(new StateCommand(this), currentTask);
        SetInstructions();
    }

    private void Update()
    {

        if (currentTask.TaskComplete == true)
        {
            TaskNum++;

            if (TaskNum < Tasks.Count)
            {
                // Get Next Task
                setTask = Tasks[TaskNum].TaskName;
                currentTask = FindTask();

                // Change to New Task
                stateMachine.ChangeState(new StateCommand(this), currentTask);
                SetInstructions();

                // Reset to detect Completion of Task
                currentTask.TaskComplete = false;
            }
            else if (TaskNum >= Tasks.Count)
            {
                if (currentTask.SetTime != TimeSet)
                {
                    float progress = ProgressBar.value;
                    ProgressBar.value = progress - 0.1f;
                }
                TaskManager.GetComponent<TaskManager>().TaskComplete = true;
            }
        }

    }

    public void OnScreenTapDown(Vector2 tapPosition){
        stateMachine.Update(tapPosition, "DOWN");
    }

    public void OnScreenDrag(Vector2 tapPosition) {
        stateMachine.Update(tapPosition, "DRAG");
    }


    public void OnScreenTapUp(Vector2 tapPosition) {
        stateMachine.Update(tapPosition, "UP");
    }


    TaskScreen FindTask()
    {
        foreach (TaskScreen task in Tasks)
        {
            if(task.TaskName == setTask)
            {
                return task;
            }
        }
        return Tasks[0];
    }


    void SetInstructions()
    {
        if (currentTask.TaskName == "Setting")
        {
            string timeText;
            float time = UnityEngine.Random.Range(0, 90);

            float Hours = time / 60;

            if (Hours >= 1) { Hours = 1; }
            else { Hours = 0; }

            float Mins = time - (Hours * 60);

            if (Mins < 10) { timeText = Hours.ToString() + ":0" + Mins.ToString(); }
            else { timeText = Hours.ToString() + ":" + Mins.ToString(); }

            InstructionText.text = Instructions[TaskNum] + timeText;
            TimeSet = timeText;
        }
        else
        {
            InstructionText.text = Instructions[TaskNum];
        }
    }
}

public class StateTasks : MonoBehaviour
{
    // Moving Variables
    GameObject Bowl;
    GameObject Container;

    // Placing Variables
    GameObject Oven;

    // Setting Variables
    GameObject IncreaseTime;
    GameObject DecreaseTime;
    GameObject Time;
    GameObject SubmitButton;

    // Task Call Functions

    // Moving Task
    public void Moving(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        MovingTaskSetup(currentTask);

        // Moving Main Function
        if (DSTapRouter.RectangleContainsDSPoint(Bowl.GetComponent<RectTransform>(), tapPosition))
        {
            MoveWithCursor(Bowl.GetComponent<RectTransform>(), tapPosition);
        }
    }

    public void MovingComplete(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        MovingTaskSetup(currentTask);
        BakingTask TaskMaster;

        if (DSTapRouter.RectangleContainsDSPoint(Bowl.GetComponent<RectTransform>(), tapPosition))
        {
            // Task CompletionCheck
            if (rectOverlaps(Bowl.GetComponent<RectTransform>(), Container.GetComponent<RectTransform>()))
            {
                Container.transform.GetChild(1).gameObject.SetActive(true);
                TaskMaster = FindObjectOfType<BakingTask>();
                TaskMaster.currentTask.TaskComplete = true;
            }
        }
    }


    // Placing Task
    public void Placing(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        PlacingTaskSetup(currentTask);

        //Placing Main Function
        if (DSTapRouter.RectangleContainsDSPoint(Container.GetComponent<RectTransform>(), tapPosition))
        {
            MoveWithCursor(Container.GetComponent<RectTransform>(), tapPosition);
        }
    }

    public void PlacingComplete(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        PlacingTaskSetup(currentTask);
        BakingTask TaskMaster;

        // Task CompletionCheck
        if (DSTapRouter.RectangleContainsDSPoint(Container.GetComponent<RectTransform>(), tapPosition))
        {
            if (rectOverlaps(Container.GetComponent<RectTransform>(), Oven.GetComponent<RectTransform>()))
            {
                TaskMaster = FindObjectOfType<BakingTask>();
                TaskMaster.currentTask.TaskComplete = true;
            }
        }
    }


    // Seting Task
    public void Setting(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        SettingTaskSetup(currentTask);


        if (IncreaseTime != null && DecreaseTime != null)
        {
            if (DSTapRouter.RectangleContainsDSPoint(IncreaseTime.GetComponent<RectTransform>(), tapPosition))
            {
                float timeNum = GetTime();
                if (timeNum < 1000)
                    timeNum++;
                SetTime(timeNum);
            }
        }

        if (DSTapRouter.RectangleContainsDSPoint(DecreaseTime.GetComponent<RectTransform>(), tapPosition))
        {
            float timeNum = GetTime();
            if (timeNum > 0)
                timeNum--;
            SetTime(timeNum);
        }
    }
    

    public void SettingComplete(TaskScreen currentTask, Vector2 tapPosition)
    {
        // Set Variables
        SettingTaskSetup(currentTask);
        BakingTask TaskMaster;

        if (DSTapRouter.RectangleContainsDSPoint(SubmitButton.GetComponent<RectTransform>(), tapPosition))
        {
            TaskMaster = FindObjectOfType<BakingTask>();

            TaskMaster.currentTask.SetTime = Time.GetComponentInChildren<Text>().text;
            TaskMaster.currentTask.TaskComplete = true;
        }
    }

    // Callable Setup Functions
    public void Load(TaskScreen newTask)
    {
        foreach (GameObject objects in newTask.EnteryObjects)
        {
            objects.SetActive(true);
        }
    }

    public void Clear(TaskScreen pastTask)
    {
        foreach(GameObject objects in pastTask.ExitObjects)
        {
            objects.SetActive(false);
        }
    }


    // Setup Functions
    void MovingTaskSetup(TaskScreen task)
    {
        try
        {
            Bowl = FindObject("Bowl", task.EnteryObjects);
            Container = FindObject("Container", task.EnteryObjects);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Object Reference in Baking Task: Moving Function not found");
        }
    }

    void PlacingTaskSetup(TaskScreen task)
    {
        try
        {
            Oven = FindObject("Oven", task.EnteryObjects);
            Container = FindObject("Container", task.EnteryObjects);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Object Reference in Baking Task: Placing Function not found");
        }
    }

    void SettingTaskSetup(TaskScreen task)
    {
        try
        {
            IncreaseTime = FindObject("IncreaseTime", task.EnteryObjects);
            DecreaseTime = FindObject("DecreaseTime", task.EnteryObjects);
            SubmitButton = FindObject("SubmitButton", task.EnteryObjects);
            Time = FindObject("Time", task.EnteryObjects);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Object Reference in Baking Task: Setting Function not found");
        }
    }


    // Interaction Functions
    void MoveWithCursor(RectTransform Object, Vector2 cursorPosition) //Sets plate to position of the cursor
    {
        // Gets Mouse position
        Vector3 currPosition = Object.anchoredPosition;
        currPosition.x += (cursorPosition.x - (currPosition.x + 150));
        currPosition.y += (cursorPosition.y - (currPosition.y + 90));

        // Sets plate rect transform to that position
        Object.anchoredPosition = currPosition;
    }

    bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2) //Gets if the 1st RectTransform is overlapping the 2nd RectTransform
    {
        // recreates both RectTransforms poitions current position and size
        Rect rect1 = new Rect(rectTrans1.anchoredPosition.x, rectTrans1.anchoredPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.anchoredPosition.x, rectTrans2.anchoredPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        // uses the recreated RectTransforms positions and sizes to check if they are overlapping
        return rect1.Overlaps(rect2);
    }

    GameObject FindObject(string ObjectName, List<GameObject> ObjectList)
    {
        foreach(GameObject Object in ObjectList)
        {
            if (ObjectName == Object.name)
            {
                return Object;
            }
        }
        return null;
    }

    float GetTime()
    {
        string timeText = Time.GetComponentInChildren<Text>().text;
        timeText = timeText.Replace(":", ".");
        float timeNum = float.Parse(timeText);

        if (timeNum >= 1)
        {
            float Hours = timeNum;
            string[] x = Hours.ToString().Split(char.Parse("."));
            Hours = float.Parse(x[0]);
            float Mins = timeNum;
            Mins = Mins - Hours;
            Mins = Mins * 100;
            timeNum = (Hours * 60) + Mins;
        }
        else
        {
            float Mins = timeNum;
            timeNum = Mins * 100;
        }

        return timeNum;
    }

    void SetTime(float timeNum)
    {
        float Hours = timeNum / 60;
        string[] x = Hours.ToString().Split(char.Parse("."));
        Hours = float.Parse(x[0]);
        float Mins = Mathf.Round((timeNum - (Hours * 60)) * 100f) / 100f;

        if (Mins < 10) Time.GetComponentInChildren<Text>().text = Hours.ToString() + ":0" + Mins;
        else Time.GetComponentInChildren<Text>().text = Hours.ToString() + ":" + Mins;
    }

}


