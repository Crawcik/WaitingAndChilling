using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using UnityEngine;
using Mirror;

namespace WaitingAndChilling
{
    [PluginDetails(
        author = "F4Fridey",
        name = "Waiting And Chilling",
        description = "A plugin that spawn players in during the Waiting For Players stage.",
        id = "f4fridey.waitingandchilling.plugin",
        configPrefix = "wac",
        langFile = "waitingandchilling",
        version = "1.4.0",
        SmodMajor = 3,
        SmodMinor = 10,
        SmodRevision = 2
        )]
    public class WaitingAndChilling : Plugin
    {
        [ConfigOption]
        public readonly bool Enabled = true;

        [ConfigOption]
        public readonly bool Stations = false;//keep false this breaks worstations around the map

        [ConfigOption("role")]
        public readonly int Role = (int)RoleType.Tutorial;

        [ConfigOption("cords")]
        public readonly string Cords = "-11,1005,-43";

        public override void OnDisable()
        {
            Info("WAC Disabled.");
        }

        public override void OnEnable()
        {
            if (Enabled)
            {
                Info("WAC Enabled.");
            }
            else
            {
                Info("WAC has not registered, wac_enabled is false!");
            }
        }

        public override void Register()
        {
            if (Enabled)
            {
                AddEventHandlers(new EventHandler(this));
            }
        }

        public void SpawnWorkbench(Vector3 position, Vector3 rotation, Vector3 size, bool spawn, string objName)
        {
            /*GameObject bench =
                UnityEngine.Object.Instantiate(
                    NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == objName));
            Offset offset = new Offset();
            offset.position = position;
            offset.rotation = rotation;
            offset.scale = Vector3.one;
            bench.gameObject.transform.localScale = size;

            if (spawn) NetworkServer.Spawn(bench);
            else NetworkServer.Destroy(bench);
            
            bench.GetComponent<>().Networkposition = offset;
            bench.AddComponent<WorkStationUpgrader>();*/
        }
    }
}
