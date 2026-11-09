using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private TextMenu textMenu;

    private GameServices services;
    private bool isOpen;

    public void Construct(GameServices gameServices)
    {
        services = gameServices;
    }

    private void Awake()
    {
        if (menuRoot != null) menuRoot.SetActive(false);
        if (textMenu != null)
        {
            textMenu.Picked += OnPicked;
            textMenu.Cancelled += Close;
        }
    }

    private void Update()
    {
        if (services == null) return;

        // Check if in world scene and can toggle pause
        if (MenuInput.Cancel)
        {
            if (isOpen)
            {
                Close();
            }
            else if (CanOpen())
            {
                Open();
            }
        }
    }

    private bool CanOpen()
    {
        if (services == null || services.Scenes == null) return false;
        if (services.Scenes.IsBusy) return false;
        if (services.Dialogue != null && services.Dialogue.IsRunning) return false;
        if (services.Input != null && services.Input.IsLocked) return false;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneFlow.WorldScene) return false;
        return true;
    }

    public void Open()
    {
        isOpen = true;
        services?.Input?.Lock();
        if (menuRoot != null) menuRoot.SetActive(true);
        if (textMenu != null)
        {
            textMenu.Open(new[] { "Lanjut", "Simpan sekarang", "Kembali ke judul" });
        }
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        if (textMenu != null) textMenu.Close();
        if (menuRoot != null) menuRoot.SetActive(false);
        services?.Input?.Unlock();
    }

    private void OnPicked(int index)
    {
        switch (index)
        {
            case 0: // Lanjut
                Close();
                break;
            case 1: // Simpan sekarang
                Close();
                if (services != null && services.Save != null)
                {
                    Transform player = GameObject.FindWithTag("Player")?.transform;
                    Vector2 pos = player != null ? (Vector2)player.position : new Vector2(21.5f, 6f);
                    bool ok = services.Save.Save(SaveMapper.Capture(services, pos));
                    services.Toast?.Show(ok ? "Tersimpan" : "Gagal menyimpan");
                }
                break;
            case 2: // Kembali ke judul
                Close();
                services?.Scenes?.GoTo(SceneFlow.TitleScene);
                break;
        }
    }
}
