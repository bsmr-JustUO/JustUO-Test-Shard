using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Network;

namespace Server.Multis
{
    public abstract class NewBaseBoat : BaseShip
    {
        private NewTillerMan m_TillerMan;
        private NewPlank m_PPlank;
        private NewPlank m_SPlank;
        private NewHold m_Hold;

        protected abstract int NorthID { get; }

        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public NewTillerMan TillerMan { get { return m_TillerMan; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank PPlank { get { return m_PPlank; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank SPlank { get { return m_SPlank; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewHold NewHold { get { return m_Hold; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual BaseDockedNewBoat DockedNewBoat { get { return null; } }
        #endregion

        protected NewBaseBoat(int ItemID, ushort maxDurability, Point3D tillermanOffset, Point3D pPlankOffset, Point3D sPlankOffset, Point3D holdOffset)
            : base(ItemID, maxDurability)
        {
            m_TillerMan = new NewTillerMan(this, tillermanOffset);
            m_PPlank = new NewPlank(this, pPlankOffset, PlankSide.Port, 0);
            m_SPlank = new NewPlank(this, sPlankOffset, PlankSide.Starboard, 0);
            m_Hold = new NewHold(this, holdOffset);
        }

        public NewBaseBoat(Serial serial)
            : base(serial)
        { }
    
        protected override int GetMultiId(Direction newFacing)
        {
            return NorthID + ((int)newFacing / 2);
        }

        protected override bool IsEnabledLandID(int landID)
        {
            if (landID > 167 && landID < 172)
                return true;

            if (landID == 310 || landID == 311)
                return true;

            return false;
        }

        protected override bool IsEnabledStaticID(int staticID)
        {
            if (staticID > 0x1795 && staticID < 0x17B3)
                return true;

            return false;
        }
		
		public override uint CreateKeys( Mobile m )
		{
			uint value = Key.RandomValue();

			Key packKey = new Key( KeyType.Gold, value, this );
			Key bankKey = new Key( KeyType.Gold, value, this );
			

			packKey.MaxRange = 10;
			bankKey.MaxRange = 10;

			packKey.Name = "a ship key";
			bankKey.Name = "a ship key";

			BankBox box = m.BankBox;

			if ( !box.TryDropItem( m, bankKey, false ) )
				bankKey.Delete();
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502484 ); // A ship's key is now in my safety deposit box.

			if ( m.AddToBackpack( packKey ) )
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502485 ); // A ship's key is now in my backpack.
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502483 ); // A ship's key is now at my feet.

			return value;
		}	

		#region DryDock
        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying }

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;
            if ((m_SPlank == null || !Key.ContainsKey(pack, m_SPlank.KeyValue)) && (m_PPlank == null || !Key.ContainsKey(pack, m_PPlank.KeyValue)))
                return DryDockResult.NoKey;

            if (!Anchored)
                return DryDockResult.NotAnchored;

            if (Hold != null && Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o == this || o is NewPlank || o is NewTillerMan || o is NewHold )
                    continue;

                if (o is Item && Contains((Item)o))
                {
                    eable.Free();
                    return DryDockResult.Items;
                }
                else if (o is Mobile && Contains((Mobile)o))
                {
                    eable.Free();
                    return DryDockResult.Mobiles;
                }
            }

            eable.Free();
            return DryDockResult.Valid;
        }
		
        public void BeginDryDock(Mobile from)
        {
            if (CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.Valid)
                from.SendGump(new NewBoatConfirmDryDockGump(from, this));
        }

        public void EndDryDock(Mobile from)
        {
            if (Deleted || CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedNewBoat boat = DockedNewBoat;

            if (boat == null)
                return;

            RemoveKeys(from);

            from.AddToBackpack(boat);

            Delete();			
        }	
		
        public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (m_PPlank != null)
                keyValue = this.m_PPlank.KeyValue;

            if (keyValue == 0 && m_SPlank != null)
                keyValue = this.m_SPlank.KeyValue;

            Key.RemoveKeys(m, keyValue);
        }	
		#endregion

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_TillerMan = reader.ReadItem() as NewTillerMan;
            m_PPlank = reader.ReadItem() as NewPlank;
            m_SPlank = reader.ReadItem() as NewPlank;
            m_Hold = reader.ReadItem() as NewHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)m_TillerMan);
            writer.Write((Item)m_PPlank);
            writer.Write((Item)m_SPlank);
            writer.Write((Item)m_Hold);
        }
        #endregion
    }
}