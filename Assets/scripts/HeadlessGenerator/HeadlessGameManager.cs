using Edgar.Unity.Examples;
using UnityEngine;
using System.Collections;
using System;

public class HeadlessGameManager : GameManagerBase<HeadlessGameManager>
{
    [SerializeField]
    private HeadlessDungeonGeneratorGrid2D generator;

    public new void Awake()
    {
         if (Instance == null)
         {
            Instance = this;
         }
        else if (!ReferenceEquals(Instance, this))
        {
            Destroy(gameObject);
            return;
        }

        if (Canvas != null)
        {
            Canvas.SetActive(true);
        }

        // generate the graph once
        generator.GenerateGraph();
        // then we'll load several levels with this graph
        LoadNextLevel();
    }

    public void Update()
    {
        /*if (InputHelper.GetKeyDown(KeyCode.G))
        {
            LoadNextLevel();
        }*/
    }

    public override void LoadNextLevel()
    {
        // Start the generator coroutinel
        StartCoroutine(GeneratorCoroutine());
    }

    /// <summary>
    /// Coroutine that generates the level.
    /// We need to yield return before the generator starts because we want to show the loading screen
    /// and it cannot happen in the same frame.
    /// It is also sometimes useful to yield return before we hide the loading screen to make sure that
    /// all the scripts that were possibly created during the process are properly initialized.
    /// </summary>
    private IEnumerator GeneratorCoroutine()
    {
        /*
        var stopwatch = new Stopwatch();

        stopwatch.Start();

        ShowLoadingScreen("Headless", "loading..");
        yield return null;

        generator.Generate();

        yield return null;

        stopwatch.Stop();

        SetLevelInfo($"Generated in {stopwatch.ElapsedMilliseconds / 1000d:F}s");
        HideLoadingScreen();
        */

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();


        // try several times to generate the dungeon with current graph
        bool generated = false;
        int numberOfTry = 5;
        int i = 1;
        while(!generated && i <= numberOfTry)
        {
            ShowLoadingScreen("Generating dungeon", "generating graph...");
            yield return new WaitForSeconds(.1f);

            generator.GenerateGraph();
            yield return null;

            ShowLoadingScreen("Generating dungeon", "generating layout - (" + i.ToString() + "/" + numberOfTry + ")");
            yield return new WaitForSeconds(.1f);

            i++;

            try
            {
                generator.Generate();
                generated = true;
            }
            catch (Exception) {
                Debug.LogWarning("Try " + i.ToString() + " failed.");
            }
        }
        yield return null;

        // if still fails, generate a very easy dungeon
        if (!generated)
        {
            ShowLoadingScreen("Generating dungeon", "Failed to generate dungeon, generating fallback graph...");
            yield return new WaitForSeconds(.1f);

            bool oldDisableRight = generator.disableRight;
            generator.disableRight = true;
            generator.GenerateGraph();
            yield return null;
            generator.disableRight = oldDisableRight;

            ShowLoadingScreen("Generating dungeon", "generating fallback layout");
            yield return new WaitForSeconds(.1f);

            generator.Generate();
            yield return null;
        }

        stopwatch.Stop();

        SetLevelInfo($"Generated in {stopwatch.ElapsedMilliseconds / 1000d:F}s");
        HideLoadingScreen();
    }
}