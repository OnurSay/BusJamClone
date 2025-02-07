using UnityEngine;

namespace BusJamClone.Scripts.Data
{
    [CreateAssetMenu(fileName = "Game Color", menuName = "Game Color")]
    public class GameColors : ScriptableObject
    {

        public Color[] activeColors;
        public Material[] activeMaterials;

   
    
    }
}