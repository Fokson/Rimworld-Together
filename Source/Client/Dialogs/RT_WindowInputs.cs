﻿using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace GameClient
{
    public interface RT_WindowInputs
    {
        public abstract void CacheInputs();

        public abstract void SubstituteInputs(List<object> newInputs);
    }
}