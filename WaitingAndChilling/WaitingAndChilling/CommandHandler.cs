using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2.Commands;
using Smod2.API;
using UnityEngine;
using Mirror;

namespace WaitingAndChilling
{
    class CommandHandler : ICommandHandler
    {
        private readonly WaitingAndChilling plugin;

        public CommandHandler(WaitingAndChilling plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "All waiting and chilling commands.";
        }

        public string GetUsage()
        {
            return "WAC";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            List<string> _args = args.ToList();
            if (_args.Any())
            {
                switch (_args[0].ToLower())
                    {
                        default:
                            return new string[] { "Invalid argument." };
                        case "weaponman":
                        try
                        {
                            if (float.TryParse(_args[1], out float x))
                            {
                                if (float.TryParse(_args[2], out float y))
                                {
                                    if (float.TryParse(_args[3], out float z))
                                    {
                                        if (float.TryParse(_args[4], out float rx))
                                        {
                                            if (float.TryParse(_args[5], out float ry))
                                            {
                                                if (float.TryParse(_args[6], out float rz))
                                                {
                                                    if (int.TryParse(_args[7], out int playerid))
                                                    {
                                                        foreach (Player player in plugin.Server.GetPlayers())
                                                        {
                                                            if (player.PlayerId == playerid)
                                                            {
                                                                GameObject playerObj = (GameObject)player.GetGameObject();
                                                                plugin.SpawnWorkbench(new Vector3(player.GetPosition().x, player.GetPosition().y, player.GetPosition().z), new Vector3(rx, ry, rz), new Vector3(x, y, z), true, "Work Station");
                                                                string str = "Spawning weapon manager at " + player.GetPosition().x + " " + player.GetPosition().y + " " + player.GetPosition().z;
                                                                return new string[] { str };
                                                            }
                                                        }
                                                        return new string[] { "Player not found." };
                                                    }
                                                    else
                                                    {
                                                        return new string[] { "The playerId must be a number." };
                                                    }
                                                }
                                                else
                                                {
                                                    return new string[] { "RZ must be a number." };
                                                }
                                            }
                                            else
                                            {
                                                return new string[] { "RY must be a number." };
                                            }
                                        }
                                        else
                                        {
                                            return new string[] { "RX must be a number." };
                                        }
                                    }
                                    else
                                    {
                                        return new string[] { "Z must be a number." };
                                    }
                                }
                                else
                                {
                                    return new string[] { "Y must be a number." };
                                }
                            }
                            else
                            {
                                return new string[] { "X must be a number or use here" };
                            }
                        }
                        catch
                        {
                            return new string[] { "Arguments <x>, <y>, <z>, <rx>, <ry>, <rz>, and <playerid> required." };
                        }
                }
            }
            else
            {
                return new string[] { "\n[ Waiting And Chilling ]\nCommands:\nwac weaponman/locker <x> <y> <z> <rx> <ry> <rz> <playerid>" };
            }
        }
    }
}
