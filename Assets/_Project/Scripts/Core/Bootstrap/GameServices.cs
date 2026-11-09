public class GameServices
{
    public InputGate Input { get; set; }
    public MonsterDatabase Database { get; set; }
    public PlayerParty Party { get; set; }
    public BattleSession Battle { get; set; }
    public SceneFlow Scenes { get; set; }
    public SaveService Save { get; set; }
    public AudioService Audio { get; set; }
    public DialogueRunner Dialogue { get; set; }
    public ToastView Toast { get; set; }
    public StoryFlags Flags { get; set; }
    public PauseMenu Pause { get; set; }
}
