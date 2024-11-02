using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace OkapiKit
{
    public static class Helpers
    {
        public static void CheckErrorAnim(Animator anm, string logParameter, string parameterName, AnimatorControllerParameterType type, List<LogEntry> ret)
        {
            if (parameterName == "")
            {
                return;
            }
            if (!anm.isActiveAndEnabled) return;
            for (int i = 0; i < anm.parameterCount; i++)
            {
                try
                {
                    var param = anm.GetParameter(i);
                    if (param.name == parameterName)
                    {
                        if (param.type != type)
                        {
                            ret.Add(new LogEntry(LogEntry.Type.Error, $"Animation parameter type {parameterName} for {logParameter} is of wrong type (expected {type}, found {param.type})!", $"Animation parameter type {parameterName} for {logParameter} is of wrong type (expected {type}, found {param.type})!"));
                        }
                        return;
                    }
                }
                catch
                {
                    // Unity screws up sometimes and returns an IndexOutOfRangeException when it should work properly
                }
            }

            ret.Add(new LogEntry(LogEntry.Type.Error, $"Animation parameter {parameterName} for {logParameter} not found!", "The given animator doesn't have this parameter. Either set it to empty (so we don't try to drive it), or add it on the animator."));
        }
    }
}
