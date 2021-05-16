// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class AppleArcadeCapability : BaseCapability
    {
        public AppleArcadeCapability()
        {
        }

        public AppleArcadeCapability(PListDictionary dic)
        {
        }

        public AppleArcadeCapability(AppleArcadeCapability other)
        : base (other)
        {
        }

        #region implemented abstract members of BaseCapability
        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            return dic;
        }

        public override BaseCapability Clone()
        {
            return new AppleArcadeCapability(this);
        }
        #endregion
    }
}

