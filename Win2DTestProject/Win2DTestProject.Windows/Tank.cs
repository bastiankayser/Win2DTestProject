using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Graphics.Canvas.Effects;

namespace Win2DTestProject
{
    public class Tank
    {

        public float damage;
        public Vector2 position;
        public Vector2 direction;
        public float currentRot;
        public float desiredRot;

        public float size;
        public int fraction;

        public bool selected;
        public bool hit;
        public bool destroyed;

        public bool firing;
        public bool rotating;

        public Transform2DEffect rotationEffect;
        public Transform2DEffect translationEffect;
        public Vector2 target;
    }
}
