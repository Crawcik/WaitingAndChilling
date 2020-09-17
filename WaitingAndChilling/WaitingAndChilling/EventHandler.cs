using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace WaitingAndChilling
{
    internal class EventHandler : IEventHandlerFixedUpdate, IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerDoorAccess, IEventHandlerElevatorUse, IEventHandlerPlayerJoin
    {
        private readonly WaitingAndChilling plugin;

        bool waitingForPlayers = true;

        bool startUpDone = false;
        bool stationsSpwned = false;

        float time = 1;
        float timer = 0;

        List<int> roles = new List<int>();
        List<Vector> positions = new List<Vector>();

        string positionString = "";
        string rolesString = "";

        public EventHandler(WaitingAndChilling plugin)
        {
            this.plugin = plugin;
            
        }

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            if (waitingForPlayers)
            {
                ev.Allow = false;
            }
        }

        public void OnElevatorUse(PlayerElevatorUseEvent ev)
        {
            if (waitingForPlayers)
            {
                ev.AllowUse = false;
            }
        }

        int i = -1;
        int role;

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (waitingForPlayers)
            {
                //System.Random rnd = new System.Random();
                try { GameObject.Find("StartRound").transform.localScale = Vector3.zero; }
                catch { plugin.Debug("StartRound not found... If you see this because you just started the server, ignore this."); }

                if (timer < time)
                {
                    timer += Time.deltaTime;
                }
                else if (plugin.Server.GetPlayers().Count > 0)
                {
                    timer = 0;
                    string str = "<color=#fffbad>WAITING FOR PLAYERS</color>\n";
                    short waitingTimer = GameCore.RoundStart.singleton.Timer;
                    if (waitingTimer == -2)
                    {
                        str += "<color=#ff7d92>paused</color>";
                    }
                    else if (waitingTimer == -1)
                    {
                        return;
                    }
                    else
                    {
                        str += "<color=#7dff8a>" + waitingTimer.ToString("0") + "</color> seconds left";
                    }
                    foreach (Player player in plugin.Server.GetPlayers())
                    {
                        player.ShowHint(str);
                    }
                    /*foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
                    {
                        if (referenceHub.isDedicatedServer)
                            continue;

                        if (referenceHub.characterClassManager.CurClass == RoleType.Spectator || referenceHub.characterClassManager.CurClass == (RoleType)i)
                        {
                            if (!plugin.oneRolePerRound)
                                role = rnd.Next(0, roles.Count);
                            referenceHub.characterClassManager.SetPlayersClass((RoleType)roles[role], referenceHub.gameObject, false, false, false);
                            referenceHub.playerMovementSync.OverridePosition(positions[pos], 0);

                        referenceHub.inventory.AllowChangeItem();

                        if (plugin.items.ToList().Any())
                        {
                            foreach (int item in plugin.items)
                            {
                                referenceHub.inventory.AddNewItem((ItemType)item);
                            }
                            referenceHub.ammoBox[(int)AmmoType.AMMO9MM] = 100;
                            referenceHub.ammoBox[(int)AmmoType.AMMO556] = 100;
                            referenceHub.ammoBox[(int)AmmoType.AMMO762] = 100;
                        }
                    }
                        referenceHub.hints.Show(new Hints.TextHint(str, new Hints.HintParameter[] { new Hints.StringHintParameter(string.Empty) }, Hints.HintEffectPresets.FadeInAndOut(0f), 1f));
                    }*/
                }
                else
                {
                    timer = 0;
                }

                if (plugin.stations)
                {
                    foreach (Player player in plugin.Server.GetPlayers())
                    {
                        if (player.GetPosition().x >= 53.2576f && player.GetPosition().x <= 53.75784f && player.GetPosition().y >= 1018 && player.GetPosition().y <= 1020 && player.GetPosition().z >= -40.1317f && player.GetPosition().z <= -39)
                        {
                            player.Teleport(new Vector(53.43536f + 63.93872f, 1019.5f - 41.38f, -40.70692f + 173.04748f));
                        }
                        else if (player.GetPosition().x >= 53.2576f + 63.93872f && player.GetPosition().x <= 53.75784f + 63.93872f && player.GetPosition().y >= 1018 - 41.38f && player.GetPosition().y <= 1020 - 41.38f && player.GetPosition().z >= -40.1317f + 173.04748f && player.GetPosition().z <= -39 + 173.04748f)
                        {
                            player.Teleport(new Vector(53.43536f, 1019.5f, -40.70692f));
                            player.Teleport(new Vector(53.43536f, 1019.5f, -40.70692f));
                        }
                    }
                }
            }
        }

        int pos = 0;

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            System.Random rnd = new System.Random();
            pos = rnd.Next(0, positions.Count);
            if (plugin.oneRolePerRound)
                role = rnd.Next(0, roles.Count);
            waitingForPlayers = true;
            stationsSpwned = false;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            waitingForPlayers = false;
        }

        void StartUp()
        {
            List<char> chars = plugin.cords.ToCharArray().ToList();
            foreach (char _char in chars)
            {
                if (_char != ' ')
                {
                    positionString += _char;
                }
            }
            plugin.Info(positionString);
            List<string> posStrings = positionString.Split(';').ToList();
            foreach (string posString in posStrings)
            {
                plugin.Info(posString);
                List<string> xyz = posString.Split(',').ToList();
                if (xyz.Count == 3)
                {
                    plugin.Info("Gojng to try parse " + xyz[0] + " " + xyz[1] + " " + xyz[2]);
                    try
                    {
                        positions.Add(new Vector(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2])));
                    }
                    catch
                    {
                        plugin.Info("Could not parse " + posString);
                    }
                }
            }
            if (!positions.Any())
            {
                plugin.Info("positions list empty, adding default.");
                positions.Add(new Vector(53, 1020, -43));
            }
            System.Random rnd = new System.Random();
            pos = rnd.Next(0, positions.Count);
            List<char> charsRole = plugin.roleInt.ToCharArray().ToList();
            foreach (char _char in charsRole)
            {
                if (_char != ' ')
                {
                    rolesString += _char;
                }
            }
            plugin.Info(rolesString);
            List<string> roleStrings = rolesString.Split(',').ToList();
            foreach (string roleString in roleStrings)
            {
                plugin.Info(roleString);
                try
                {
                    roles.Add(int.Parse(roleString));
                }
                catch
                {
                    plugin.Info("Could not parse " + roleString);
                }
            }
            if (!roles.Any())
            {
                roles.Add(14);
            }
            if (plugin.oneRolePerRound)
                role = rnd.Next(0, roles.Count);

            
            
        }

        void SpawnStations()//spawn weapon stations
        {
            plugin.SpawnWorkbench(new Vector3(97, 976, 117), new Vector3(0, -90, 0), new Vector3(25, 10, 5), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(126, 976, 138), new Vector3(0, 0, 0), new Vector3(25, 10, 5), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(147, 976, 109), new Vector3(0, 90, 0), new Vector3(25, 10, 5), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(119, 976, 90), new Vector3(0, 180, 0), new Vector3(25, 10, 5), true, "Work Station");

            plugin.SpawnWorkbench(new Vector3(55, 1019, -39.5f), new Vector3(0, 90, 0), new Vector3(1, 1, 1), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(52, 1019, -39.5f), new Vector3(0, -90, 0), new Vector3(1, 1, 1), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(53.5f, 1018, -39.4f), new Vector3(0, 180, 0), new Vector3(1, 1, 1), true, "Work Station");

            plugin.SpawnWorkbench(new Vector3(55 + 63.93872f, 1019 - 41.38f, -39.5f + 173.04748f), new Vector3(0, 90, 0), new Vector3(1, 1, 1), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(52 + 63.93872f, 1019 - 41.38f, -39.5f + 173.04748f), new Vector3(0, -90, 0), new Vector3(1, 1, 1), true, "Work Station");
            plugin.SpawnWorkbench(new Vector3(53.5f + 63.93872f, 1018 - 41.38f, -39.4f + 173.04748f), new Vector3(0, 180, 0), new Vector3(1, 1, 1), true, "Work Station");
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (!startUpDone)
            {
                StartUp();
                startUpDone = true;
            }
            if (plugin.stations && !stationsSpwned)
            {
                SpawnStations();
                stationsSpwned = true;
            }
            if (waitingForPlayers)
            {
                if (!plugin.oneRolePerRound)
                {
                    System.Random rnd = new System.Random();
                    role = rnd.Next(0, roles.Count);
                }
                //ev.Player.Teleport(positions[pos]);
                ev.Player.ChangeRole((Smod2.API.RoleType)roles[role]);
            }
        }
    }
}
