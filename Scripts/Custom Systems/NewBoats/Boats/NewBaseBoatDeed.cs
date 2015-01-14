using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public abstract class NewBaseBoatDeed : Item
	{
		private int m_MultiID;
		private Point3D m_Offset;
		private Direction m_ChosenDirection;
		private bool m_IsGoodMultiID;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get{ return m_MultiID; } set{ m_MultiID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get{ return m_Offset; } set{ m_Offset = value; } }
		
		public abstract NewBaseBoat Boat{ get; }

		public NewBaseBoatDeed( int id, Point3D offset ) : base( 0x14F2 )
		{
			Weight = 1.0;

			if ( !Core.AOS )
			LootType = LootType.Newbied;
      
      		Hue = 2401;
			m_MultiID = id;
			m_Offset = offset;
			m_IsGoodMultiID = false;
			m_ChosenDirection = Direction.North;
		}

		public NewBaseBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_MultiID );
			writer.Write( m_Offset );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_MultiID = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();

					break;
				}
			}

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendGump( new BoatPlacementGump( from, this, null ) );
		}
		
		public void PlacementDirection( Mobile from, Direction chosenDirection)
		{
			m_ChosenDirection = chosenDirection;
		
			if ( m_IsGoodMultiID == false )
			{	
				switch ( chosenDirection )
				{		

						case Direction.West:
						{
							m_MultiID += 3;	
							m_IsGoodMultiID = true;

							break;
						}	

						case Direction.South:
						{
							m_MultiID += 2;	
							m_IsGoodMultiID = true;

							break;					
						}

						case Direction.East:
						{
							m_MultiID += 1;
							m_IsGoodMultiID = true;
							
							break;
						}							
				}
			}
			
			ShipPlacement(from);
		}
		
		public void ShipPlacement( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas) )
			{
				from.SendLocalizedMessage( 1010567, null, 0x25 ); // You may not place a boat from this location.
			}
			else
			{
				if ( Core.SE )
					from.SendLocalizedMessage( 502482 ); // Where do you wish to place the ship?
				else
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502482 ); // Where do you wish to place the ship?

				from.Target = new InternalTarget( this );
			}	
		}

		

		public void OnPlacement( Mobile from, Point3D p )
		{
			if ( Deleted )
			{
				return;
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if ( map == null )
					return;

				if ( from.AccessLevel < AccessLevel.GameMaster && (map == Map.Ilshenar || map == Map.Malas) )
				{
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
					return;
				}

				if ( from.Region.IsPartOf( typeof( HouseRegion ) ) /*|| BaseGalleon.FindBoatAt( from, from.Map ) != null */)
				{
					from.SendLocalizedMessage( 1010568, null, 0x25 ); // You may not place a ship while on another ship or inside a house.
					return;
				}

				NewBaseBoat boat = Boat;

				if ( boat == null )
					return;

				p = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );

				if ( /*BaseSmoothMulti.IsValidLocation( p, map ) &&*/ boat.CanFit( p, map, boat.ItemID ) )
				{
					Delete();

					boat.Owner = from;
					boat.Anchored = false;
					
					uint keyValue = boat.CreateKeys( from );

					if ( boat.PPlank != null )
						boat.PPlank.KeyValue = keyValue;						
					if ( boat.SPlank != null )
						boat.SPlank.KeyValue = keyValue;

					boat.MoveToWorld( p, map );
					
					boat.SetFacing(m_ChosenDirection);
				}
				else
				{
					boat.Delete();
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private NewBaseBoatDeed m_Deed;

			public InternalTarget( NewBaseBoatDeed deed ) : base( deed.MultiID, deed.Offset )
			{
				m_Deed = deed;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D ip = o as IPoint3D;

				if ( ip != null )
				{
					if ( ip is Item )
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if ( region.IsPartOf( typeof( DungeonRegion ) ) )
						from.SendLocalizedMessage( 502488 ); // You can not place a ship inside a dungeon.
					else if ( region.IsPartOf( typeof( HouseRegion ) ) || region.IsPartOf( typeof( ChampionSpawnRegion ) ) )
						from.SendLocalizedMessage( 1042549 ); // A boat may not be placed in this area.
					else
						m_Deed.OnPlacement( from, p );
				}
			}
		}
	}
}