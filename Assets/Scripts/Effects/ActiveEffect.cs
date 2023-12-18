using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class ActiveEffect
    {
        float endTime;
        public EffectType Type;
        public bool IsHaunt = true;
        public string Text;

        public ActiveEffect(EffectType effectType, float duration, string text)
        {
            endTime = Time.time + duration;
            Type = effectType;
            Text = text;
        }
    
        public bool HasEnded()    
        {
            return Time.time > endTime;
        }
        
        public float GetRemainingTime() {
            return endTime - Time.time;
        }
        
        public new string ToString() {
            return Text + " [" + GetRemainingTime().ToString("0.0") + "s]";
        }
    }
}
