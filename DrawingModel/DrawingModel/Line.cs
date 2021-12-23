﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingModel
{
    class Line : IShape
    {
        const int TWO = 2;
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;
        //畫圖
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

        public Tuple<double, double> Center
        {
            get
            {
                double centerX = (_x1 + _x2) / TWO;
                double centerY = (_y1 + _y2) / TWO;
                return new Tuple<double, double>(centerX, centerY);
            }
        }

        public IShape FirseShape
        {
            get; set;
        }

        public IShape SecondShape
        {
            get; set;
        }

    }
}
