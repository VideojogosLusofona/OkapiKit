using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public struct LogEntry
    {
        public enum Type { Debug, Warning, Error };

        public Type     type;
        public string   text;

        public LogEntry(Type type, string text)
        {
            this.type = type;
            this.text = text;
        }
    }

}
