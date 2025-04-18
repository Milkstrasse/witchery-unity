using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "Theme", menuName = "ScriptableObjects/Theme")]
public class Theme : ScriptableObject
{
    public Color[] colors;
    public TMP_StyleSheet sheet;
}