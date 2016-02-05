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
        public Vector2 direction;
        private Vector2 _target;

        public Transform2DEffect translationEffect;

        public Vector2 Target
        {
            get { return _target; }
            set
            {
                _target = value;
                direction = Vector2.Normalize(_target - pos);
            }
        }
    }
}
