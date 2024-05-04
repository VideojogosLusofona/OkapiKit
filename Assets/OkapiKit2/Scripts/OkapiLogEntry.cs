using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKitV2
{
    public struct LogEntry
    {
        public enum Type { Debug, Warning, Error };

        public Type     type;
        public string   text;
        public string   tooltip;

        public LogEntry(Type type, string text, string tooltip = "")
        {
            this.type = type;
            this.text = text;
            this.tooltip = tooltip;
        }
    }
}
