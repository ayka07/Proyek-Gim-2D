#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class DemoBuilder
{
    private const string ProjectRoot = "Assets/_Project";
    private const string ScenesRoot = ProjectRoot + "/Scenes";
    private const string DataRoot = ProjectRoot + "/Data";
    private const string PrefabsRoot = ProjectRoot + "/Prefabs";

    [MenuItem("Pulau Monster/Build Everything (P1 to P7)")]
    public static void BuildEverything()
    {
        Debug.Log("[DemoBuilder] Starting complete build of Pulau Monster...");

        ConfigureAssets();
        CreateProjectSettings();
        CreatePlayerConfig();
        CreateAnimations();
        BuildMonsterDatabase();
        BuildBootstrapScene();
        BuildTitleScene();
        BuildWorldScene();
        BuildBattleScene();
        BuildEndScene();
        SetBuildScenes();

        Debug.Log("[DemoBuilder] Pulau Monster build finished successfully!");
    }

    [MenuItem("Pulau Monster/1. Configure Asset Importers")]
    public static void ConfigureAssets()
    {
        ConfigureSprite("Assets/Art/Characters/Player_Walk_16x16.png", 16, 16, 4, 3, "Player");
        ConfigureSprite("Assets/Art/Characters/NPC_Elder_Walk_16x16.png", 16, 16, 4, 3, "NPC_Elder");
        ConfigureSprite("Assets/Art/Characters/NPC_Girl_Walk_16x16.png", 16, 16, 4, 3, "NPC_Girl");
        ConfigureSprite("Assets/Art/Characters/NPC_Guard_Walk_16x16.png", 16, 16, 4, 3, "NPC_Guard");
        ConfigureSprite("Assets/Art/Characters/NPC_Merchant_Walk_16x16.png", 16, 16, 4, 3, "NPC_Merchant");

        // Monster sprites (2 frames idle)
        ConfigureMonsterSprite("Assets/Art/Monsters/Pyro_1_Pyrit_16x16.png", 16, 16, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Pyro_2_Pyrago_24x24.png", 24, 24, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Pyro_3_Infernox_32x32.png", 32, 32, 2);

        ConfigureMonsterSprite("Assets/Art/Monsters/Aqua_1_Blubi_16x16.png", 16, 16, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Aqua_2_Finnox_24x24.png", 24, 24, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Aqua_3_Tidalord_32x32.png", 32, 32, 2);

        ConfigureMonsterSprite("Assets/Art/Monsters/Sprout_1_Sprig_16x16.png", 16, 16, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Sprout_2_Bloomlet_24x24.png", 24, 24, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Sprout_3_Florazor_32x32.png", 32, 32, 2);

        ConfigureMonsterSprite("Assets/Art/Monsters/Volt_1_Zapi_16x16.png", 16, 16, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Volt_2_Voltcat_24x24.png", 24, 24, 2);
        ConfigureMonsterSprite("Assets/Art/Monsters/Volt_3_Raijugar_32x32.png", 32, 32, 2);

        // Tiles
        ConfigureTilesSprite("Assets/Art/Tiles/Tiles_Overworld_16x16.png", 16, 16, 12, 5);

        // VFX
        ConfigureVfxSprite("Assets/Art/VFX/VFX_HitSlash_4f_32x32.png", 32, 32, 4);

        // UI Panels & Sprites
        ConfigureUiSliced("Assets/Art/UI/UI_Panel_Navy.png", 4);
        ConfigureUiSliced("Assets/Art/UI/UI_Panel_Gold.png", 4);
        ConfigureUiSliced("Assets/Art/UI/UI_Panel_Narr.png", 4);
        ConfigureSingleSprite("Assets/Art/UI/UI_Pixel.png");
        ConfigureSingleSprite("Assets/Art/UI/UI_Ring.png");
        ConfigureSingleSprite("Assets/Art/UI/Prop_SaveStone.png");
        ConfigureSingleSprite("Assets/Art/UI/UI_Alert.png");
        ConfigureSingleSprite("Assets/Art/UI/UI_Exclaim.png");

        // Backgrounds
        ConfigureSingleSprite("Assets/Art/Backgrounds/BG_Battle_Cave_320x180.png");
        ConfigureSingleSprite("Assets/Art/Backgrounds/BG_Battle_Forest_320x180.png");
        ConfigureSingleSprite("Assets/Art/Backgrounds/BG_Battle_Grassland_320x180.png");
        ConfigureSingleSprite("Assets/Art/Backgrounds/BG_Title_World.png");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[DemoBuilder] Asset Importers configured.");
    }

    private static void ConfigureSingleSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static void ConfigureUiSliced(string path, int border)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spriteBorder = new Vector4(border, border, border, border);
        importer.SaveAndReimport();
    }

    private static void ConfigureVfxSprite(string path, int frameWidth, int frameHeight, int count)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var sheet = new List<SpriteMetaData>();
        for (int i = 0; i < count; i++)
        {
            sheet.Add(new SpriteMetaData
            {
                name = Path.GetFileNameWithoutExtension(path) + "_" + i,
                rect = new Rect(i * frameWidth, 0, frameWidth, frameHeight),
                alignment = (int)SpriteAlignment.Center
            });
        }
#pragma warning disable CS0618
        importer.spritesheet = sheet.ToArray();
#pragma warning restore CS0618
        importer.SaveAndReimport();
    }

    private static void ConfigureMonsterSprite(string path, int frameWidth, int frameHeight, int count)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var sheet = new List<SpriteMetaData>();
        for (int i = 0; i < count; i++)
        {
            sheet.Add(new SpriteMetaData
            {
                name = Path.GetFileNameWithoutExtension(path) + "_" + i,
                rect = new Rect(i * frameWidth, 0, frameWidth, frameHeight),
                alignment = (int)SpriteAlignment.Center
            });
        }
#pragma warning disable CS0618
        importer.spritesheet = sheet.ToArray();
#pragma warning restore CS0618
        importer.SaveAndReimport();
    }

    private static void ConfigureTilesSprite(string path, int cellW, int cellH, int cols, int rows)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var sheet = new List<SpriteMetaData>();
        int index = 0;
        // Tiles counted row by row from top to bottom
        for (int r = 0; r < rows; r++)
        {
            int yPos = (rows - 1 - r) * cellH;
            for (int c = 0; c < cols; c++)
            {
                sheet.Add(new SpriteMetaData
                {
                    name = "Tiles_Overworld_16x16_" + index,
                    rect = new Rect(c * cellW, yPos, cellW, cellH),
                    alignment = (int)SpriteAlignment.Center
                });
                index++;
            }
        }
#pragma warning disable CS0618
        importer.spritesheet = sheet.ToArray();
#pragma warning restore CS0618
        importer.SaveAndReimport();
    }

    private static void ConfigureSprite(string path, int cellW, int cellH, int rows, int cols, string prefix)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var sheet = new List<SpriteMetaData>();
        int index = 0;
        // 4 rows: Down (top), Left, Right, Up (bottom)
        for (int r = 0; r < rows; r++)
        {
            int yPos = (rows - 1 - r) * cellH;
            for (int c = 0; c < cols; c++)
            {
                sheet.Add(new SpriteMetaData
                {
                    name = prefix + "_" + index,
                    rect = new Rect(c * cellW, yPos, cellW, cellH),
                    alignment = (int)SpriteAlignment.Center
                });
                index++;
            }
        }
#pragma warning disable CS0618
        importer.spritesheet = sheet.ToArray();
