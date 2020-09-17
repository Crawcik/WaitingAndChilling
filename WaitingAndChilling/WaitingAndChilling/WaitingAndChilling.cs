using System;
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
        version = "1.3.2",
        SmodMajor = 3,
        SmodMinor = 9,
        SmodRevision = 0
        )]
    public class WaitingAndChilling : Plugin
    {
        [ConfigOption]
        public readonly bool enabled = true;

        [ConfigOption]
        public readonly bool stations = false;//keep false this breaks worstations around the map

        [ConfigOption("roles")]
        public readonly string roleInt = "14";

        [ConfigOption("choose_one_role_per_round")]
        public readonly bool oneRolePerRound = true;

        [ConfigOption("cords")]
        public readonly string cords = "-11,1005,-43";
        /*
        [ConfigOption("items")]
        public readonly int[] items = { 23 , 30 };*/

        public override void OnDisable()
        {
            Info("WAC Disabled.");
        }

        public override void OnEnable()
        {
            if (enabled)
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
            if (enabled)
            {
                AddEventHandlers(new EventHandler(this));
                AddCommand("wac", new CommandHandler(this));
            }
        }

        public void SpawnWorkbench(Vector3 position, Vector3 rotation, Vector3 size, bool spawn, string objName)
        {
            GameObject bench =
                UnityEngine.Object.Instantiate(
                    NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == objName));
            Offset offset = new Offset();
            offset.position = position;
            offset.rotation = rotation;
            offset.scale = Vector3.one;
            bench.gameObject.transform.localScale = size;

            if (spawn) NetworkServer.Spawn(bench);
            else NetworkServer.Destroy(bench);
            bench.GetComponent<WorkStation>().Networkposition = offset;
            bench.AddComponent<WorkStationUpgrader>();
        }
    }
}
