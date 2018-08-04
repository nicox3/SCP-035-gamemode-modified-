using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.API;
using Smod.SCP035;
using System;
using System.Collections.Generic;
using Smod2.EventHandlers;


namespace scp035
{
	[PluginDetails(
		author = "ShingekiNoRex, modified by Nicopara",
		name = "scp035",
		description = "Tuned version of the original 035 gamemode",
		id = "rex.scp035",
		version = "2.0N",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 7
	)]
	class SCP035 : Plugin
	{
		public override void OnDisable()
		{
		}

		public override void OnEnable()
		{
			this.Info("SCP-035 has loaded :)");
		}

		public override void Register()
		{
			// Register Events
			this.AddEventHandlers(new SmodEventHandler(this), Priority.Normal);
			GamemodeManager.GamemodeManager.RegisterMode(this, "43444443444344434444");
		}
	}
}

namespace Smod.SCP035
{
	class SmodEventHandler : IEventHandlerPlayerDie, IEventHandlerRoundStart, IEventHandlerSetRole, IEventHandlerPlayerHurt, IEventHandlerCheckRoundEnd
	{
		private static List<int> scp_list = new List<int>();
		private Plugin plugin;
		//private static bool roundend;
		//private static bool roundstart;
		private static bool allowkill;
		public SmodEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

        //Now the warhead starts automatically after 10 minutes, and LCZ after 6. -Nico
        public void OnStartCountDown(WarheadStartEvent WEV)
        {
            WEV.TimeLeft = 600f;
        }
        /*not yet done, probably never :(, note: this is just pseudocode, apparently you can't do recipes from here.
        public void OnSCP914Activate(SCP914ActivateEvent S914)
        {
            if(S914.KnobSetting == KnobSetting.ROUGH && Itemtype.COIN) { S914.Outtake = rand.range; }
        }
        */
        public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
                List<Player> PlayerList = new List<Player>();
				scp_list = new List<int>();
				foreach (Player player in ev.Server.GetPlayers())
				{
					if (player.TeamRole.Team == Team.CLASSD)
					{
						PlayerList.Add(player);
					}
					if (player.TeamRole.Team == Team.SCP)
					{
						scp_list.Add(player.PlayerId);
					}
				}

				Random rm = new Random();
				Player scp035 = PlayerList[rm.Next(PlayerList.Count)];
				scp_list.Add(scp035.PlayerId);

				scp035.GiveItem(ItemType.COM15);
				//scp035.GiveItem(ItemType.MEDKIT);
				//scp035.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
				//added FRAG, WEAPON_MANAGER_TABLET, x2 flash and 300 ammo for each type of ammo -Nico
				scp035.GiveItem(ItemType.FRAG_GRENADE);
				scp035.GiveItem(ItemType.FLASHBANG);
				scp035.GiveItem(ItemType.FLASHBANG);
				scp035.GiveItem(ItemType.WEAPON_MANAGER_TABLET);
                scp035.SetAmmo(AmmoType.DROPPED_5, 300);
                scp035.SetAmmo(AmmoType.DROPPED_7, 300);
                scp035.SetAmmo(AmmoType.DROPPED_9, 300);
			
				
				PlayerList.Remove(scp035);

				if (PlayerList.Count > 4)
				{
					Player randomplayer = PlayerList[rm.Next(PlayerList.Count)];
                    //Added RADIO -Nico
                    randomplayer.GiveItem(ItemType.MP4);
                    randomplayer.GiveItem(ItemType.JANITOR_KEYCARD);
                    randomplayer.GiveItem(ItemType.RADIO);
                    PlayerList.Remove(randomplayer);

					if (PlayerList.Count > 8)
					{
						randomplayer = PlayerList[rm.Next(PlayerList.Count)];
						randomplayer.GiveItem(ItemType.MP4);
                        randomplayer.GiveItem(ItemType.JANITOR_KEYCARD);
                        randomplayer.GiveItem(ItemType.RADIO);

                        PlayerList.Remove(randomplayer);
					}

					if (PlayerList.Count > 10)
					{
						randomplayer = PlayerList[rm.Next(PlayerList.Count)];
						//Removed the MEDKIT, FLASHBANG. Switched ZONE_MANAGER_KEYCARD to JANITOR_KEYCARD -Nico
						//randomplayer.GiveItem(ItemType.MEDKIT);
						//randomplayer.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
						//randomplayer.GiveItem(ItemType.FLASHBANG);
						randomplayer.GiveItem(ItemType.JANITOR_KEYCARD);
						randomplayer.GiveItem(ItemType.RADIO);
						scp_list.Add(randomplayer.PlayerId);
					}
				}
				/*DELETED THIS -Nico
				foreach (Item item in ev.Server.Map.GetItems(ItemType.FRAG_GRENADE, false))
				{
					item.Remove();
				}
				 */

				//roundend = false;
				//roundstart = true;
				allowkill = false;
				System.Timers.Timer t = new System.Timers.Timer();
				t.Interval = 60000;
				t.AutoReset = false;
				t.Enabled = true;
				t.Elapsed += delegate
				{
					allowkill = true;
					t.Enabled = false;
				};
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				if (ev.Player.PlayerId != ev.Killer.PlayerId && !scp_list.Contains(ev.Player.PlayerId) && !scp_list.Contains(ev.Killer.PlayerId) && ev.Killer.TeamRole.Team != Team.SCP && ev.Player.TeamRole.Team != Team.SCP)
				{
					ev.Killer.Kill();
				}

