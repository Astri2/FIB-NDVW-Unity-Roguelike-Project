using Edgar.Unity.Examples;
using Edgar.Unity;
using System.Diagnostics;
using UnityEngine;
using Edgar.Unity.Examples.Example2;
using System.Collections;

public class HeadlessGameManager : GameManagerBase<HeadlessGameManager>
{
    [SerializeField]
    private HeadlessDungeonGeneratorGrid2D generator;

    public void Update()
    {
        if (InputHelper.GetKeyDown(KeyCode.G))
        {
            LoadNextLevel();
        }
    }

    public override void LoadNextLevel()
    {
        // Show loading screen
        ShowLoadingScreen("Headless", "loading..");

        // Start the generator coroutine
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
        var stopwatch = new Stopwatch();

        stopwatch.Start();

        yield return null;

        generator.Generate();

        yield return null;

        stopwatch.Stop();

        SetLevelInfo($"Generated in {stopwatch.ElapsedMilliseconds / 1000d:F}s");
        HideLoadingScreen();
    }
}