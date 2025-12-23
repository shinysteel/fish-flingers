using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShinyOwl.Common
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}