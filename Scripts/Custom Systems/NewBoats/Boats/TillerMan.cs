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
			if (m_Boat != null && m_Boat.Contains(from))
			{
				if (!Transport.IsDriven)
				{
					from.SendMessage("You are now piloting this vessel");
					Transport.TakeCommand(from);
				}    
				else
				{
					Transport.LeaveCommand(from);
					from.SendMessage("You are no longer piloting this vessel");
				}
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
			if (m_Boat != null && !m_Boat.Contains(from))
			{
				list.Add(new DryDockEntry(from, (NewBaseBoat)this.Transport));
			}
			else
			{
				list.Add(new RenameEntry(from, (NewBaseBoat)this.Transport));
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
		
		private class DryDockEntry : ContextMenuEntry
		{			
			private readonly Mobile m_From;
			private NewBaseBoat m_Boat;
			
			public DryDockEntry(Mobile from, NewBaseBoat boat)
				: base(1116520)
			{				
				m_From = from;
				m_Boat = boat;
			}

			public override void OnClick()
			{
				if ((m_From != null) && (m_Boat != null))
					m_Boat.BeginDryDock(m_From);			
			}
		}			
    }
}