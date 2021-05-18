// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class UserManagementCapability : BaseCapability
    {
        public enum UserManagementType { RunsAsCurrentUser, GetCurrentUser };

        const string USER_MANAGEMENT_KEY = "com.apple.developer.user-management";

        public UserManagementCapability ()
        {
        }

        public UserManagementCapability (PListDictionary dic)
        {
            var controllers = dic.ArrayValue (USER_MANAGEMENT_KEY);

            if (controllers != null && controllers.Count > 0)
            {
                var gc = new List<UserManagementType>();

                for (int ii = 0; ii < controllers.Count; ii++)
                {
                    UserManagementType c;

                    if (controllers.EnumValue (ii, out c))
                    {
                        gc.Add (c);
                    }
                }

                UserManagement = gc.ToArray ();
            }
        }

        public UserManagementCapability (UserManagementCapability other)
        : base (other)
        {
            if (other.UserManagement == null || other.UserManagement.Length <= 0)
            {
                UserManagement = null;
            }
            else
            {
                System.Array.Copy (other.UserManagement, UserManagement, other.UserManagement.Length);
            }
        }

        #region implemented abstract members of BaseCapability
        public override PListDictionary Serialize ()
        {
            var dic = new PListDictionary ();

            if (UserManagement != null && UserManagement.Length > 0)
            {
                var controllers = new PListArray ();

                foreach (var c in UserManagement)
                {
                    controllers.Add (c.ToString ());
                }

                dic.Add (USER_MANAGEMENT_KEY, controllers);
            }

            return dic;
        }

        public override BaseCapability Clone ()
        {
            return new UserManagementCapability (this);
        }

        #endregion

        public UserManagementType [] UserManagement
        {
            get;
            set;
        }

    }
}
