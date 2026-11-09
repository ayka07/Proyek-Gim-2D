using System.Collections;
using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] protected string npcName = "Warga";
    [SerializeField] protected SpriteRenderer body;
    [SerializeField] protected Sprite faceDown;
    [SerializeField] protected Sprite faceLeft;
    [SerializeField] protected Sprite faceRight;
    [SerializeField] protected Sprite faceUp;
    [SerializeField] protected DialogueData dialogue;

    public string NpcName => npcName;
    protected DialogueRunner Dialogue { get; private set; }
    protected StoryFlags Flags { get; private set; }
    protected GameServices Services { get; private set; }

    public void Construct(GameServices services)
    {
        Services = services;
        Dialogue = services?.Dialogue;
        Flags = services?.Flags;
        OnConstructed();
    }

    protected virtual void OnConstructed() { }

    public void Interact(PlayerInteractor who)
    {
        if (Dialogue == null || Dialogue.IsRunning) return;
        Face(who.transform.position - transform.position);
        Dialogue.Run(Talk());
    }

    protected virtual IEnumerator Talk()
    {
        if (dialogue != null)
        {
            yield return Dialogue.PlayData(dialogue);
        }
        else
        {
            yield return Dialogue.Say(npcName, "Halo, petualang!");
        }
    }

    public void Face(Vector2 d)
    {
        if (body == null) return;
        body.sprite = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? (faceRight ?? faceDown) : (faceLeft ?? faceDown))
            : (d.y > 0 ? (faceUp ?? faceDown) : faceDown);
    }
}
