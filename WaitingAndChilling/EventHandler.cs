using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace WaitingAndChilling
{
    internal class EventHandler : IEventHandlerFixedUpdate, IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerDoorAccess, IEventHandlerElevatorUse, IEventHandlerPlayerJoin
    {
        private const float _offset = 1;

        private readonly List<Vector> _positions = new List<Vector>();
        private readonly WaitingAndChilling _plugin;
        private bool _waitingForPlayers = true;
        private bool _startUpDone = false;
        private bool _stationsSpawned = false;
        private float _timer = 0;
        private int _nextPos = 0;

        public EventHandler(WaitingAndChilling plugin)
        {
            _plugin = plugin;

        }

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            if (_waitingForPlayers)
            {
                ev.Allow = false;
            }
        }

        public void OnElevatorUse(PlayerElevatorUseEvent ev)
        {
            if (_waitingForPlayers)
            {
                ev.AllowUse = false;
            }
        }

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (_waitingForPlayers)
            {
                //System.Random rnd = new System.Random();
                try { GameObject.Find("StartRound").transform.localScale = Vector3.zero; }
                catch { _plugin.Debug("StartRound not found... If you see this because you just started the server, ignore this."); }

                if (_timer < _offset)
                {
                    _timer += Time.deltaTime;
                }
                else if (_plugin.Server.GetPlayers().Count > 0)
                {
                    _timer = 0;
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
                    foreach (Player player in _plugin.Server.GetPlayers())
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
                    _timer = 0;
                }

                if (_plugin.Stations)
                {
                    foreach (Player player in _plugin.Server.GetPlayers())
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

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            _nextPos = (_nextPos + 1) % _positions.Count;
            _waitingForPlayers = true;
            _stationsSpawned = false;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            _waitingForPlayers = false;
        }

        void StartUp()
        {
            foreach (string posString in _plugin.Cords.Split(';'))
            {
                _plugin.Info(posString);
                string[] xyz = posString.Split(',').Select(x=>x.Trim()).ToArray();
                if (xyz.Length == 3)
                {
                    _plugin.Info("Going to try parse " + xyz[0] + " " + xyz[1] + " " + xyz[2]);
                    try
                    {
                        _positions.Add(new Vector(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2])));
                    }
                    catch
                    {
                        _plugin.Info("Could not parse " + posString);
                    }
                }
            }
            if (!_positions.Any())
            {
                _plugin.Info("positions list empty, adding default.");
                _positions.Add(new Vector(53, 1020, -43));
            }
            _nextPos = (_nextPos + 1) % _positions.Count;
        }

        void SpawnStations()//spawn weapon stations
        {
            _plugin.SpawnWorkbench(new Vector3(97, 976, 117), new Vector3(0, -90, 0), new Vector3(25, 10, 5), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(126, 976, 138), new Vector3(0, 0, 0), new Vector3(25, 10, 5), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(147, 976, 109), new Vector3(0, 90, 0), new Vector3(25, 10, 5), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(119, 976, 90), new Vector3(0, 180, 0), new Vector3(25, 10, 5), true, "Work Station");

            _plugin.SpawnWorkbench(new Vector3(55, 1019, -39.5f), new Vector3(0, 90, 0), new Vector3(1, 1, 1), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(52, 1019, -39.5f), new Vector3(0, -90, 0), new Vector3(1, 1, 1), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(53.5f, 1018, -39.4f), new Vector3(0, 180, 0), new Vector3(1, 1, 1), true, "Work Station");

            _plugin.SpawnWorkbench(new Vector3(55 + 63.93872f, 1019 - 41.38f, -39.5f + 173.04748f), new Vector3(0, 90, 0), new Vector3(1, 1, 1), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(52 + 63.93872f, 1019 - 41.38f, -39.5f + 173.04748f), new Vector3(0, -90, 0), new Vector3(1, 1, 1), true, "Work Station");
            _plugin.SpawnWorkbench(new Vector3(53.5f + 63.93872f, 1018 - 41.38f, -39.4f + 173.04748f), new Vector3(0, 180, 0), new Vector3(1, 1, 1), true, "Work Station");
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (!_startUpDone)
            {
                StartUp();
                _startUpDone = true;
            }
            if (_plugin.Stations && !_stationsSpawned)
            {
                SpawnStations();
                _stationsSpawned = true;
            }
            if (_waitingForPlayers)
            {
                
                ev.Player.ChangeRole((Smod2.API.RoleType)_plugin.Role);
                ev.Player.Teleport(_positions[_nextPos]);
            }
        }
    }
}
