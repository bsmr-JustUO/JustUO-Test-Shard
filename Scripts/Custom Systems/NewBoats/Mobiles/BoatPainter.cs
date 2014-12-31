using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class BoatPainter : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public BoatPainter()
            : base("the Boat Painter")
        { 
            this.SetSkill(SkillName.Carpentry, 60.0, 83.0);
            this.SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public BoatPainter(Serial serial)
            : base(serial)
        { 
        }

        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo() 
        { 
            this.m_SBInfos.Add(new SBBoatPainter()); 
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.SmithHammer());
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
}