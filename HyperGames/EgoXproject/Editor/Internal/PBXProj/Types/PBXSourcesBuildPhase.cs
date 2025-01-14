﻿//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXSourcesBuildPhase : PBXBaseBuildPhase
    {
        public PBXSourcesBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXSourcesBuildPhase, uid, dict)
        {
        }

        public static PBXSourcesBuildPhase Create(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException( (uid).ToString(), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXSourcesBuildPhase.ToString());
            PopulateEmptyDictionary(emptyDic);
            return new PBXSourcesBuildPhase(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion


    }
}
