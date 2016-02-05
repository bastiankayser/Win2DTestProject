using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;

namespace Win2DTestProject
{
    class Projectile
    {
        public Vector2 pos;
        public Vector2 directtion;

        public Transform2DEffect translationEffect;
    }
}
