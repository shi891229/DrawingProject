﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingModel
{
    class Line : Shape
    {
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;
        public void Draw(IGraphics graphics)
        {
            graphics.DrawLine(_x1, _y1, _x2, _y2);
        }

        public double FirstX
        {
            get
            {
                return _x1;
            }
            set
            {
                _x1 = value;
            }
        }

        public double SecondX
        {
            get
            {
                return _x2;
            }
            set
            {
                _x2 = value;
            }
        }

        public double FirstY
        {
            get
            {
                return _y1;
            }
            set
            {
                _y1 = value;
            }
        }

        public double SecondY
        {
            get
            {
                return _y2;
            }
            set
            {
                _y2 = value;
            }
        }
    }
}
