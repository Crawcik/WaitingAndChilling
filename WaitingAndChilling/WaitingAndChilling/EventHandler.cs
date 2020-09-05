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
    class EventHandler : IEventHandlerFixedUpdate, IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerDoorAccess, IEventHandlerElevatorUse, IEventHandlerPlayerJoin
    {
        private readonly WaitingAndChilling plugin;

        bool waitingForPlayers = true;

        bool startUpDone = false;

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

                    foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
                    {
                        if (referenceHub.isDedicatedServer)
                            continue;

                        /*if (referenceHub.characterClassManager.CurClass == RoleType.Spectator || referenceHub.characterClassManager.CurClass == (RoleType)i)
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
                    }*/
                        referenceHub.hints.Show(new Hints.TextHint(str, new Hints.HintParameter[] { new Hints.StringHintParameter(string.Empty) }, Hints.HintEffectPresets.FadeInAndOut(0f), 1f));
                    }
                }
                else
                {
                    timer = 0;
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

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (!startUpDone)
            {
                StartUp();
                startUpDone = true;
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
