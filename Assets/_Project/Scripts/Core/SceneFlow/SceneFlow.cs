using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFlow : MonoBehaviour
{
    public const string BootstrapScene = "Scn_Bootstrap";
    public const string TitleScene = "Scn_Title";
    public const string WorldScene = "Scn_World";
    public const string BattleScene = "Scn_Battle";
    public const string EndScene = "Scn_End";

    [SerializeField] private Image fader;
    [SerializeField] private float fadeDuration = 0.4f;

    private GameServices services;
    public bool IsBusy { get; private set; }
    public bool HoldFade { get; set; }

    public void Construct(GameServices gameServices)
    {
        services = gameServices;
    }

    private void Awake()
    {
        if (fader != null)
        {
            fader.color = new Color(0, 0, 0, 0);
            fader.gameObject.SetActive(false);
        }
    }

    public void GoTo(string sceneName)
    {
        if (IsBusy) return;
        StartCoroutine(TransitionToScene(sceneName));
    }

    public void StartBattle(MonsterInstance enemy, bool isBoss = false)
    {
        if (IsBusy) return;
        StartCoroutine(EnterBattle(enemy, isBoss));
    }

    public void EndBattle(BattleResult result)
    {
        if (IsBusy) return;
        StartCoroutine(ExitBattle(result));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fader == null) yield break;
        fader.gameObject.SetActive(true);
        float startAlpha = fader.color.a;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fader.color = new Color(0, 0, 0, a);
            yield return null;
        }
        fader.color = new Color(0, 0, 0, targetAlpha);
        if (targetAlpha <= 0f) fader.gameObject.SetActive(false);
    }

    public IEnumerator ReleaseFade()
    {
        HoldFade = false;
        yield return Fade(0f);
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        IsBusy = true;
        services?.Input?.Lock();

        yield return Fade(1f);

        // Unload currently active content scene if not Bootstrap
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name != BootstrapScene && s.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(s);
            }
        }

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene loaded = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loaded);

        // Find SceneInstaller in newly loaded scene
        SceneInstaller installer = FindAnyObjectByType<SceneInstaller>();
        if (installer != null && services != null)
        {
            installer.Install(services);
        }

        if (!HoldFade)
        {
            yield return Fade(0f);
        }

        services?.Input?.Unlock();
        IsBusy = false;
    }

    private IEnumerator EnterBattle(MonsterInstance enemy, bool isBoss)
    {
        IsBusy = true;
        services?.Input?.Lock();
        services?.Battle?.StartBattle(enemy, isBoss);

        yield return Fade(1f);

        yield return SceneManager.LoadSceneAsync(BattleScene, LoadSceneMode.Additive);
        Scene battleScene = SceneManager.GetSceneByName(BattleScene);
        SceneManager.SetActiveScene(battleScene);

        SceneInstaller installer = FindAnyObjectByType<BattleInstaller>();
        if (installer != null && services != null)
        {
            installer.Install(services);
        }

        yield return Fade(0f);
        IsBusy = false;
    }

    private IEnumerator ExitBattle(BattleResult result)
    {
        IsBusy = true;

        yield return Fade(1f);

        Scene battleScene = SceneManager.GetSceneByName(BattleScene);
        if (battleScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(battleScene);
        }

        Scene worldScene = SceneManager.GetSceneByName(WorldScene);
        if (worldScene.isLoaded)
        {
            SceneManager.SetActiveScene(worldScene);
        }

        services?.Battle?.EndBattle(result);
        services?.Audio?.ResumeWorldMusic();

        yield return Fade(0f);
        services?.Input?.Unlock();
        IsBusy = false;
    }
}
