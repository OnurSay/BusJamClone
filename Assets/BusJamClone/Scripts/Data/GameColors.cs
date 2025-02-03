using UnityEngine;

[CreateAssetMenu(fileName = "Game Color", menuName = "Game Color")]
public class GameColors : ScriptableObject
{

    public Color[] ActiveColors;
    public Material[] ActiveMaterials;
    public Material[] ShadowMaterials;
    public Material[] TrailMaterials;

   
    
}