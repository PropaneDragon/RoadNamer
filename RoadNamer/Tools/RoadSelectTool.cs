using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadNamer.Tools
{
    class RoadSelectTool : DefaultTool
    {
        protected override void Awake()
        {
            base.Awake();

            Debug.Log("Tool awake");
        }

        protected override void OnToolGUI()
        {
            base.OnToolGUI();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();
        }
    }
}
