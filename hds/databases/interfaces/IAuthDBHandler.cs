﻿using System;
using System.Collections.Generic;
using System.Text;

namespace hds.databases.interfaces{
    public interface IAuthDBHandler{
        bool fetchWordList(ref WorldList wl);
    }
}
