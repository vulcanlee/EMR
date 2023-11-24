﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Events
{
    public class OnOffNavigationPageEvent : PubSubEvent<OnOffNavigationPagePayload>
    {

    }
    public class OnOffNavigationPagePayload
    {
        public bool IsOn { get; set; } = false;
    }
}
