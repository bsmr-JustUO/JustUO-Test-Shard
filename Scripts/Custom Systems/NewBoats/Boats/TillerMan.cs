using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class NewTillerMan : BaseShipItem, IFacingChange
    {	
		private NewBaseBoat m_Boat;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ShareHue { get { return false; } }		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public NewBaseBoat Boat { get { return m_Boat; } }		
	
        public NewTillerMan(NewBaseBoat boat, Point3D initOffset)
            : base(boat, 0x3E4E, initOffset)
        {
			m_Boat = boat;
			m_Boat.TillerManItem = this;
        }

        public NewTillerMan(Serial serial)
            : base(serial)
        {
        }

        public void Say(int number)
        {
            if(!Transport.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            if (!Transport.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }
		
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && m_Boat != null && m_Boat.CanCommand(from) && m_Boat.Contains(from))
            {
                m_Boat.AssociateMap((MapItem)dropped);
            }

            return false;
        }		

        public override void OnDoubleClick(Mobile from)
        {
            if (!Transport.IsDriven)
           {
                if (m_Boat != null)
                {
                    if (from.InRange(this.Location, 2))
                    {
                        if (!from.Mounted)
                        {
                            if (from == m_Boat.Owner)
                            {
                                from.SendMessage("You take control of the ship");
                                Transport.TakeCommand(from);
                            }
                            else if (m_Boat.PlayerAccess != null)
                            {
                                if (m_Boat.PlayerAccess.ContainsKey((PlayerMobile)from))
                                {
                                    if (m_Boat.PlayerAccess[(PlayerMobile)from] == 2)
                                    {
                                        from.SendMessage("You take control of the ship");
                                        Transport.TakeCommand(from);
                                    }
                                    else if (m_Boat.PlayerAccess[(PlayerMobile)from] == 3)
                                    {
                                        from.SendMessage("You take control of the ship");
                                        Transport.TakeCommand(from);
                                    }
                                    else if (m_Boat.PlayerAccess[(PlayerMobile)from] == 4)
                                    {
                                        from.SendMessage("You take control of the ship");
                                        Transport.TakeCommand(from);
                                    }
                                    else if ((from.Guild == m_Boat.Owner.Guild) && (from.Guild != null))
                                    {
                                        if (m_Boat.Guild == 2)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Guild == 3)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Guild == 4)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                    }
                                    else if ((from.Party == m_Boat.Owner.Party) && (from.Party != null))
                                    {
                                        if (m_Boat.Party == 2)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Party == 3)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Party == 4)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                    }
                                    else
                                    {
                                        if (m_Boat.Public == 2)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Public == 3)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                        else if (m_Boat.Public == 4)
                                        {
                                            from.SendMessage("You take control of the ship");
                                            Transport.TakeCommand(from);
                                        }
                                    }
                                }
                                else
                                {
                                    from.SendMessage("You are not allowed to do that");
                                }
                            }
                            else
                            {
                                from.SendMessage("You are not allowed to do that");
                            }
                        }
                        else
                        {
                            from.SendMessage("You can not control the ship while mounted");
                        }
                    }
                    else
                    {
                        from.SendMessage("You are to far away from the tiller");
                    }
                }
            }
            else
            {
                Transport.LeaveCommand(from);
                from.SendMessage("You step away from the tiller");
            }
        }

        public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.South: SetItemIDOnSmooth(0x3E4B); break;
                case Direction.North: SetItemIDOnSmooth(0x3E4E); break;
                case Direction.West: SetItemIDOnSmooth(0x3E50); break;
                case Direction.East: SetItemIDOnSmooth(0x3E55); break;
            }

            if (oldFacing == Server.Direction.North)
            {
                SetLocationOnSmooth(new Point3D(X - 1, Y, Z));
            }
            else if (newFacing == Server.Direction.North)
            {
                switch (oldFacing)
                {
                    case Server.Direction.South: SetLocationOnSmooth(new Point3D(X - 1, Y, Z)); break;
                    case Server.Direction.East: SetLocationOnSmooth(new Point3D(X, Y + 1, Z)); break;
                    case Server.Direction.West: SetLocationOnSmooth(new Point3D(X, Y - 1, Z)); break;
                }
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version
			
			// version 1 : m_Boat
			writer.Write((NewBaseBoat)m_Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch (version)
            {
                case 1:
                    {
                        m_Boat = reader.ReadItem() as NewBaseBoat;
                        break;
                    }
            }			
        }
        #endregion
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new SecuritySettingsEntry(from, (NewBaseBoat)this.Transport));
			list.Add(new RenameEntry(from, (NewBaseBoat)this.Transport));
        }		
		
		private class SecuritySettingsEntry : ContextMenuEntry
		{			
			private readonly Mobile m_from;
			private NewBaseBoat m_Boat;
			
			public SecuritySettingsEntry(Mobile from, NewBaseBoat boat)
				: base(1116567)
			{				
				m_from = from;
				m_Boat = boat;
			}

			public override void OnClick()
			{
				if (m_from != null)
				{
				
					Dictionary<int, PlayerMobile> PlayersAboard = new Dictionary<int,PlayerMobile>();
					IPooledEnumerable eable = m_from.Map.GetClientsInRange(m_from.Location, m_Boat.GetMaxUpdateRange());
					int i = 0;
					foreach (NetState state in eable)
					{
						Mobile m = state.Mobile;

						if (m is PlayerMobile)						
							if (m_Boat.IsOnBoard(m))											
								PlayersAboard.Add(i++,(PlayerMobile)m);																            
					}
					eable.Free();					
				
					m_from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, m_from, (BaseShip)m_Boat, PlayersAboard, 1, null));
				}
			}
		}

		private class RenameEntry : ContextMenuEntry
		{			
			private readonly Mobile m_from;
			private NewBaseBoat m_Boat;
			
			public RenameEntry(Mobile from, NewBaseBoat boat)
				: base(1111680)
			{				
				m_from = from;
				m_Boat = boat;
			}

			public override void OnClick()
			{
				if ((m_from != null) && (m_Boat != null))
					m_Boat.BeginRename(m_from);			
			}
		}				
    }
}