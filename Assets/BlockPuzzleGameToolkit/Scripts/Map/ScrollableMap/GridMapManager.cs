using UnityEngine;
using BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap;
using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.Popups;
using UnityEngine.UI;
using System.Linq;

public class GridMapManager : SingletonBehaviour<GridMapManager>
{
    private List<LevelPin> openedLevels = new List<LevelPin>();
    private List<LevelPin> allLevels = new List<LevelPin>();

    [SerializeField] 
    private CustomButton backButton;

    [SerializeField] 
    private ScrollMap scrollMap;

    [SerializeField] 
    private Transform levelsGrid;

    [SerializeField]
    public LevelPin levelPrefab;

    private void OnEnable()
    {
        UpdateLevels();
    }

    private void Start()
    {
        backButton.onClick.AddListener(SceneLoader.instance.GoMain);
       
        CreateLevels();
    }

    private void CreateLevels()
    {
        openedLevels.Clear();

        int totalLevelsInResources = Resources.LoadAll<Level>("Levels").Length;
        int lastLevel = GameDataManager.GetLevelNum();

        for (int i = 0; i < totalLevelsInResources; i++)
        {
            int newLevelNumber = i + 1;
            LevelPin newPin = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, levelsGrid);

            // Assign proper level number
            newPin.name = $"Level_{newLevelNumber}";
            newPin.SetNumber(newLevelNumber);

            allLevels.Add(newPin);

            // Set lock state based on current progress
            if (newLevelNumber > lastLevel)
            {
                newPin.Lock();
            }
            else
            {
                newPin.UnLock();
                if (!openedLevels.Contains(newPin))
                {
                    openedLevels.Add(newPin);
                }
                newPin.SetCurrent(newLevelNumber == lastLevel);
            }
        }

        scrollMap.ScrollToAvatar(GetPositionOpenedLevel());

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
    }

    private void UpdateLevels()
    {
        int lastLevel = GameDataManager.GetLevelNum();

        for (int i = 0; i < allLevels.Count; i++)
        {
            // Set lock state based on current progress
            if (allLevels[i].number <= lastLevel)
            {
                if (allLevels[i].IsLocked)
                {
                    allLevels[i].UnLock();
                    if (!openedLevels.Contains(allLevels[i]))
                    {
                        openedLevels.Add(allLevels[i]);
                    }
                }
                allLevels[i].SetCurrent(allLevels[i].number == lastLevel);
            }
        }
    }

    private Vector3 GetPositionOpenedLevel()
    {
        int currentLevel = GameDataManager.GetLevelNum();
        LevelPin currentLevelPin = openedLevels.FirstOrDefault(pin => pin.number == currentLevel);
        return currentLevelPin != null ? currentLevelPin.transform.position : openedLevels[^1].transform.position;
    }

    public void OpenLevel(int number)
    {
        SceneLoader.instance.StartGameScene(number);
    }
}
