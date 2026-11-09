using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Monster/MonsterDatabase")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterData> monsters = new List<MonsterData>();

    public List<MonsterData> AllMonsters => monsters;

    public MonsterData GetById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null && monsters[i].Id == id)
                return monsters[i];
        }
        return null;
    }

    public MonsterData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null && monsters[i].DisplayName == name)
                return monsters[i];
        }
        return null;
    }
}
