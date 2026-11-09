using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = SceneFlow.TitleScene;
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private SceneFlow sceneFlow;
    [SerializeField] private AudioService audioService;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private ToastView toastView;
    [SerializeField] private PauseMenu pauseMenu;

    private static GameBootstrap instance;
    private GameServices services;

    public static GameServices Services => instance != null ? instance.services : null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        var moveInput = new InputSystemMoveInput();
        var actionInput = new InputSystemActionInput();
        var gate = new InputGate(moveInput, actionInput);

        services = new GameServices
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession(),
            Scenes = sceneFlow,
            Save = new SaveService(),
            Audio = audioService,
            Dialogue = dialogueRunner,
            Toast = toastView,
            Flags = new StoryFlags(),
            Pause = pauseMenu
        };

        if (sceneFlow != null) sceneFlow.Construct(services);
        if (dialogueRunner != null) dialogueRunner.Construct(gate, audioService);
        if (pauseMenu != null) pauseMenu.Construct(services);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(firstScene, LoadSceneMode.Additive);
        Scene active = SceneManager.GetSceneByName(firstScene);
        SceneManager.SetActiveScene(active);

        SceneInstaller installer = FindAnyObjectByType<SceneInstaller>();
        if (installer != null)
        {
            installer.Install(services);
        }
    }

    private void Update()
    {
        if (services != null && services.Party != null)
        {
            // Update playtime when not in Title or End scene
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == SceneFlow.WorldScene || currentScene == SceneFlow.BattleScene)
            {
                services.Party.TickPlaytime(Time.unscaledDeltaTime);
            }
        }
    }
}