#pragma warning restore CS0618
        importer.SaveAndReimport();
    }

    [MenuItem("Pulau Monster/2. Create Project Settings (Tags, Layers, URP 2D Sorting)")]
    public static void CreateProjectSettings()
    {
        // Add layers: Player, Interactable, Wild, CameraBounds
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        SetLayer(layers, 6, "Player");
        SetLayer(layers, 7, "Interactable");
        SetLayer(layers, 8, "Wild");
        SetLayer(layers, 9, "CameraBounds");
        tagManager.ApplyModifiedProperties();

        // 2D Renderer Transparency Sort Mode = Custom Axis (0, 1, 0)
        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/Renderer2D.asset");
        if (renderer != null)
        {
            var so = new SerializedObject(renderer);
            var sortMode = so.FindProperty("m_TransparencySortMode");
            var sortAxis = so.FindProperty("m_TransparencySortAxis");
            if (sortMode != null) sortMode.intValue = (int)TransparencySortMode.CustomAxis;
            if (sortAxis != null) sortAxis.vector3Value = new Vector3(0, 1, 0);
            so.ApplyModifiedProperties();
        }

        Debug.Log("[DemoBuilder] Project Settings updated.");
    }

    private static void SetLayer(SerializedProperty layers, int index, string name)
    {
        SerializedProperty prop = layers.GetArrayElementAtIndex(index);
        if (prop != null) prop.stringValue = name;
    }

    [MenuItem("Pulau Monster/3. Create PlayerConfig")]
    public static void CreatePlayerConfig()
    {
        string dir = DataRoot;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string path = dir + "/Cfg_Player.asset";
        var cfg = AssetDatabase.LoadAssetAtPath<PlayerConfig>(path);
        if (cfg == null)
        {
            cfg = ScriptableObject.CreateInstance<PlayerConfig>();
            AssetDatabase.CreateAsset(cfg, path);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[DemoBuilder] PlayerConfig created at " + path);
    }

    [MenuItem("Pulau Monster/4. Create Animations & AC_Player")]
    public static void CreateAnimations()
    {
        string animDir = ProjectRoot + "/Animations";
        if (!Directory.Exists(animDir)) Directory.CreateDirectory(animDir);

        Sprite[] sprites = LoadSubSprites("Assets/Art/Characters/Player_Walk_16x16.png");
        if (sprites == null || sprites.Length < 12)
        {
            Debug.LogError("[DemoBuilder] Player sprites not found or not sliced!");
            return;
        }

        // Down: 0, 1, 0, 2
        // Left: 3, 4, 3, 5
        // Right: 6, 7, 6, 8
        // Up: 9, 10, 9, 11
        AnimationClip idleDown = CreateClip(animDir + "/Anim_Player_Idle_Down.anim", new[] { sprites[0] }, 8, false);
        AnimationClip idleLeft = CreateClip(animDir + "/Anim_Player_Idle_Left.anim", new[] { sprites[3] }, 8, false);
        AnimationClip idleRight = CreateClip(animDir + "/Anim_Player_Idle_Right.anim", new[] { sprites[6] }, 8, false);
        AnimationClip idleUp = CreateClip(animDir + "/Anim_Player_Idle_Up.anim", new[] { sprites[9] }, 8, false);

        AnimationClip walkDown = CreateClip(animDir + "/Anim_Player_Walk_Down.anim", new[] { sprites[0], sprites[1], sprites[0], sprites[2] }, 8, true);
        AnimationClip walkLeft = CreateClip(animDir + "/Anim_Player_Walk_Left.anim", new[] { sprites[3], sprites[4], sprites[3], sprites[5] }, 8, true);
        AnimationClip walkRight = CreateClip(animDir + "/Anim_Player_Walk_Right.anim", new[] { sprites[6], sprites[7], sprites[6], sprites[8] }, 8, true);
        AnimationClip walkUp = CreateClip(animDir + "/Anim_Player_Walk_Up.anim", new[] { sprites[9], sprites[10], sprites[9], sprites[11] }, 8, true);

        // Create AnimatorController
        string controllerPath = animDir + "/AC_Player.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.AddParameter("MoveX", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveY", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

        var rootStateMachine = controller.layers[0].stateMachine;

        // Idle BlendTree
        BlendTree idleTree;
        var idleState = controller.CreateBlendTreeInController("Idle", out idleTree, 0);
        idleTree.blendType = BlendTreeType.SimpleDirectional2D;
        idleTree.blendParameter = "MoveX";
        idleTree.blendParameterY = "MoveY";
        idleTree.AddChild(idleDown, new Vector2(0, -1));
        idleTree.AddChild(idleLeft, new Vector2(-1, 0));
        idleTree.AddChild(idleRight, new Vector2(1, 0));
        idleTree.AddChild(idleUp, new Vector2(0, 1));

        // Walk BlendTree
        BlendTree walkTree;
        var walkState = controller.CreateBlendTreeInController("Walk", out walkTree, 0);
        walkTree.blendType = BlendTreeType.SimpleDirectional2D;
        walkTree.blendParameter = "MoveX";
        walkTree.blendParameterY = "MoveY";
        walkTree.AddChild(walkDown, new Vector2(0, -1));
        walkTree.AddChild(walkLeft, new Vector2(-1, 0));
        walkTree.AddChild(walkRight, new Vector2(1, 0));
        walkTree.AddChild(walkUp, new Vector2(0, 1));

        rootStateMachine.defaultState = idleState;

        // Transitions
        var toWalk = idleState.AddTransition(walkState);
        toWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        toWalk.hasExitTime = false;
        toWalk.duration = 0f;

        var toIdle = walkState.AddTransition(idleState);
        toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        toIdle.hasExitTime = false;
        toIdle.duration = 0f;

        AssetDatabase.SaveAssets();
        Debug.Log("[DemoBuilder] Animations and AC_Player created at " + controllerPath);
    }

    private static AnimationClip CreateClip(string path, Sprite[] frames, float sampleRate, bool loop)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, path);
        }

        clip.frameRate = sampleRate;
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        var keyframes = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / sampleRate,
                value = frames[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyframes);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static Sprite[] LoadSubSprites(string path)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath(path);
        var sprites = new List<Sprite>();
        foreach (var a in assets)
        {
            if (a is Sprite s) sprites.Add(s);
        }
        return sprites.ToArray();
    }

    [MenuItem("Pulau Monster/5. Build Monster Database from CSV")]
    public static void BuildMonsterDatabase()
    {
        string monsterDataDir = DataRoot + "/Monsters";
        if (!Directory.Exists(monsterDataDir)) Directory.CreateDirectory(monsterDataDir);

        string csvPath = "Assets/Data/MonsterDatabase.csv";
        if (!File.Exists(csvPath))
        {
            Debug.LogError("MonsterDatabase.csv not found!");
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);
        var monsterMap = new Dictionary<string, MonsterData>();
        var evolvesToMap = new Dictionary<string, string>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            string[] cols = line.Split(',');
            if (cols.Length < 13) continue;

            string id = cols[0].Trim();
            string name = cols[1].Trim();
            Element elem = (Element)Enum.Parse(typeof(Element), cols[2].Trim());
            int stage = int.Parse(cols[3].Trim());
            int hp = int.Parse(cols[4].Trim());
            int atk = int.Parse(cols[5].Trim());
            int def = int.Parse(cols[6].Trim());
            int spd = int.Parse(cols[7].Trim());
            string evolvesTo = cols[8].Trim();
            int evolveLevel = string.IsNullOrEmpty(cols[9].Trim()) ? 0 : int.Parse(cols[9].Trim());
            Element strong = string.IsNullOrEmpty(cols[10].Trim()) ? elem : (Element)Enum.Parse(typeof(Element), cols[10].Trim());
            string spriteFile = cols[11].Trim();

            string assetPath = $"{monsterDataDir}/{id}_{name}.asset";
            var data = AssetDatabase.LoadAssetAtPath<MonsterData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MonsterData>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            data.Id = id;
            data.DisplayName = name;
            data.Element = elem;
            data.Stage = stage;
            data.BaseHP = hp;
            data.BaseATK = atk;
            data.BaseDEF = def;
            data.BaseSPD = spd;
            data.EvolveLevel = evolveLevel;
            data.StrongAgainst = strong;

            // Load sprites
            string spritePath = "Assets/Art/Monsters/" + spriteFile;
            data.Frames = LoadSubSprites(spritePath);
            if (data.Frames != null && data.Frames.Length > 0)
            {
                data.Portrait = data.Frames[0];
            }

            EditorUtility.SetDirty(data);
            monsterMap[name] = data;
            if (!string.IsNullOrEmpty(evolvesTo))
            {
                evolvesToMap[name] = evolvesTo;
            }
        }

        // Link evolutions
        foreach (var pair in evolvesToMap)
        {
            if (monsterMap.TryGetValue(pair.Key, out MonsterData src) &&
                monsterMap.TryGetValue(pair.Value, out MonsterData target))
            {
                src.EvolvesTo = target;
                EditorUtility.SetDirty(src);
            }
        }

        // Create MonsterDatabase
        string dbPath = DataRoot + "/MonsterDatabase.asset";
        var db = AssetDatabase.LoadAssetAtPath<MonsterDatabase>(dbPath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<MonsterDatabase>();
            AssetDatabase.CreateAsset(db, dbPath);
        }

        db.AllMonsters.Clear();
        foreach (var m in monsterMap.Values)
        {
            db.AllMonsters.Add(m);
        }
        EditorUtility.SetDirty(db);

        AssetDatabase.SaveAssets();
        Debug.Log("[DemoBuilder] Monster Database created with " + db.AllMonsters.Count + " monsters.");
    }

    [MenuItem("Pulau Monster/6. Build Scn_Bootstrap")]
    public static void BuildBootstrapScene()
    {
        string scenePath = ScenesRoot + "/Scn_Bootstrap.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Regular.ttf");
        var boldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Bold.ttf");
        var navySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Navy.png");
        var goldSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Gold.png");
        var narrSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Narr.png");
        var db = AssetDatabase.LoadAssetAtPath<MonsterDatabase>(DataRoot + "/MonsterDatabase.asset");

        // EventSystem
        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // Bootstrap Manager GameObject
        var bootGo = new GameObject("GameBootstrap");
        var bootstrap = bootGo.AddComponent<GameBootstrap>();
        var audioService = bootGo.AddComponent<AudioService>();

        // Audio Sources for AudioService
        var musicA = bootGo.AddComponent<AudioSource>();
        var musicB = bootGo.AddComponent<AudioSource>();
        var sfx = bootGo.AddComponent<AudioSource>();

        SetPrivateField(audioService, "musicSourceA", musicA);
        SetPrivateField(audioService, "musicSourceB", musicB);
        SetPrivateField(audioService, "sfxSource", sfx);

        // Canvas for Transition & Fader (Order 100)
        var canvasFaderGo = new GameObject("Canvas_Fader");
        var cFader = canvasFaderGo.AddComponent<Canvas>();
        cFader.renderMode = RenderMode.ScreenSpaceOverlay;
        cFader.sortingOrder = 100;
        var csFader = canvasFaderGo.AddComponent<CanvasScaler>();
        csFader.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        csFader.referenceResolution = new Vector2(1280, 720);
        csFader.matchWidthOrHeight = 0.5f;
        canvasFaderGo.AddComponent<GraphicRaycaster>();

        var faderImgGo = new GameObject("FaderImage");
        faderImgGo.transform.SetParent(canvasFaderGo.transform, false);
        var faderImg = faderImgGo.AddComponent<Image>();
        faderImg.color = new Color(0, 0, 0, 0);
        faderImg.rectTransform.anchorMin = Vector2.zero;
        faderImg.rectTransform.anchorMax = Vector2.one;
        faderImg.rectTransform.sizeDelta = Vector2.zero;
        faderImgGo.SetActive(false);

        var sceneFlow = bootGo.AddComponent<SceneFlow>();
        SetPrivateField(sceneFlow, "fader", faderImg);

        // Toast View Canvas (Order 50)
        var canvasToastGo = new GameObject("Canvas_Toast");
        var cToast = canvasToastGo.AddComponent<Canvas>();
        cToast.renderMode = RenderMode.ScreenSpaceOverlay;
        cToast.sortingOrder = 50;
        var csToast = canvasToastGo.AddComponent<CanvasScaler>();
        csToast.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        csToast.referenceResolution = new Vector2(1280, 720);
        csToast.matchWidthOrHeight = 0.5f;

        var toastBox = new GameObject("ToastBox");
        toastBox.transform.SetParent(canvasToastGo.transform, false);
        var toastImg = toastBox.AddComponent<Image>();
        toastImg.sprite = goldSprite;
        toastImg.type = Image.Type.Sliced;
        toastImg.pixelsPerUnitMultiplier = 1.5625f;
        var toastRect = toastBox.GetComponent<RectTransform>();
        toastRect.anchorMin = new Vector2(0.5f, 1f);
        toastRect.anchorMax = new Vector2(0.5f, 1f);
        toastRect.pivot = new Vector2(0.5f, 1f);
        toastRect.anchoredPosition = new Vector2(0, -20);
        toastRect.sizeDelta = new Vector2(600, 60);

        var toastTextGo = new GameObject("ToastText");
        toastTextGo.transform.SetParent(toastBox.transform, false);
        var toastText = toastTextGo.AddComponent<Text>();
        toastText.font = boldFont ?? font;
        toastText.fontSize = 28;
        toastText.alignment = TextAnchor.MiddleCenter;
        toastText.color = UiTheme.Ink;
        toastText.rectTransform.anchorMin = Vector2.zero;
        toastText.rectTransform.anchorMax = Vector2.one;
        toastText.rectTransform.sizeDelta = Vector2.zero;

        var toastCg = toastBox.AddComponent<CanvasGroup>();
        var toastView = bootGo.AddComponent<ToastView>();
        SetPrivateField(toastView, "container", toastBox);
        SetPrivateField(toastView, "messageText", toastText);
        SetPrivateField(toastView, "canvasGroup", toastCg);
        toastBox.SetActive(false);

        // Pause Menu Canvas (Order 40)
        var canvasPauseGo = new GameObject("Canvas_Pause");
        var cPause = canvasPauseGo.AddComponent<Canvas>();
        cPause.renderMode = RenderMode.ScreenSpaceOverlay;
        cPause.sortingOrder = 40;
        var csPause = canvasPauseGo.AddComponent<CanvasScaler>();
        csPause.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        csPause.referenceResolution = new Vector2(1280, 720);
        csPause.matchWidthOrHeight = 0.5f;
        canvasPauseGo.AddComponent<GraphicRaycaster>();

        var pauseDim = new GameObject("PauseDim");
        pauseDim.transform.SetParent(canvasPauseGo.transform, false);
        var pauseDimImg = pauseDim.AddComponent<Image>();
        pauseDimImg.color = new Color(0.07f, 0.06f, 0.15f, 0.6f);
        pauseDimImg.rectTransform.anchorMin = Vector2.zero;
        pauseDimImg.rectTransform.anchorMax = Vector2.one;

        var pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(pauseDim.transform, false);
        var pausePanelImg = pausePanel.AddComponent<Image>();
        pausePanelImg.sprite = navySprite;
        pausePanelImg.type = Image.Type.Sliced;
        pausePanelImg.pixelsPerUnitMultiplier = 1.5625f;
        var pauseRect = pausePanel.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(0.5f, 0.5f);
        pauseRect.anchorMax = new Vector2(0.5f, 0.5f);
        pauseRect.sizeDelta = new Vector2(448, 248);

        var pauseTitleGo = new GameObject("PauseTitle");
        pauseTitleGo.transform.SetParent(pausePanel.transform, false);
        var pauseTitle = pauseTitleGo.AddComponent<Text>();
        pauseTitle.font = boldFont ?? font;
        pauseTitle.fontSize = 32;
        pauseTitle.text = "MENU";
        pauseTitle.color = UiTheme.Gold;
        pauseTitle.alignment = TextAnchor.UpperCenter;
        pauseTitle.rectTransform.anchorMin = new Vector2(0, 1);
        pauseTitle.rectTransform.anchorMax = new Vector2(1, 1);
        pauseTitle.rectTransform.anchoredPosition = new Vector2(0, -20);
        pauseTitle.rectTransform.sizeDelta = new Vector2(0, 40);

        var pauseMenuComp = bootGo.AddComponent<PauseMenu>();
        var pauseMenuTextMenu = pausePanel.AddComponent<TextMenu>();

        var pauseRows = new List<Text>();
        string[] pauseLabels = { "Lanjut", "Simpan sekarang", "Kembali ke judul" };
        for (int i = 0; i < 3; i++)
        {
            var rGo = new GameObject("Row_" + i);
            rGo.transform.SetParent(pausePanel.transform, false);
            var t = rGo.AddComponent<Text>();
            t.font = font;
            t.fontSize = 28;
            t.color = UiTheme.Text;
            t.text = pauseLabels[i];
            t.rectTransform.anchorMin = new Vector2(0, 1);
            t.rectTransform.anchorMax = new Vector2(1, 1);
            t.rectTransform.anchoredPosition = new Vector2(40, -80 - i * 50);
            t.rectTransform.sizeDelta = new Vector2(-80, 40);
            pauseRows.Add(t);
        }
        SetPrivateField(pauseMenuTextMenu, "rows", pauseRows.ToArray());
        SetPrivateField(pauseMenuComp, "menuRoot", pauseDim);
        SetPrivateField(pauseMenuComp, "textMenu", pauseMenuTextMenu);
        pauseDim.SetActive(false);

        // Dialogue Canvas (Order 20, up to 110 for Narration)
        var canvasDialogueGo = new GameObject("Canvas_Dialogue");
        var cDiag = canvasDialogueGo.AddComponent<Canvas>();
        cDiag.renderMode = RenderMode.ScreenSpaceOverlay;
        cDiag.sortingOrder = 20;
        var csDiag = canvasDialogueGo.AddComponent<CanvasScaler>();
        csDiag.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        csDiag.referenceResolution = new Vector2(1280, 720);
        csDiag.matchWidthOrHeight = 0.5f;
        canvasDialogueGo.AddComponent<GraphicRaycaster>();

        var dialogueRunner = bootGo.AddComponent<DialogueRunner>();
        var dialogueView = canvasDialogueGo.AddComponent<DialogueView>();

        var diagRoot = new GameObject("DialogueRoot");
        diagRoot.transform.SetParent(canvasDialogueGo.transform, false);
        var diagRootRect = diagRoot.AddComponent<RectTransform>();
        diagRootRect.anchorMin = Vector2.zero;
        diagRootRect.anchorMax = Vector2.one;

        // ClickCatcher
        var ccGo = new GameObject("ClickCatcher");
        ccGo.transform.SetParent(diagRoot.transform, false);
        var ccImg = ccGo.AddComponent<Image>();
        ccImg.color = new Color(0, 0, 0, 0);
        var ccBtn = ccGo.AddComponent<Button>();
        ccBtn.transition = Selectable.Transition.None;
        ccImg.rectTransform.anchorMin = Vector2.zero;
        ccImg.rectTransform.anchorMax = Vector2.one;

        // Dialogue Box: 6, 128, 308, 46 (x4 -> 24, -512, 1232, 184)
        var boxGo = new GameObject("Box");
        boxGo.transform.SetParent(diagRoot.transform, false);
        var boxImg = boxGo.AddComponent<Image>();
        boxImg.sprite = navySprite;
        boxImg.type = Image.Type.Sliced;
        boxImg.pixelsPerUnitMultiplier = 1.5625f;
        var boxRect = boxGo.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0, 1);
        boxRect.anchorMax = new Vector2(0, 1);
        boxRect.pivot = new Vector2(0, 1);
        boxRect.anchoredPosition = new Vector2(24, -512);
        boxRect.sizeDelta = new Vector2(1232, 184);

        var bodyGo = new GameObject("Body");
        bodyGo.transform.SetParent(boxGo.transform, false);
        var bodyText = bodyGo.AddComponent<Text>();
        bodyText.font = font;
        bodyText.fontSize = 32;
        bodyText.color = UiTheme.Text;
        var bodyShadow = bodyGo.AddComponent<Shadow>();
        bodyShadow.effectColor = UiTheme.Narr;
        bodyShadow.effectDistance = new Vector2(2, -2);
        var bodyRect = bodyGo.GetComponent<RectTransform>();
        bodyRect.anchorMin = Vector2.zero;
        bodyRect.anchorMax = Vector2.one;
        bodyRect.offsetMin = new Vector2(30, 20);
        bodyRect.offsetMax = new Vector2(-40, -20);

        // SpeakerTag: 12, 117 (x4 -> 48, -468)
        var speakerGo = new GameObject("SpeakerTag");
        speakerGo.transform.SetParent(diagRoot.transform, false);
        var speakerImg = speakerGo.AddComponent<Image>();
        speakerImg.sprite = goldSprite;
        speakerImg.type = Image.Type.Sliced;
        speakerImg.pixelsPerUnitMultiplier = 1.5625f;
        var speakerRect = speakerGo.GetComponent<RectTransform>();
        speakerRect.anchorMin = new Vector2(0, 1);
        speakerRect.anchorMax = new Vector2(0, 1);
        speakerRect.pivot = new Vector2(0, 1);
        speakerRect.anchoredPosition = new Vector2(48, -468);
        speakerRect.sizeDelta = new Vector2(240, 50);

        var speakerTextGo = new GameObject("SpeakerText");
        speakerTextGo.transform.SetParent(speakerGo.transform, false);
        var spText = speakerTextGo.AddComponent<Text>();
        spText.font = boldFont ?? font;
        spText.fontSize = 28;
        spText.color = UiTheme.Ink;
        spText.alignment = TextAnchor.MiddleCenter;
        spText.rectTransform.anchorMin = Vector2.zero;
        spText.rectTransform.anchorMax = Vector2.one;

        // ContinueHint: "▼" at bottom right
        var hintGo = new GameObject("ContinueHint");
        hintGo.transform.SetParent(boxGo.transform, false);
        var hintT = hintGo.AddComponent<Text>();
        hintT.font = font;
        hintT.fontSize = 28;
        hintT.text = "▼";
        hintT.color = UiTheme.Gold;
        hintT.alignment = TextAnchor.LowerRight;
        hintT.rectTransform.anchorMin = new Vector2(1, 0);
        hintT.rectTransform.anchorMax = new Vector2(1, 0);
        hintT.rectTransform.pivot = new Vector2(1, 0);
        hintT.rectTransform.anchoredPosition = new Vector2(-20, 15);
        hintT.rectTransform.sizeDelta = new Vector2(40, 40);

        // Choices Menu
        var choicesGo = new GameObject("Choices");
        choicesGo.transform.SetParent(diagRoot.transform, false);
        var chImg = choicesGo.AddComponent<Image>();
        chImg.sprite = navySprite;
        chImg.type = Image.Type.Sliced;
        chImg.pixelsPerUnitMultiplier = 1.5625f;
        var chRect = choicesGo.GetComponent<RectTransform>();
        chRect.anchorMin = new Vector2(1, 1);
        chRect.anchorMax = new Vector2(1, 1);
        chRect.pivot = new Vector2(1, 1);
        chRect.anchoredPosition = new Vector2(-40, -280);
        chRect.sizeDelta = new Vector2(420, 220);

        var chMenu = choicesGo.AddComponent<TextMenu>();
        var chRows = new List<Text>();
        for (int i = 0; i < 3; i++)
        {
            var crGo = new GameObject("Choice_" + i);
            crGo.transform.SetParent(choicesGo.transform, false);
            var ct = crGo.AddComponent<Text>();
            ct.font = font;
            ct.fontSize = 28;
            ct.color = UiTheme.Text;
            ct.rectTransform.anchorMin = new Vector2(0, 1);
            ct.rectTransform.anchorMax = new Vector2(1, 1);
            ct.rectTransform.anchoredPosition = new Vector2(20, -30 - i * 60);
            ct.rectTransform.sizeDelta = new Vector2(-40, 45);
            chRows.Add(ct);
        }
        SetPrivateField(chMenu, "rows", chRows.ToArray());
        choicesGo.SetActive(false);

        // Wire dialogue view
        SetPrivateField(dialogueView, "canvas", cDiag);
        SetPrivateField(dialogueView, "dialogueRoot", diagRoot);
        SetPrivateField(dialogueView, "clickCatcher", ccBtn);
        SetPrivateField(dialogueView, "boxImage", boxImg);
        SetPrivateField(dialogueView, "bodyText", bodyText);
        SetPrivateField(dialogueView, "speakerTag", speakerGo);
        SetPrivateField(dialogueView, "speakerText", spText);
        SetPrivateField(dialogueView, "speakerTagRect", speakerRect);
        SetPrivateField(dialogueView, "continueHint", hintGo);
        SetPrivateField(dialogueView, "choicesMenu", chMenu);
        SetPrivateField(dialogueView, "navyPanelSprite", navySprite);
        SetPrivateField(dialogueView, "narrPanelSprite", narrSprite);

        SetPrivateField(dialogueRunner, "view", dialogueView);
        var blipClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Dialog_Blip.wav");
        SetPrivateField(dialogueRunner, "blipSfx", blipClip);

        diagRoot.SetActive(false);

        // Wire GameBootstrap
        SetPrivateField(bootstrap, "firstScene", SceneFlow.TitleScene);
        SetPrivateField(bootstrap, "monsterDatabase", db);
        SetPrivateField(bootstrap, "sceneFlow", sceneFlow);
        SetPrivateField(bootstrap, "audioService", audioService);
        SetPrivateField(bootstrap, "dialogueRunner", dialogueRunner);
        SetPrivateField(bootstrap, "toastView", toastView);
        SetPrivateField(bootstrap, "pauseMenu", pauseMenuComp);

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[DemoBuilder] Scn_Bootstrap saved at " + scenePath);
    }

    [MenuItem("Pulau Monster/7. Build Scn_Title")]
    public static void BuildTitleScene()
    {
        string scenePath = ScenesRoot + "/Scn_Title.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Regular.ttf");
        var boldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Bold.ttf");
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Backgrounds/BG_Title_World.png");
        var titleBgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/BGM_Title.wav");
        var db = AssetDatabase.LoadAssetAtPath<MonsterDatabase>(DataRoot + "/MonsterDatabase.asset");

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;
        cam.backgroundColor = UiTheme.Ink;
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGo.tag = "MainCamera";

        // Canvas_Title
        var canvasGo = new GameObject("Canvas_Title");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background moving image
        var bgGo = new GameObject("BG_Title");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.sprite = bgSprite;
        var bgRect = bgImg.rectTransform;
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.sizeDelta = new Vector2(1920, 1080);

        // Dark overlay #121027 alpha 0.55
        var overGo = new GameObject("DarkOverlay");
        overGo.transform.SetParent(canvasGo.transform, false);
        var overImg = overGo.AddComponent<Image>();
        overImg.color = new Color(0.07f, 0.06f, 0.15f, 0.55f);
        overImg.rectTransform.anchorMin = Vector2.zero;
        overImg.rectTransform.anchorMax = Vector2.one;

        // Title Subtitle
        var subGo = new GameObject("Subtitle");
        subGo.transform.SetParent(canvasGo.transform, false);
        var subText = subGo.AddComponent<Text>();
        subText.font = font;
        subText.fontSize = 36;
        subText.text = "PULAU";
        subText.color = UiTheme.Text;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.rectTransform.anchoredPosition = new Vector2(0, 230);
        subText.rectTransform.sizeDelta = new Vector2(600, 50);

        // Title Big Header
        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(canvasGo.transform, false);
        var titleText = titleGo.AddComponent<Text>();
        titleText.font = boldFont ?? font;
        titleText.fontSize = 80;
        titleText.text = "MONSTER";
        titleText.color = UiTheme.Gold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.rectTransform.anchoredPosition = new Vector2(0, 160);
        titleText.rectTransform.sizeDelta = new Vector2(800, 100);
        var shadow = titleGo.AddComponent<Shadow>();
        shadow.effectColor = UiTheme.Ink;
        shadow.effectDistance = new Vector2(4, -4);

        // 4 Starter Images: Pyrit, Blubi, Sprig, Zapi
        var starterImages = new List<Image>();
        var starterDatas = new List<MonsterData>();
        string[] starterNames = { "Pyrit", "Blubi", "Sprig", "Zapi" };

        var startersContainer = new GameObject("Starters");
        startersContainer.transform.SetParent(canvasGo.transform, false);
        startersContainer.AddComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);

        for (int i = 0; i < 4; i++)
        {
            var sGo = new GameObject("Starter_" + starterNames[i]);
            sGo.transform.SetParent(startersContainer.transform, false);
            var img = sGo.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(96, 96);
            img.rectTransform.anchoredPosition = new Vector2(-225 + i * 150, 0);

            MonsterData mData = db != null ? db.GetByName(starterNames[i]) : null;
            if (mData != null && mData.Frames != null && mData.Frames.Length > 0)
            {
                img.sprite = mData.Frames[0];
            }
            starterImages.Add(img);
            starterDatas.Add(mData);
        }

        // Menu
        var menuGo = new GameObject("Menu", typeof(RectTransform));
        menuGo.transform.SetParent(canvasGo.transform, false);
        var menuComp = menuGo.AddComponent<TextMenu>();
        var menuRect = menuGo.GetComponent<RectTransform>();
        menuRect.anchoredPosition = new Vector2(0, -140);
        menuRect.sizeDelta = new Vector2(400, 160);

        var rows = new List<Text>();
        string[] defaultOpts = { "Lanjutkan", "Mulai baru", "Keluar" };
        for (int i = 0; i < 3; i++)
        {
            var rGo = new GameObject("Row_" + i);
            rGo.transform.SetParent(menuGo.transform, false);
            var t = rGo.AddComponent<Text>();
            t.font = font;
            t.fontSize = 32;
            t.color = UiTheme.Text;
            t.text = defaultOpts[i];
            t.alignment = TextAnchor.MiddleCenter;
            t.rectTransform.anchoredPosition = new Vector2(0, 40 - i * 50);
            t.rectTransform.sizeDelta = new Vector2(400, 40);
            rows.Add(t);
        }
        SetPrivateField(menuComp, "rows", rows.ToArray());

        // Hint Text
        var hintGo = new GameObject("Hint");
        hintGo.transform.SetParent(canvasGo.transform, false);
        var hintText = hintGo.AddComponent<Text>();
        hintText.font = font;
        hintText.fontSize = 20;
        hintText.text = "Enter / Spasi untuk mulai · klik juga bisa";
        hintText.color = UiTheme.Muted;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.rectTransform.anchoredPosition = new Vector2(0, -310);
        hintText.rectTransform.sizeDelta = new Vector2(800, 30);

        // Installer
        var instGo = new GameObject("TitleInstaller");
        var installer = instGo.AddComponent<TitleInstaller>();
        SetPrivateField(installer, "menu", menuComp);
        SetPrivateField(installer, "titleMusic", titleBgm);
        SetPrivateField(installer, "starterImages", starterImages.ToArray());
        SetPrivateField(installer, "starterDatas", starterDatas.ToArray());
        SetPrivateField(installer, "movingBackground", bgRect);

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[DemoBuilder] Scn_Title saved at " + scenePath);
    }

    [MenuItem("Pulau Monster/8. Build Scn_World")]
    public static void BuildWorldScene()
    {
        string scenePath = ScenesRoot + "/Scn_World.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Regular.ttf");
        var boldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Bold.ttf");
        var navySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Navy.png");
        var goldSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Gold.png");
        var pixelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Pixel.png");
        var saveStoneSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Prop_SaveStone.png");
        var alertSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Alert.png");
        var db = AssetDatabase.LoadAssetAtPath<MonsterDatabase>(DataRoot + "/MonsterDatabase.asset");
        var playerCfg = AssetDatabase.LoadAssetAtPath<PlayerConfig>(DataRoot + "/Cfg_Player.asset");
        var playerAc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ProjectRoot + "/Animations/AC_Player.controller");

        var villageBgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/BGM_Village.wav");
        var forestBgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/BGM_Forest.wav");
        var saveSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Save.wav");

        // Camera with CameraFollow & PixelPerfectCamera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(21.5f, 6f, -10f);

        var camFollow = camGo.AddComponent<CameraFollow>();
        var ppCam = camGo.AddComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
        ppCam.assetsPPU = 16;
        ppCam.refResolutionX = 320;
        ppCam.refResolutionY = 180;

        // CameraBounds PolygonCollider2D trigger (0,0) (40,0) (40,36) (0,36)
        var boundsGo = new GameObject("CameraBounds");
        boundsGo.layer = LayerMask.NameToLayer("CameraBounds");
        var poly = boundsGo.AddComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.points = new[] { new Vector2(0, 0), new Vector2(40, 0), new Vector2(40, 36), new Vector2(0, 36) };

        // PixelBurst VFX pool in scene
        var burstGo = new GameObject("PixelBurst");
        var burst = burstGo.AddComponent<PixelBurst>();
        SetPrivateField(burst, "pixel", pixelSprite);

        // Tilemap Grid
        var gridGo = new GameObject("Grid");
        var grid = gridGo.AddComponent<Grid>();
        grid.cellSize = new Vector3(1, 1, 0);

        Sprite[] overworldSprites = LoadSubSprites("Assets/Art/Tiles/Tiles_Overworld_16x16.png");

        Tilemap tmGround = CreateTilemap(gridGo, "TM_Ground", 0, false);
        Tilemap tmDecor = CreateTilemap(gridGo, "TM_Decor", 1, false);
        Tilemap tmBlocking = CreateTilemap(gridGo, "TM_Blocking", 2, true);
        Tilemap tmTallGrass = CreateTilemap(gridGo, "TM_TallGrass", 3, false);

        // Build 40x36 Tilemap layout
        FillTilemapWorld(tmGround, tmDecor, tmBlocking, tmTallGrass, overworldSprites);

        // Player GameObject
        var playerGo = new GameObject("Player");
        playerGo.tag = "Player";
        playerGo.layer = LayerMask.NameToLayer("Player");
        playerGo.transform.position = new Vector3(21.5f, 6f, 0);

        var playerSr = playerGo.AddComponent<SpriteRenderer>();
        playerSr.sortingLayerName = "Characters";
        playerSr.sortingOrder = 5;
        Sprite[] playerSprites = LoadSubSprites("Assets/Art/Characters/Player_Walk_16x16.png");
        if (playerSprites != null && playerSprites.Length > 0) playerSr.sprite = playerSprites[0];

        var playerRb = playerGo.AddComponent<Rigidbody2D>();
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        playerRb.gravityScale = 0;
        playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerRb.freezeRotation = true;

        var playerCol = playerGo.AddComponent<BoxCollider2D>();
        playerCol.size = new Vector2(0.8f, 0.5f);
        playerCol.offset = new Vector2(0, -0.25f);

        var playerMovement = playerGo.AddComponent<PlayerMovement>();
        SetPrivateField(playerMovement, "config", playerCfg);
        SetPrivateField(playerMovement, "rb", playerRb);

        var playerAnimComp = playerGo.AddComponent<PlayerAnimator>();
        var anim = playerGo.GetComponent<Animator>() ?? playerGo.AddComponent<Animator>();
        anim.runtimeAnimatorController = playerAc;
        SetPrivateField(playerAnimComp, "movement", playerMovement);

        var playerInteractor = playerGo.AddComponent<PlayerInteractor>();
        SetPrivateField(playerInteractor, "movement", playerMovement);
        SetPrivateField(playerInteractor, "reach", 0.6f);
        SetPrivateField(playerInteractor, "radius", 0.3f);
        SetPrivateField(playerInteractor, "interactableMask", (LayerMask)(1 << LayerMask.NameToLayer("Interactable")));

        var grassRustle = playerGo.AddComponent<GrassRustle>();
        grassRustle.Construct(tmTallGrass, burst);

        // Partner Follower GameObject
        var partnerGo = new GameObject("Partner");
        partnerGo.transform.position = playerGo.transform.position + new Vector3(-0.8f, 0, 0);
        var partnerSr = partnerGo.AddComponent<SpriteRenderer>();
        partnerSr.sortingLayerName = "Characters";
        partnerSr.sortingOrder = 4;
        var partnerFollower = partnerGo.AddComponent<PartnerFollower>();
        SetPrivateField(partnerFollower, "target", playerGo.transform);
        SetPrivateField(partnerFollower, "spriteRenderer", partnerSr);
        partnerGo.SetActive(false);

        // NPCs
        Sprite[] elderSprites = LoadSubSprites("Assets/Art/Characters/NPC_Elder_Walk_16x16.png");
        Sprite[] girlSprites = LoadSubSprites("Assets/Art/Characters/NPC_Girl_Walk_16x16.png");
        Sprite[] guardSprites = LoadSubSprites("Assets/Art/Characters/NPC_Guard_Walk_16x16.png");
        Sprite[] merchantSprites = LoadSubSprites("Assets/Art/Characters/NPC_Merchant_Walk_16x16.png");

        // 1. Tetua Rahmat (25.5, 7) - StarterGiver
        var elderGo = CreateNpcObject<StarterGiver>("Pfb_Npc_Elder", "Tetua Rahmat", new Vector3(25.5f, 7f, 0), elderSprites);
        MonsterData pyrit = db.GetByName("Pyrit");
        MonsterData blubi = db.GetByName("Blubi");
        MonsterData sprig = db.GetByName("Sprig");
        SetPrivateField(elderGo.GetComponent<StarterGiver>(), "starters", new[] { pyrit, blubi, sprig });

        // 2. Sari (7, 7) - Npc
        var girlGo = CreateNpcObject<Npc>("Pfb_Npc_Girl", "Sari", new Vector3(7f, 7f, 0), girlSprites);
        var sariDiag = ScriptableObject.CreateInstance<DialogueData>();
        sariDiag.lines = new[]
        {
            new DialogueLine { speaker = "Sari", text = "Halo! Apa kamu sudah tahu tentang elemen monster?" },
            new DialogueLine { speaker = "Sari", text = "Api mengalahkan Tumbuhan, Tumbuhan mengalahkan Air, dan Air mengalahkan Api!" },
            new DialogueLine { speaker = "Sari", text = "Listrik sangat kuat melawan Air, tapi berhati-hatilah di dalam Gua Petir!" }
        };
        SetPrivateField(girlGo.GetComponent<Npc>(), "dialogue", sariDiag);

        // 3. Penjaga Batas (20, 11.4) - GuardNpc
        var guardGo = CreateNpcObject<GuardNpc>("Pfb_Npc_Guard", "Penjaga Batas", new Vector3(20f, 11.4f, 0), guardSprites);
        // Gate blocking collider at (20, 11.5)
        var gateColliderGo = new GameObject("Gate_Hutan");
        gateColliderGo.layer = LayerMask.NameToLayer("Default");
        gateColliderGo.transform.position = new Vector3(20f, 11.5f, 0);
        var gateCol = gateColliderGo.AddComponent<BoxCollider2D>();
        gateCol.size = new Vector2(2f, 1f);
        SetPrivateField(guardGo.GetComponent<GuardNpc>(), "gateCollider", gateColliderGo);
        SetPrivateField(guardGo.GetComponent<GuardNpc>(), "asidePosition", new Vector2(22.5f, 10.4f));

        var flagToggle = gateColliderGo.AddComponent<FlagToggle>();
        SetPrivateField(flagToggle, "flagKey", StoryFlags.HasStarter);
        SetPrivateField(flagToggle, "hideWhenSet", true);

        // 4. Pak Dagang (13, 4) - MerchantNpc
        var merchantGo = CreateNpcObject<MerchantNpc>("Pfb_Npc_Merchant", "Pak Dagang", new Vector3(13f, 4f, 0), merchantSprites);

        // 5. SaveStone at (24f, 7f, 0)
        var stoneGo = new GameObject("SaveStone");
        stoneGo.layer = LayerMask.NameToLayer("Interactable");
        stoneGo.transform.position = new Vector3(24f, 7f, 0);
        var stoneSr = stoneGo.AddComponent<SpriteRenderer>();
        stoneSr.sprite = saveStoneSprite;
        stoneSr.sortingLayerName = "Characters";
        var stoneCol = stoneGo.AddComponent<BoxCollider2D>();
        stoneCol.size = new Vector2(1f, 1f);
        var saveStoneComp = stoneGo.AddComponent<SaveStone>();
        SetPrivateField(saveStoneComp, "saveClip", saveSfx);
        SetPrivateField(saveStoneComp, "burst", burst);

        // Wild Monsters in Hutan tall grass area
        string[] wildNames = { "Pyrit", "Blubi", "Sprig", "Zapi" };
        Vector2[] wildPositions =
        {
            new Vector2(10f, 15f), new Vector2(15f, 17f), new Vector2(8f, 20f),
            new Vector2(18f, 21f), new Vector2(22f, 16f), new Vector2(12f, 23f),
            new Vector2(6f, 18f),  new Vector2(16f, 24f)
        };

        for (int i = 0; i < wildPositions.Length; i++)
        {
            string monName = wildNames[i % wildNames.Length];
            MonsterData mData = db.GetByName(monName);
            CreateWildMonsterObject("Wild_" + monName + "_" + i, mData, wildPositions[i], 2, 4, false, alertSprite);
        }

        // Boss Raijugar in Cave at (29f, 30f)
        MonsterData raijugarData = db.GetByName("Raijugar");
        var bossGo = CreateWildMonsterObject("Boss_Raijugar", raijugarData, new Vector2(29f, 30f), 9, 10, true, alertSprite);

        // 2D Lighting for Gua Petir (P7)
        var lightsContainer = new GameObject("Lights");
        var globalLightGo = new GameObject("GlobalLight2D");
        globalLightGo.transform.SetParent(lightsContainer.transform);
        var globalLight = globalLightGo.AddComponent<Light2D>();
        globalLight.lightType = Light2D.LightType.Global;
        globalLight.intensity = 1.0f;

        // Player Light
        var pLightGo = new GameObject("PlayerLight");
        pLightGo.transform.SetParent(playerGo.transform, false);
        var pLight = pLightGo.AddComponent<Light2D>();
        pLight.lightType = Light2D.LightType.Point;
        pLight.pointLightInnerRadius = 1.2f;
        pLight.pointLightOuterRadius = 3.6f;
        pLight.color = new Color32(255, 240, 200, 255); // warm cream
        pLight.intensity = 0f;

        // Boss Light on Raijugar
        var bLightGo = new GameObject("BossLight");
        bLightGo.transform.SetParent(bossGo.transform, false);
        bLightGo.transform.localPosition = new Vector3(0, 0.9f, 0);
        var bLight = bLightGo.AddComponent<Light2D>();
        bLight.lightType = Light2D.LightType.Point;
        bLight.pointLightInnerRadius = 0.3f;
        bLight.pointLightOuterRadius = 1.8f;
        bLight.color = UiTheme.Gold;
        bLight.intensity = 0f;

        // 5 Cave Crystals: (24,27), (36,28), (26,32), (35,32), (33,27)
        Vector2[] crystalPositions = { new Vector2(24f, 27f), new Vector2(36f, 28f), new Vector2(26f, 32f), new Vector2(35f, 32f), new Vector2(33f, 27f) };
        var extraLights = new List<Light2D>();
        extraLights.Add(bLight);

        for (int i = 0; i < crystalPositions.Length; i++)
        {
            var cryGo = new GameObject("Crystal_" + (int)crystalPositions[i].x + "_" + (int)crystalPositions[i].y);
            cryGo.transform.SetParent(lightsContainer.transform);
            cryGo.transform.position = crystalPositions[i];
            var l = cryGo.AddComponent<Light2D>();
            l.lightType = Light2D.LightType.Point;
            l.pointLightInnerRadius = 0.3f;
            l.pointLightOuterRadius = 1.9f;
            l.color = new Color32(255, 250, 180, 255); // light yellow
            l.intensity = 0f;
            extraLights.Add(l);
        }

        // Area Zones: Desa Awal, Hutan Rimbun, Danau Tenang, Gua Petir
        CreateAreaZone("Area_Desa", "Desa Awal", villageBgm, new Rect(0, 0, 40, 12));
        CreateAreaZone("Area_Hutan", "Hutan Rimbun", forestBgm, new Rect(0, 12, 27, 14));
        CreateAreaZone("Area_Danau", "Danau Tenang", forestBgm, new Rect(27, 12, 13, 14));
        var guaZoneGo = CreateAreaZone("Area_Gua", "Gua Petir", null, new Rect(0, 26, 40, 10));

        // LightZone on Area_Gua
        var lightZone = guaZoneGo.AddComponent<LightZone>();
        lightZone.Construct(globalLight, pLight, extraLights.ToArray());

        // Partner HUD Canvas (ScreenSpaceOverlay)
        var canvasHudGo = new GameObject("Canvas_HUD");
        var cHud = canvasHudGo.AddComponent<Canvas>();
        cHud.renderMode = RenderMode.ScreenSpaceOverlay;
        cHud.sortingOrder = 10;
        var csHud = canvasHudGo.AddComponent<CanvasScaler>();
        csHud.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        csHud.referenceResolution = new Vector2(1280, 720);
        csHud.matchWidthOrHeight = 0.5f;

        var partnerHud = BuildPartnerHudUi(canvasHudGo, navySprite, goldSprite, font, boldFont);

        // WorldInstaller
        var worldInstallerGo = new GameObject("WorldInstaller");
        var worldInstaller = worldInstallerGo.AddComponent<WorldInstaller>();
        SetPrivateField(worldInstaller, "player", playerMovement);
        SetPrivateField(worldInstaller, "interactor", playerInteractor);
        SetPrivateField(worldInstaller, "cameraFollow", camFollow);
        SetPrivateField(worldInstaller, "partnerFollower", partnerFollower);
        SetPrivateField(worldInstaller, "partnerHud", partnerHud);
        SetPrivateField(worldInstaller, "respawnPoint", new Vector2(21.5f, 6f));
        SetPrivateField(worldInstaller, "villageBgm", villageBgm);

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[DemoBuilder] Scn_World saved at " + scenePath);
    }

    private static PartnerHud BuildPartnerHudUi(GameObject canvasGo, Sprite navySprite, Sprite goldSprite, Font font, Font boldFont)
    {
        // Partner Panel: 4, 4, 112, 34 (x4 -> 16, -16, 448, 136)
        var panelGo = new GameObject("PartnerPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelImg = panelGo.AddComponent<Image>();
        panelImg.sprite = navySprite;
        panelImg.type = Image.Type.Sliced;
        panelImg.pixelsPerUnitMultiplier = 1.5625f;
        var pRect = panelGo.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0, 1);
        pRect.anchorMax = new Vector2(0, 1);
        pRect.pivot = new Vector2(0, 1);
        pRect.anchoredPosition = new Vector2(16, -16);
        pRect.sizeDelta = new Vector2(448, 136);

        // Portrait: 8, 9, 24x24 (x4 -> 32, -36, 96x96)
        var portGo = new GameObject("Portrait");
        portGo.transform.SetParent(panelGo.transform, false);
        var portImg = portGo.AddComponent<Image>();
        var portRect = portGo.GetComponent<RectTransform>();
        portRect.anchorMin = new Vector2(0, 1);
        portRect.anchorMax = new Vector2(0, 1);
        portRect.pivot = new Vector2(0, 1);
        portRect.anchoredPosition = new Vector2(32, -20);
        portRect.sizeDelta = new Vector2(96, 96);

        // Name Text
        var nameGo = new GameObject("NameText");
        nameGo.transform.SetParent(panelGo.transform, false);
        var nameText = nameGo.AddComponent<Text>();
        nameText.font = boldFont ?? font;
        nameText.fontSize = 28;
        nameText.color = UiTheme.Text;
        nameText.rectTransform.anchorMin = new Vector2(0, 1);
        nameText.rectTransform.anchorMax = new Vector2(0, 1);
        nameText.rectTransform.pivot = new Vector2(0, 1);
        nameText.rectTransform.anchoredPosition = new Vector2(144, -16);
        nameText.rectTransform.sizeDelta = new Vector2(180, 36);

        // Level Text
        var lvlGo = new GameObject("LevelText");
        lvlGo.transform.SetParent(panelGo.transform, false);
        var lvlText = lvlGo.AddComponent<Text>();
        lvlText.font = font;
        lvlText.fontSize = 26;
        lvlText.color = UiTheme.Gold;
        lvlText.alignment = TextAnchor.UpperRight;
        lvlText.rectTransform.anchorMin = new Vector2(0, 1);
        lvlText.rectTransform.anchorMax = new Vector2(0, 1);
        lvlText.rectTransform.pivot = new Vector2(0, 1);
        lvlText.rectTransform.anchoredPosition = new Vector2(320, -16);
        lvlText.rectTransform.sizeDelta = new Vector2(100, 36);

        // HP Bar Bg + Fill
        var hpBgGo = new GameObject("HpBarBg");
        hpBgGo.transform.SetParent(panelGo.transform, false);
        var hpBg = hpBgGo.AddComponent<Image>();
        hpBg.color = new Color(0.04f, 0.03f, 0.09f, 1f);
        var hpBgRect = hpBgGo.GetComponent<RectTransform>();
        hpBgRect.anchorMin = new Vector2(0, 1);
        hpBgRect.anchorMax = new Vector2(0, 1);
        hpBgRect.pivot = new Vector2(0, 1);
        hpBgRect.anchoredPosition = new Vector2(144, -60);
        hpBgRect.sizeDelta = new Vector2(200, 24);

        var hpFillGo = new GameObject("HpFill");
        hpFillGo.transform.SetParent(hpBgGo.transform, false);
        var hpFill = hpFillGo.AddComponent<Image>();
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        hpFill.fillOrigin = 0;
        hpFill.fillAmount = 1f;
        hpFill.color = HpColors.High;
        var hpFillRect = hpFillGo.GetComponent<RectTransform>();
        hpFillRect.anchorMin = Vector2.zero;
        hpFillRect.anchorMax = Vector2.one;
        hpFillRect.offsetMin = new Vector2(2, 2);
        hpFillRect.offsetMax = new Vector2(-2, -2);

        // HP Text
        var hpTextGo = new GameObject("HpText");
        hpTextGo.transform.SetParent(panelGo.transform, false);
        var hpText = hpTextGo.AddComponent<Text>();
        hpText.font = font;
        hpText.fontSize = 20;
        hpText.color = UiTheme.Text;
        hpText.alignment = TextAnchor.UpperRight;
        hpText.rectTransform.anchorMin = new Vector2(0, 1);
        hpText.rectTransform.anchorMax = new Vector2(0, 1);
        hpText.rectTransform.pivot = new Vector2(0, 1);
        hpText.rectTransform.anchoredPosition = new Vector2(350, -58);
        hpText.rectTransform.sizeDelta = new Vector2(80, 30);

        // EXP Bar Bg + Fill
        var expBgGo = new GameObject("ExpBarBg");
        expBgGo.transform.SetParent(panelGo.transform, false);
        var expBg = expBgGo.AddComponent<Image>();
        expBg.color = new Color(0.04f, 0.03f, 0.09f, 1f);
        var expBgRect = expBgGo.GetComponent<RectTransform>();
        expBgRect.anchorMin = new Vector2(0, 1);
        expBgRect.anchorMax = new Vector2(0, 1);
        expBgRect.pivot = new Vector2(0, 1);
        expBgRect.anchoredPosition = new Vector2(144, -96);
        expBgRect.sizeDelta = new Vector2(280, 14);

        var expFillGo = new GameObject("ExpFill");
        expFillGo.transform.SetParent(expBgGo.transform, false);
        var expFill = expFillGo.AddComponent<Image>();
        expFill.type = Image.Type.Filled;
        expFill.fillMethod = Image.FillMethod.Horizontal;
        expFill.fillOrigin = 0;
        expFill.fillAmount = 0.5f;
        expFill.color = UiTheme.Exp;
        var expFillRect = expFillGo.GetComponent<RectTransform>();
        expFillRect.anchorMin = Vector2.zero;
        expFillRect.anchorMax = Vector2.one;
        expFillRect.offsetMin = new Vector2(1, 1);
        expFillRect.offsetMax = new Vector2(-1, -1);

        // Area & Potion Panel: 226, 4, 90, 24 (x4 -> 904, -16, 360, 96)
        var areaPanelGo = new GameObject("AreaPanel");
        areaPanelGo.transform.SetParent(canvasGo.transform, false);
        var areaPanelImg = areaPanelGo.AddComponent<Image>();
        areaPanelImg.sprite = navySprite;
        areaPanelImg.type = Image.Type.Sliced;
        areaPanelImg.pixelsPerUnitMultiplier = 1.5625f;
        var apRect = areaPanelGo.GetComponent<RectTransform>();
        apRect.anchorMin = new Vector2(1, 1);
        apRect.anchorMax = new Vector2(1, 1);
        apRect.pivot = new Vector2(1, 1);
        apRect.anchoredPosition = new Vector2(-16, -16);
        apRect.sizeDelta = new Vector2(360, 96);

        var areaTextGo = new GameObject("AreaText");
        areaTextGo.transform.SetParent(areaPanelGo.transform, false);
        var areaText = areaTextGo.AddComponent<Text>();
        areaText.font = boldFont ?? font;
        areaText.fontSize = 26;
        areaText.text = "Desa Awal";
        areaText.color = UiTheme.Gold;
        areaText.rectTransform.anchorMin = new Vector2(0, 1);
        areaText.rectTransform.anchorMax = new Vector2(1, 1);
        areaText.rectTransform.anchoredPosition = new Vector2(20, -16);
        areaText.rectTransform.sizeDelta = new Vector2(-40, 32);

        var potionTextGo = new GameObject("PotionText");
        potionTextGo.transform.SetParent(areaPanelGo.transform, false);
        var potionText = potionTextGo.AddComponent<Text>();
        potionText.font = font;
        potionText.fontSize = 24;
        potionText.text = "Ramuan ×3";
        potionText.color = UiTheme.Text;
        potionText.rectTransform.anchorMin = new Vector2(0, 1);
        potionText.rectTransform.anchorMax = new Vector2(1, 1);
        potionText.rectTransform.anchoredPosition = new Vector2(20, -52);
        potionText.rectTransform.sizeDelta = new Vector2(-40, 32);

        var hudComp = canvasGo.AddComponent<PartnerHud>();
        SetPrivateField(hudComp, "partnerPanel", panelGo);
        SetPrivateField(hudComp, "portraitImage", portImg);
        SetPrivateField(hudComp, "nameText", nameText);
        SetPrivateField(hudComp, "levelText", lvlText);
        SetPrivateField(hudComp, "hpFill", hpFill);
        SetPrivateField(hudComp, "hpText", hpText);
        SetPrivateField(hudComp, "expFill", expFill);
        SetPrivateField(hudComp, "areaText", areaText);
        SetPrivateField(hudComp, "potionText", potionText);

        return hudComp;
    }

    private static GameObject CreateAreaZone(string objName, string areaName, AudioClip bgm, Rect rect)
    {
        var go = new GameObject(objName);
        go.transform.position = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, 0);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(rect.width, rect.height);
        var zone = go.AddComponent<AreaZone>();
        SetPrivateField(zone, "areaName", areaName);
        SetPrivateField(zone, "areaBgm", bgm);
        return go;
    }

    private static GameObject CreateWildMonsterObject(string objName, MonsterData data, Vector2 pos, int minLvl, int maxLvl, bool isBoss, Sprite alertSprite)
    {
        var go = new GameObject(objName);
        go.transform.position = new Vector3(pos.x, pos.y, 0);
        go.layer = LayerMask.NameToLayer("Wild");

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Characters";
        sr.sortingOrder = 3;
        if (data != null && data.Frames != null && data.Frames.Length > 0)
        {
            sr.sprite = data.Frames[0];
        }

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1f, 1f);

        var alertGo = new GameObject("AlertIcon");
        alertGo.transform.SetParent(go.transform, false);
        alertGo.transform.localPosition = new Vector3(0, 0.9f, 0);
        var alertSr = alertGo.AddComponent<SpriteRenderer>();
        alertSr.sprite = alertSprite;
        alertSr.sortingLayerName = "Characters";
        alertSr.sortingOrder = 10;
        alertGo.SetActive(false);

        var comp = go.AddComponent<WildMonster>();
        SetPrivateField(comp, "monsterData", data);
        SetPrivateField(comp, "minLevel", minLvl);
        SetPrivateField(comp, "maxLevel", maxLvl);
        SetPrivateField(comp, "isBoss", isBoss);
        SetPrivateField(comp, "alertIcon", alertGo);
        SetPrivateField(comp, "body", sr);
        SetPrivateField(comp, "detectRadius", isBoss ? 2f : 3.5f);
        SetPrivateField(comp, "wanderRadius", isBoss ? 0f : 2.5f);

        return go;
    }

    private static GameObject CreateNpcObject<T>(string prefabName, string name, Vector3 pos, Sprite[] sprites) where T : Npc
    {
        var go = new GameObject(name);
        go.transform.position = pos;
        go.layer = LayerMask.NameToLayer("Interactable");

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Characters";
        sr.sortingOrder = 3;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 0.5f);
        col.offset = new Vector2(0, -0.25f);

        var npc = go.AddComponent<T>();
        SetPrivateField(npc, "npcName", name);
        SetPrivateField(npc, "body", sr);

        if (sprites != null && sprites.Length >= 10)
        {
            sr.sprite = sprites[0];
            SetPrivateField(npc, "faceDown", sprites[0]);
            SetPrivateField(npc, "faceLeft", sprites[3]);
            SetPrivateField(npc, "faceRight", sprites[6]);
            SetPrivateField(npc, "faceUp", sprites[9]);
        }

        return go;
    }

    private static Tilemap CreateTilemap(GameObject parent, string name, int order, bool hasCollider)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tm = go.AddComponent<Tilemap>();
        var tmr = go.AddComponent<TilemapRenderer>();
        tmr.sortingLayerName = "Default";
        tmr.sortingOrder = order;

        if (hasCollider)
        {
            var col = go.AddComponent<TilemapCollider2D>();
            var comp = go.AddComponent<CompositeCollider2D>();
            col.compositeOperation = Collider2D.CompositeOperation.Merge;
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Static;
        }

        return tm;
    }

    private static void FillTilemapWorld(Tilemap tmGround, Tilemap tmDecor, Tilemap tmBlocking, Tilemap tmTallGrass, Sprite[] tiles)
    {
        if (tiles == null || tiles.Length < 60) return;

        // Build tile instances for key indices
        Tile grassTile = CreateTile(tiles[10]);     // Grass_A
        Tile pathTile = CreateTile(tiles[13]);      // Path_C
        Tile waterTile = CreateTile(tiles[18]);     // Water_C
        Tile tallGrassTile = CreateTile(tiles[27]); // TallGrass_A
        Tile treeTile = CreateTile(tiles[36]);      // Tree_Round
        Tile fenceTile = CreateTile(tiles[41]);     // Fence_H
        Tile caveFloor = CreateTile(tiles[43]);     // Cave_Floor_A
        Tile caveWall = CreateTile(tiles[45]);      // Cave_Wall

        // Map layout: 40 wide (x: 0..39), 36 high (y: 0..35)
        // Y: 0..11 -> Desa Awal
        // Y: 12..25 -> Hutan Rimbun (left) & Danau (right)
        // Y: 26..35 -> Gua Petir
        for (int x = 0; x < 40; x++)
        {
            for (int y = 0; y < 36; y++)
            {
                var pos = new Vector3Int(x, y, 0);

                if (y >= 26)
                {
                    // Cave area
                    if (x == 0 || x == 39 || y == 35 || (y == 26 && x != 20 && x != 21))
                    {
                        tmBlocking.SetTile(pos, caveWall);
                    }
                    else
                    {
                        tmGround.SetTile(pos, caveFloor);
                    }
                }
                else
                {
                    // Ground layer
                    tmGround.SetTile(pos, grassTile);

                    // Map borders blocking
                    if (x == 0 || x == 39 || y == 0)
                    {
                        tmBlocking.SetTile(pos, treeTile);
                    }

                    // Water in Lake (Danau Tenang: x: 27..38, y: 13..24)
                    if (x >= 28 && x <= 37 && y >= 14 && y <= 23)
                    {
                        tmBlocking.SetTile(pos, waterTile);
                    }
                    // Tall grass in Forest (x: 3..24, y: 13..24)
                    else if (x >= 4 && x <= 24 && y >= 14 && y <= 24)
                    {
                        tmTallGrass.SetTile(pos, tallGrassTile);
                    }

                    // Village roads
                    if ((x >= 18 && x <= 22 && y <= 12) || (y >= 5 && y <= 7 && x >= 5 && x <= 34))
                    {
                        tmDecor.SetTile(pos, pathTile);
                    }

                    // Fence separating Village and Forest at y = 11
                    if (y == 11 && x >= 2 && x <= 37)
                    {
                        if (x != 20 && x != 21) // opening for guard
                        {
                            tmBlocking.SetTile(pos, fenceTile);
                        }
                    }
                }
            }
        }
    }

    private static Tile CreateTile(Sprite s)
    {
        var t = ScriptableObject.CreateInstance<Tile>();
        t.sprite = s;
        return t;
    }

    [MenuItem("Pulau Monster/9. Build Scn_Battle")]
    public static void BuildBattleScene()
    {
        string scenePath = ScenesRoot + "/Scn_Battle.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Regular.ttf");
        var boldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Bold.ttf");
        var navySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Panel_Navy.png");
        var forestBg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Backgrounds/BG_Battle_Forest_320x180.png");
        var caveBg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Backgrounds/BG_Battle_Cave_320x180.png");
        var ringSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Ring.png");
        var pixelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/UI_Pixel.png");

        var battleBgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/BGM_Battle.wav");
        var bossBgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/BGM_Boss.wav");
        var hitSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Hit.wav");
        var critSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Hit_Critical.wav");
        var missSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Miss.wav");
        var faintSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Faint.wav");
        var vicSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Victory_Jingle.wav");
        var lvlSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_LevelUp.wav");
        var evoSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Evolve.wav");
        var healSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Heal.wav");

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;
        camGo.tag = "MainCamera";

        // Background SpriteRenderer
        var bgGo = new GameObject("BattleBackground");
        var bgSr = bgGo.AddComponent<SpriteRenderer>();
        bgSr.sprite = forestBg;
        bgSr.sortingOrder = -10;

        // PixelBurst
        var burstGo = new GameObject("PixelBurst");
        var burst = burstGo.AddComponent<PixelBurst>();
        SetPrivateField(burst, "pixel", pixelSprite);

        // VfxPool with HitSlash
        var slashPrefabGo = new GameObject("Pfb_Vfx_HitSlash");
        var slashSr = slashPrefabGo.AddComponent<SpriteRenderer>();
        slashSr.sortingLayerName = "Characters";
        slashSr.sortingOrder = 20;
        var slashVfx = slashPrefabGo.AddComponent<SpriteVfx>();
        Sprite[] slashSprites = LoadSubSprites("Assets/Art/VFX/VFX_HitSlash_4f_32x32.png");
        SetPrivateField(slashVfx, "frames", slashSprites);
        SetPrivateField(slashVfx, "fps", 18f);
        slashPrefabGo.SetActive(false);

        var vfxPoolGo = new GameObject("VfxPool");
        var vfxPool = vfxPoolGo.AddComponent<VfxPool>();
        SetPrivateField(vfxPool, "prefab", slashVfx);
        SetPrivateField(vfxPool, "initialSize", 4);

        // Enemy Monster GameObject: x = 4.5, y = -0.125
        var enemyMonGo = new GameObject("EnemyMon");
        enemyMonGo.transform.position = new Vector3(4.5f, -0.125f, 0);
        var enemySr = enemyMonGo.AddComponent<SpriteRenderer>();
        enemySr.sortingLayerName = "Characters";
        enemySr.sortingOrder = 5;

        // Player Monster GameObject: x = -4.5, y = -2.375, flipX
        var playerMonGo = new GameObject("PlayerMon");
        playerMonGo.transform.position = new Vector3(-4.5f, -2.375f, 0);
        var playerSr = playerMonGo.AddComponent<SpriteRenderer>();
        playerSr.flipX = true;
        playerSr.sortingLayerName = "Characters";
        playerSr.sortingOrder = 5;

        // Canvas_Battle (1280x720)
        var canvasGo = new GameObject("Canvas_Battle");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var battleView = canvasGo.AddComponent<BattleView>();

        // EnemyPanel (8, 8, 128, 30 -> x4: 32, -32, 512, 120)
        var enemyPanelGo = new GameObject("EnemyPanel");
        enemyPanelGo.transform.SetParent(canvasGo.transform, false);
        var epImg = enemyPanelGo.AddComponent<Image>();
        epImg.sprite = navySprite;
        epImg.type = Image.Type.Sliced;
        epImg.pixelsPerUnitMultiplier = 1.5625f;
        var epRect = enemyPanelGo.GetComponent<RectTransform>();
        epRect.anchorMin = new Vector2(0, 1);
        epRect.anchorMax = new Vector2(0, 1);
        epRect.pivot = new Vector2(0, 1);
        epRect.anchoredPosition = new Vector2(32, -32);
        epRect.sizeDelta = new Vector2(512, 120);

        var epComp = enemyPanelGo.AddComponent<MonsterPanel>();
        SetPrivateField(epComp, "body", enemySr);
        SetPrivateField(epComp, "faceRight", false);
        BuildPanelContents(enemyPanelGo, epComp, font, boldFont, false);

        // PlayerPanel (186, 92, 128, 36 -> x4: 744, -368, 512, 144)
        var playerPanelGo = new GameObject("PlayerPanel");
        playerPanelGo.transform.SetParent(canvasGo.transform, false);
        var ppImg = playerPanelGo.AddComponent<Image>();
        ppImg.sprite = navySprite;
        ppImg.type = Image.Type.Sliced;
        ppImg.pixelsPerUnitMultiplier = 1.5625f;
        var ppRect = playerPanelGo.GetComponent<RectTransform>();
        ppRect.anchorMin = new Vector2(0, 1);
        ppRect.anchorMax = new Vector2(0, 1);
        ppRect.pivot = new Vector2(0, 1);
        ppRect.anchoredPosition = new Vector2(744, -368);
        ppRect.sizeDelta = new Vector2(512, 144);

        var ppComp = playerPanelGo.AddComponent<MonsterPanel>();
        SetPrivateField(ppComp, "body", playerSr);
        SetPrivateField(ppComp, "faceRight", true);
        BuildPanelContents(playerPanelGo, ppComp, font, boldFont, true);

        // MessagePanel (0, 132, 196, 48 -> x4: 0, -528, 784, 192)
        var msgGo = new GameObject("MessagePanel");
        msgGo.transform.SetParent(canvasGo.transform, false);
        var msgImg = msgGo.AddComponent<Image>();
        msgImg.sprite = navySprite;
        msgImg.type = Image.Type.Sliced;
        msgImg.pixelsPerUnitMultiplier = 1.5625f;
        var msgRect = msgGo.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 1);
        msgRect.anchorMax = new Vector2(0, 1);
        msgRect.pivot = new Vector2(0, 1);
        msgRect.anchoredPosition = new Vector2(0, -528);
        msgRect.sizeDelta = new Vector2(784, 192);

        var msgTextGo = new GameObject("MessageText");
        msgTextGo.transform.SetParent(msgGo.transform, false);
        var msgText = msgTextGo.AddComponent<Text>();
        msgText.font = font;
        msgText.fontSize = 32;
        msgText.color = UiTheme.Text;
        msgText.rectTransform.anchorMin = Vector2.zero;
        msgText.rectTransform.anchorMax = Vector2.one;
        msgText.rectTransform.offsetMin = new Vector2(30, 40);
        msgText.rectTransform.offsetMax = new Vector2(-30, -20);

        var hintTextGo = new GameObject("SkillHint");
        hintTextGo.transform.SetParent(msgGo.transform, false);
        var hintText = hintTextGo.AddComponent<Text>();
        hintText.font = font;
        hintText.fontSize = 24;
        hintText.color = UiTheme.Gold;
        hintText.rectTransform.anchorMin = new Vector2(0, 0);
        hintText.rectTransform.anchorMax = new Vector2(1, 0);
        hintText.rectTransform.pivot = new Vector2(0, 0);
        hintText.rectTransform.anchoredPosition = new Vector2(30, 15);
        hintText.rectTransform.sizeDelta = new Vector2(-60, 30);

        // MenuPanel (196, 132, 124, 48 -> x4: 784, -528, 496, 192)
        var menuGo = new GameObject("MenuPanel");
        menuGo.transform.SetParent(canvasGo.transform, false);
        var menuImg = menuGo.AddComponent<Image>();
        menuImg.sprite = navySprite;
        menuImg.type = Image.Type.Sliced;
        menuImg.pixelsPerUnitMultiplier = 1.5625f;
        var menuRect = menuGo.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0, 1);
        menuRect.anchorMax = new Vector2(0, 1);
        menuRect.pivot = new Vector2(0, 1);
        menuRect.anchoredPosition = new Vector2(784, -528);
        menuRect.sizeDelta = new Vector2(496, 192);

        var textMenu = menuGo.AddComponent<TextMenu>();
        SetPrivateField(textMenu, "columns", 2);

        var bRows = new List<Text>();
        // 2x2 grid buttons: (col * 240, -row * 70)
        string[] btnNames = { "Serang", "Jurus (3)", "Ramuan (3)", "Kabur" };
        for (int i = 0; i < 4; i++)
        {
            int col = i % 2;
            int row = i / 2;
            var bGo = new GameObject("Btn_" + i);
            bGo.transform.SetParent(menuGo.transform, false);
            var bt = bGo.AddComponent<Text>();
            bt.font = font;
            bt.fontSize = 28;
            bt.color = UiTheme.Text;
            bt.text = btnNames[i];
            bt.rectTransform.anchorMin = new Vector2(0, 1);
            bt.rectTransform.anchorMax = new Vector2(0, 1);
            bt.rectTransform.pivot = new Vector2(0, 1);
            bt.rectTransform.anchoredPosition = new Vector2(30 + col * 230, -30 - row * 70);
            bt.rectTransform.sizeDelta = new Vector2(220, 50);
            bRows.Add(bt);
        }
        SetPrivateField(textMenu, "rows", bRows.ToArray());

        // Screen Effects: Flash, Wipe, Rings
        var flashGo = new GameObject("Flash");
        flashGo.transform.SetParent(canvasGo.transform, false);
        var flashImg = flashGo.AddComponent<Image>();
        flashImg.color = new Color(1, 1, 1, 0);
        flashImg.rectTransform.anchorMin = Vector2.zero;
        flashImg.rectTransform.anchorMax = Vector2.one;

        var wipeTGo = new GameObject("WipeTop");
        wipeTGo.transform.SetParent(canvasGo.transform, false);
        var wipeT = wipeTGo.AddComponent<Image>();
        wipeT.color = UiTheme.Ink;
        wipeTGo.SetActive(false);

        var wipeBGo = new GameObject("WipeBottom");
        wipeBGo.transform.SetParent(canvasGo.transform, false);
        var wipeB = wipeBGo.AddComponent<Image>();
        wipeB.color = UiTheme.Ink;
        wipeBGo.SetActive(false);

        var rings = new List<Image>();
        for (int i = 0; i < 3; i++)
        {
            var rGo = new GameObject("Ring_" + i);
            rGo.transform.SetParent(canvasGo.transform, false);
            var rImg = rGo.AddComponent<Image>();
            rImg.sprite = ringSprite;
            rImg.color = UiTheme.Gold;
            rImg.rectTransform.sizeDelta = new Vector2(256, 256);
            rImg.rectTransform.anchoredPosition = new Vector2(300, -100);
            rGo.SetActive(false);
            rings.Add(rImg);
        }

        // Wire BattleView
        SetPrivateField(battleView, "enemyPanel", epComp);
        SetPrivateField(battleView, "playerPanel", ppComp);
        SetPrivateField(battleView, "messageText", msgText);
        SetPrivateField(battleView, "hintText", hintText);
        SetPrivateField(battleView, "menu", textMenu);
        SetPrivateField(battleView, "flashImage", flashImg);
        SetPrivateField(battleView, "wipeTop", wipeT);
        SetPrivateField(battleView, "wipeBottom", wipeB);
        SetPrivateField(battleView, "ringImages", rings.ToArray());

        // Battle Manager GameObject
        var mgrGo = new GameObject("BattleManager");
        var battleSystem = mgrGo.AddComponent<BattleSystem>();
        var battleAudio = mgrGo.AddComponent<BattleAudio>();
        SetPrivateField(battleAudio, "hitSfx", hitSfx);
        SetPrivateField(battleAudio, "hitCriticalSfx", critSfx);
        SetPrivateField(battleAudio, "missSfx", missSfx);
        SetPrivateField(battleAudio, "faintSfx", faintSfx);
        SetPrivateField(battleAudio, "victorySfx", vicSfx);
        SetPrivateField(battleAudio, "levelUpSfx", lvlSfx);
        SetPrivateField(battleAudio, "evolveSfx", evoSfx);
        SetPrivateField(battleAudio, "healSfx", healSfx);

        var battleFeel = mgrGo.AddComponent<BattleFeel>();
        SetPrivateField(battleFeel, "cameraRig", camGo.transform);
        SetPrivateField(battleFeel, "playerBody", playerSr);
        SetPrivateField(battleFeel, "enemyBody", enemySr);
        SetPrivateField(battleFeel, "hitVfx", vfxPool);
        SetPrivateField(battleFeel, "burst", burst);

        var installer = mgrGo.AddComponent<BattleInstaller>();
        SetPrivateField(installer, "backgroundRenderer", bgSr);
        SetPrivateField(installer, "caveBackground", caveBg);
        SetPrivateField(installer, "forestBackground", forestBg);
        SetPrivateField(installer, "battleSystem", battleSystem);
        SetPrivateField(installer, "battleView", battleView);
        SetPrivateField(installer, "battleAudio", battleAudio);
        SetPrivateField(installer, "battleFeel", battleFeel);
        SetPrivateField(installer, "battleBgm", battleBgm);
        SetPrivateField(installer, "bossBgm", bossBgm);

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[DemoBuilder] Scn_Battle saved at " + scenePath);
    }

    private static void BuildPanelContents(GameObject panelGo, MonsterPanel panelComp, Font font, Font boldFont, bool isPlayer)
    {
        // Name
        var nameGo = new GameObject("Name");
        nameGo.transform.SetParent(panelGo.transform, false);
        var nt = nameGo.AddComponent<Text>();
        nt.font = boldFont ?? font;
        nt.fontSize = 28;
        nt.color = UiTheme.Text;
        nt.rectTransform.anchorMin = new Vector2(0, 1);
        nt.rectTransform.anchorMax = new Vector2(0, 1);
        nt.rectTransform.pivot = new Vector2(0, 1);
        nt.rectTransform.anchoredPosition = new Vector2(20, -14);
        nt.rectTransform.sizeDelta = new Vector2(250, 36);

        // Level
        var lvlGo = new GameObject("Level");
        lvlGo.transform.SetParent(panelGo.transform, false);
        var lt = lvlGo.AddComponent<Text>();
        lt.font = font;
        lt.fontSize = 26;
        lt.color = UiTheme.Gold;
        lt.alignment = TextAnchor.UpperRight;
        lt.rectTransform.anchorMin = new Vector2(1, 1);
        lt.rectTransform.anchorMax = new Vector2(1, 1);
        lt.rectTransform.pivot = new Vector2(1, 1);
        lt.rectTransform.anchoredPosition = new Vector2(-20, -14);
        lt.rectTransform.sizeDelta = new Vector2(120, 36);

        // Element label (for enemy)
        Text elemText = null;
        if (!isPlayer)
        {
            var elGo = new GameObject("Element");
            elGo.transform.SetParent(panelGo.transform, false);
            elemText = elGo.AddComponent<Text>();
            elemText.font = font;
            elemText.fontSize = 22;
            elemText.color = UiTheme.Muted;
            elemText.rectTransform.anchorMin = new Vector2(0, 1);
            elemText.rectTransform.anchorMax = new Vector2(0, 1);
            elemText.rectTransform.pivot = new Vector2(0, 1);
            elemText.rectTransform.anchoredPosition = new Vector2(20, -50);
            elemText.rectTransform.sizeDelta = new Vector2(200, 30);
        }

        // HP Bar Bg + Fill
        var hpBgGo = new GameObject("HpBarBg");
        hpBgGo.transform.SetParent(panelGo.transform, false);
        var hpBg = hpBgGo.AddComponent<Image>();
        hpBg.color = new Color(0.04f, 0.03f, 0.09f, 1f);
        var hpBgRect = hpBgGo.GetComponent<RectTransform>();
        hpBgRect.anchorMin = new Vector2(0, 1);
        hpBgRect.anchorMax = new Vector2(0, 1);
        hpBgRect.pivot = new Vector2(0, 1);
        hpBgRect.anchoredPosition = new Vector2(20, isPlayer ? -55 : -80);
        hpBgRect.sizeDelta = new Vector2(470, 24);

        var hpFillGo = new GameObject("HpFill");
        hpFillGo.transform.SetParent(hpBgGo.transform, false);
        var hpFill = hpFillGo.AddComponent<Image>();
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        hpFill.fillOrigin = 0;
        hpFill.fillAmount = 1f;
        hpFill.color = HpColors.High;
        var hpFillRect = hpFillGo.GetComponent<RectTransform>();
        hpFillRect.anchorMin = Vector2.zero;
        hpFillRect.anchorMax = Vector2.one;
        hpFillRect.offsetMin = new Vector2(2, 2);
        hpFillRect.offsetMax = new Vector2(-2, -2);

        Text hpT = null;
        Image expF = null;

        if (isPlayer)
        {
            var hpTextGo = new GameObject("HpText");
            hpTextGo.transform.SetParent(panelGo.transform, false);
            hpT = hpTextGo.AddComponent<Text>();
            hpT.font = font;
            hpT.fontSize = 20;
            hpT.color = UiTheme.Text;
            hpT.alignment = TextAnchor.UpperRight;
            hpT.rectTransform.anchorMin = new Vector2(1, 1);
            hpT.rectTransform.anchorMax = new Vector2(1, 1);
            hpT.rectTransform.pivot = new Vector2(1, 1);
            hpT.rectTransform.anchoredPosition = new Vector2(-20, -82);
            hpT.rectTransform.sizeDelta = new Vector2(150, 25);

            var expBgGo = new GameObject("ExpBarBg");
            expBgGo.transform.SetParent(panelGo.transform, false);
            var expBg = expBgGo.AddComponent<Image>();
            expBg.color = new Color(0.04f, 0.03f, 0.09f, 1f);
            var expBgRect = expBgGo.GetComponent<RectTransform>();
            expBgRect.anchorMin = new Vector2(0, 1);
            expBgRect.anchorMax = new Vector2(0, 1);
            expBgRect.pivot = new Vector2(0, 1);
            expBgRect.anchoredPosition = new Vector2(20, -112);
            expBgRect.sizeDelta = new Vector2(470, 14);

            var expFillGo = new GameObject("ExpFill");
            expFillGo.transform.SetParent(expBgGo.transform, false);
            expF = expFillGo.AddComponent<Image>();
            expF.type = Image.Type.Filled;
            expF.fillMethod = Image.FillMethod.Horizontal;
            expF.fillOrigin = 0;
            expF.fillAmount = 0.5f;
            expF.color = UiTheme.Exp;
            var expFillRect = expFillGo.GetComponent<RectTransform>();
            expFillRect.anchorMin = Vector2.zero;
            expFillRect.anchorMax = Vector2.one;
            expFillRect.offsetMin = new Vector2(1, 1);
            expFillRect.offsetMax = new Vector2(-1, -1);
        }

        SetPrivateField(panelComp, "nameText", nt);
        SetPrivateField(panelComp, "levelText", lt);
        SetPrivateField(panelComp, "elementText", elemText);
        SetPrivateField(panelComp, "hpFill", hpFill);
        SetPrivateField(panelComp, "hpText", hpT);
        SetPrivateField(panelComp, "expFill", expF);
    }

    [MenuItem("Pulau Monster/10. Build Scn_End")]
    public static void BuildEndScene()
    {
        string scenePath = ScenesRoot + "/Scn_End.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Regular.ttf");
        var boldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/PixelifySans-Bold.ttf");

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;
        cam.backgroundColor = UiTheme.Ink;
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGo.tag = "MainCamera";

        // Canvas_End (1280x720)
        var canvasGo = new GameObject("Canvas_End");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background
        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.color = UiTheme.Ink;
        bgImg.rectTransform.anchorMin = Vector2.zero;
        bgImg.rectTransform.anchorMax = Vector2.one;

        // Title: "DEMO SELESAI" 20px (x4 -> 80px), Gold, Bold
        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(canvasGo.transform, false);
        var titleText = titleGo.AddComponent<Text>();
        titleText.font = boldFont ?? font;
        titleText.fontSize = 80;
        titleText.text = "DEMO SELESAI";
        titleText.color = UiTheme.Gold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.rectTransform.anchoredPosition = new Vector2(0, 200);
        titleText.rectTransform.sizeDelta = new Vector2(800, 100);

        // Partner Image: 64x64 (x4 -> 256x256), y 50 (x4 -> 200)
        var pImgGo = new GameObject("Partner");
        pImgGo.transform.SetParent(canvasGo.transform, false);
        var pImg = pImgGo.AddComponent<Image>();
        pImg.rectTransform.sizeDelta = new Vector2(256, 256);
        pImg.rectTransform.anchoredPosition = new Vector2(0, 10);

        // PartnerText: 9px (x4 -> 36px)
        var pTextGo = new GameObject("PartnerText");
        pTextGo.transform.SetParent(canvasGo.transform, false);
        var pText = pTextGo.AddComponent<Text>();
        pText.font = boldFont ?? font;
        pText.fontSize = 36;
        pText.text = "Pyrit · Lv 9";
        pText.color = UiTheme.Text;
        pText.alignment = TextAnchor.MiddleCenter;
        pText.rectTransform.anchoredPosition = new Vector2(0, -150);
        pText.rectTransform.sizeDelta = new Vector2(600, 50);

        // Playtime: 8px (x4 -> 32px)
        var timeGo = new GameObject("Playtime");
        timeGo.transform.SetParent(canvasGo.transform, false);
        var timeText = timeGo.AddComponent<Text>();
        timeText.font = font;
        timeText.fontSize = 32;
        timeText.text = "Waktu bermain 5 menit 23 detik";
        timeText.color = UiTheme.Muted;
        timeText.alignment = TextAnchor.MiddleCenter;
        timeText.rectTransform.anchoredPosition = new Vector2(0, -210);
        timeText.rectTransform.sizeDelta = new Vector2(600, 50);

        // Hint: "Tekan Enter untuk kembali ke judul"
        var hintGo = new GameObject("Hint");
        hintGo.transform.SetParent(canvasGo.transform, false);
        var hintText = hintGo.AddComponent<Text>();
        hintText.font = font;
        hintText.fontSize = 24;
        hintText.text = "Tekan Enter untuk kembali ke judul";
        hintText.color = UiTheme.Muted;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.rectTransform.anchoredPosition = new Vector2(0, -280);
        hintText.rectTransform.sizeDelta = new Vector2(600, 40);

        // ClickCatcher Button
        var ccGo = new GameObject("ClickCatcher");
        ccGo.transform.SetParent(canvasGo.transform, false);
        var ccImg = ccGo.AddComponent<Image>();
        ccImg.color = new Color(0, 0, 0, 0);
        var ccBtn = ccGo.AddComponent<Button>();
        ccBtn.transition = Selectable.Transition.None;
        ccImg.rectTransform.anchorMin = Vector2.zero;
        ccImg.rectTransform.anchorMax = Vector2.one;

        // EndInstaller
        var instGo = new GameObject("EndInstaller");
        var installer = instGo.AddComponent<EndInstaller>();
        SetPrivateField(installer, "partnerText", pText);
        SetPrivateField(installer, "timeText", timeText);
        SetPrivateField(installer, "partnerImage", pImg);
        SetPrivateField(installer, "clickCatcher", ccBtn);

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[DemoBuilder] Scn_End saved at " + scenePath);
    }

    [MenuItem("Pulau Monster/11. Set Build Scenes")]
    public static void SetBuildScenes()
    {
        string[] scenes =
        {
            ScenesRoot + "/Scn_Bootstrap.unity",
            ScenesRoot + "/Scn_Title.unity",
            ScenesRoot + "/Scn_World.unity",
            ScenesRoot + "/Scn_Battle.unity",
            ScenesRoot + "/Scn_End.unity"
        };

        var buildScenes = new List<EditorBuildSettingsScene>();
        foreach (var s in scenes)
        {
            buildScenes.Add(new EditorBuildSettingsScene(s, true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        PlayerSettings.companyName = "Politeknik Negeri Batam";
        PlayerSettings.productName = "Pulau Monster";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

        AssetDatabase.SaveAssets();
        Debug.Log("[DemoBuilder] Build Scenes and PlayerSettings configured.");
    }

    [MenuItem("Pulau Monster/12. Build Standalone Windows .exe")]
    public static void BuildStandaloneWin()
    {
        SetBuildScenes();

        string outDir = "Builds/PulauMonster_Win";
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        string exePath = outDir + "/PulauMonster.exe";
        var options = new BuildPlayerOptions
        {
            scenes = new[]
            {
                ScenesRoot + "/Scn_Bootstrap.unity",
                ScenesRoot + "/Scn_Title.unity",
                ScenesRoot + "/Scn_World.unity",
                ScenesRoot + "/Scn_Battle.unity",
                ScenesRoot + "/Scn_End.unity"
            },
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Debug.Log("[DemoBuilder] Building Windows 64-bit standalone player to " + exePath);
        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log("[DemoBuilder] Build result: " + report.summary.result + " (Errors: " + report.summary.totalErrors + ")");
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        if (target == null) return;
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            // Try base type
            var baseField = target.GetType().BaseType?.GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);
            if (baseField != null)
            {
                baseField.SetValue(target, value);
            }
        }
    }
}
#endif
