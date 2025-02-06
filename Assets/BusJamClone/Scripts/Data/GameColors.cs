using UnityEngine;
using UnityEngine.Serialization;

namespace BusJamClone.Scripts.Data
{
    [CreateAssetMenu(fileName = "Game Color", menuName = "Game Color")]
    public class GameColors : ScriptableObject
    {

        public Color[] activeColors;
        public Material[] activeMaterials;
        public Material[] shadowMaterials;
        public Material[] trailMaterials;

   
    
    }
}