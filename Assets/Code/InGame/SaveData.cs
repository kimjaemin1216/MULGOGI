using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string scene;
    public float px, py, pz;
    public int hearts;     // 현재 하트
    public int maxHearts;  // 최대 하트(선택)
}
