# WaitingAndChilling
A SCL SL plugin that spawns players in a lobby during the Waiting For Players phase of the round.

Players are spawned in a lobby as tutorials. This is just to entertain people while waiting for other players.

# Configs

`wac_enabled` Enables the plugin. Default to true.

`wac_roles` Please follow this format when using this config: `roleID,roleID,roleID` Having more than one will randomise each round. This defaults to just 14 (Tutorial)

[Currently disabled] ~~If you want to change the location of the lobby:~~ 

~~`wac_cords` Please follow this format when using this config: `X1,Y1,Z1;X2,Y2,Z2;X3,Y3,Z3` This defaults to the Tutorial spawn.~~

`wac_choose_one_role_per_round` Whether to have everyone the same role or each person a random role every round (If theres more than one role in `wac_roles`)

# Installation

Place the .dll file into you sm_plugins folder and restart your server.
