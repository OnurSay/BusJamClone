using BusJamClone.Scripts.Config;

namespace BusJamClone.Scripts.Singleton
{
    public class ConfigBehaviour : NonPersistentSingleton<ConfigBehaviour>
    {
        public GameConfig gameConfig;
    }
}