				if (scp_list.Contains(ev.Player.PlayerId))
				{
					scp_list.Remove(ev.Player.PlayerId);
				}
			}
			/*
			if (ev.Player.PlayerId != ev.Killer.PlayerId)
			{
				if (ev.Player.PlayerId == scp_id)
				{
					if (!roundend)
					{
						plugin.pluginManager.Server.Round.EndRound();
						roundend = true;
						roundstart = false;
					}
				}
				else if (ev.Killer.PlayerId != scp_id && ev.Killer.TeamRole.Team != Team.SCP && ev.Player.TeamRole.Team != Team.SCP)
				{
					ev.Killer.Kill();
					CheckEndCondition();
				}
				else
				{
					CheckEndCondition();
				}
			}
			else
			{
				CheckEndCondition();
			}*/
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				if (ev.DamageType != DamageType.FALLDOWN && ev.Player.TeamRole.Team != Team.SCP && (!scp_list.Contains(ev.Player.PlayerId) || ev.Player.TeamRole.Team == Team.CLASSD))
				{
					if (allowkill)
					{
						ev.Damage = 500.0F;
					}
					else
					{
						ev.Damage = 0.0F;
					}
				}
				if (scp_list.Contains(ev.Player.PlayerId) && scp_list.Contains(ev.Attacker.PlayerId) && ev.Player.PlayerId != ev.Attacker.PlayerId)
				{
					ev.Damage = 0.0F;
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				/* Deleted THIS -Nico
				foreach (Item item in ev.Player.GetInventory())
				{
					if (item.Equals(ItemType.FRAG_GRENADE))
					{
						item.Remove();
					}
				}
				 */

				if (ev.Role == Role.SCIENTIST)
				{
					//Changed to P90, reduced drastically the ammunition scientists have, so they aren't as trigger happy as they used to be -Nico
					//ev.Player.GiveItem(ItemType.E11_STANDARD_RIFLE);
					ev.Player.GiveItem(ItemType.P90);
					ev.Player.GiveItem(ItemType.DISARMER);
					ev.Player.GiveItem(ItemType.RADIO);
                    ev.Player.SetAmmo(AmmoType.DROPPED_5, 0);
                    ev.Player.SetAmmo(AmmoType.DROPPED_7, 0);
                    ev.Player.SetAmmo(AmmoType.DROPPED_9, 0);
				}
				else if (ev.Role == Role.NTF_SCIENTIST)
				{
					//removed flashbangs
					//ev.Player.GiveItem(ItemType.FLASHBANG);
					ev.Player.GiveItem(ItemType.DISARMER);
				}
                //added this role, so if a d-boi escapes, has a chance of fighting the traitor. -Nico
                else if(ev.Role == Role.CHAOS_INSUGENCY)
                {
                    ev.Player.GiveItem(ItemType.MICROHID);
                    ev.Player.GiveItem(ItemType.E11_STANDARD_RIFLE);
                    ev.Player.GiveItem(ItemType.O5_LEVEL_KEYCARD);
                    ev.Player.GiveItem(ItemType.MP4);
                }
			}
			//CheckEndCondition();
		}

        //added AmountOfPlayersEscaped - Nico
        private static byte AmountOfPlayersEscaped = 0;

        public void OnCheckEscape(PlayerCheckEscapeEvent ev)
        { 
            if(ev.Player.TeamRole.Role == Role.CLASSD)
            AmountOfPlayersEscaped++;
        }

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				bool scpalive = false;
				bool humanalive = false;

				foreach (Player player in ev.Server.GetPlayers())
				{
					if (scp_list.Contains(player.PlayerId) || player.TeamRole.Team == Team.SCP)
					{
						scpalive = true;
					}
					else if (player.TeamRole.Team != Team.SPECTATOR)
					{
						humanalive = true;
					}
				}

				if (scpalive && !humanalive)
				{
					ev.Status = ROUND_END_STATUS.SCP_VICTORY;
				}
                //Added this so when 2 D-bois escape, they instantly win. -Nico

                else if (!scpalive && humanalive || AmountOfPlayersEscaped == 2)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY;
				}
				else
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
			}
		}

        /*
		public void OnDisconnect(DisconnectEvent ev)
		{
			CheckEndCondition();
		}
		
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (roundstart && plugin.pluginManager.Server.NumPlayers <= 2)
			{
				plugin.pluginManager.Server.Round.EndRound();
			}
		}

		public void CheckEndCondition()
		{
			System.Timers.Timer t = new System.Timers.Timer();
			t.Interval = 5000;
			t.AutoReset = false;
			t.Enabled = true;
			t.Elapsed += delegate
			{
				t.Enabled = false;
				if (roundstart)
				{
					bool scpalive = false;
					bool humanalive = false;

					foreach (Player player in plugin.pluginManager.Server.GetPlayers())
					{
						if (player.TeamRole.Team != Team.SPECTATOR)
						{
							if (player.PlayerId == scp_id && !scpalive)
							{
								scpalive = true;
							}
							if (player.PlayerId != scp_id && !humanalive && player.TeamRole.Team != Team.SCP)
							{
								humanalive = true;
							}
						}
					}

					if (!scpalive || (scpalive && !humanalive))
					{
						if (!roundend)
						{
							plugin.pluginManager.Server.Round.EndRound();
							roundend = true;
							roundstart = false;
						}
					}
				}
			};
		}*/
    }
}