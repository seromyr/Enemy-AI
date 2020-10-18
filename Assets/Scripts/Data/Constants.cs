using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public static class Name
    {
        public const string PLAYER = "Player";
        public const string TARGET = "Target";
    }

    public enum AIState
    {
        Patrolling,
        Chasing,
        Attacking,
        Searching,
        Investigating,
        Retreating,
        Panic,
    }

    public enum Phase
    {
        Hunt,
        BeHunted,
    }

    public enum Form
    {
        Human,
        Hunter,
        God,
    }
}
