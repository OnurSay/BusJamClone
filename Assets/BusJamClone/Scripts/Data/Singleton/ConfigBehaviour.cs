using BusJamClone.Scripts.Data.Config;

namespace BusJamClone.Scripts.Singleton
{
    public class ConfigBehaviour : NonPersistentSingleton<ConfigBehaviour>
    {
        public GameConfig gameConfig;
    }
}