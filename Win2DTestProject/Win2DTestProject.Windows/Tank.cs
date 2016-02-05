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
        public Vector2 CurrentPosition;
        public Vector2 direction;
        private Vector2 _targetPosition;
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
        private Vector2 _firingTarget;

        //this could also be a list of way points
        public Vector2 TargetPosition
        {
            get { return _targetPosition; }
            set
            {
                _targetPosition = value;
                direction = Vector2.Normalize(_targetPosition - CurrentPosition);
            }
        }

        public Vector2 FiringTarget
        {
            get { return _firingTarget; }
            set
            {
                _firingTarget = value;
                var targetDirection = Vector2.Normalize(_firingTarget - CurrentPosition);
                desiredRot = (float)Math.Atan2(targetDirection.Y, targetDirection.X);
            }
        }
    }
}
