using System;

namespace Server.Items
{
    public abstract class ShipCannonball : BaseShipProjectile
    {
        public ShipCannonball()
            : this(1)
        {
        }

        public ShipCannonball(int amount)
            : base(amount, 0xE74)
        {
        }

        public ShipCannonball(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LightShipCannonball : ShipCannonball
    {
        [Constructable]
        public LightShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public LightShipCannonball(int amount)
            : base(amount)
        {
            this.Range = 17;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 1600;
            this.FireDamage = 0;
            this.FiringSpeed = 35;
            this.Name = "Light Ship Cannonball";
        }

        public LightShipCannonball(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        /*
        public override Item Dupe(int amount)
        {
        LightCannonball s = new LightCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class IronShipCannonball : ShipCannonball
    {
        [Constructable]
        public IronShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public IronShipCannonball(int amount)
            : base(amount)
        {
            this.Range = 15;
            this.Area = 0;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 4500;
            this.FireDamage = 0;
            this.FiringSpeed = 25;
            this.Name = "Iron Ship Cannonball";
        }

        public IronShipCannonball(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        /*
        public override Item Dupe(int amount)
        {
        IronCannonball s = new IronCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class ExplodingShipCannonball : ShipCannonball
    {
        [Constructable]
        public ExplodingShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public ExplodingShipCannonball(int amount)
            : base(amount)
        {
            this.Range = 11;
            this.Area = 1;
            this.AccuracyBonus = -10;
            this.PhysicalDamage = 300;
            this.FireDamage = 1250;
            this.FiringSpeed = 20;
            this.Hue = 46;
            this.Name = "Exploding Ship Cannonball";
        }

        public ExplodingShipCannonball(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        /*
        public override Item Dupe(int amount)
        {
        ExplodingCannonball s = new ExplodingCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class FieryShipCannonball : ShipCannonball
    {
        [Constructable]
        public FieryShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public FieryShipCannonball(int amount)
            : base(amount)
        {
            this.Range = 8;
            this.Area = 2;
            this.AccuracyBonus = -20;
            this.PhysicalDamage = 0;
            this.FireDamage = 2500;
            this.FiringSpeed = 10;
            this.Hue = 33;
            this.Name = "Fiery Ship Cannonball";
        }

        public FieryShipCannonball(Serial serial)
            : base(serial)
        {
        }

        // use a fireball animation when fired
        public override int AnimationID
        {
            get
            {
                return 0x36D4;
            }
        }
        public override int AnimationHue
        {
            get
            {
                return 0;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        /*
        public override Item Dupe(int amount)
        {
        FieryCannonball s = new FieryCannonball(amount);
        return this.Dupe(s, amount);
        }
        * */
    }

    public class ShipGrapeShot : ShipCannonball
    {
        [Constructable]
        public ShipGrapeShot()
            : this(1)
        {
        }

        [Constructable]
        public ShipGrapeShot(int amount)
            : base(amount)
        {
            this.Range = 17;
            this.Area = 1;
            this.AccuracyBonus = 0;
            this.PhysicalDamage = 1800;
            this.FireDamage = 0;
            this.FiringSpeed = 35;
            this.Name = "Ship Grape Shot";
        }

        public ShipGrapeShot(Serial serial)
            : base(serial)
        {
        }

        // only does damage to mobiles
        public override double StructureDamageMultiplier
        {
            get
            {
                return 0.0;
            }
        }//  damage multiplier for structures
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}