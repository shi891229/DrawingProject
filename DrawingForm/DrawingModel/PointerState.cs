﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingModel
{
    class PointerState : IState
    {
        double _firstPointX;
        double _firstPointY;
        double _secondX;
        double _secondY;
        private bool _isPressed = false;
        private bool _isMoving = false;

        //按下
        public void Press(double currentXCoordinate, double currentYCoordinate, IShape firstShape)
        {
            if (currentXCoordinate > 0 && currentYCoordinate > 0 && firstShape != null && firstShape.IsSelected)
            {
                _firstPointX = _secondX = currentXCoordinate;
                _firstPointY = _secondY = currentYCoordinate;
                _isPressed = true;
                _isMoving = false;
            }
        }

        //移動
        public IShape Move(double currentXCoordinate, double currentYCoordinate, IShape selectedShape)
        {
            if (_isPressed && selectedShape != null && selectedShape.IsSelected)
            {
                _secondX = currentXCoordinate;
                _secondY = currentYCoordinate;
                _isMoving = true;
                selectedShape.MoveShape(_secondX - _firstPointX, _secondY - _firstPointY);
            }
            return null;
        }

        //滑鼠按鍵放開後做的事情
        public IShape Release(IShape firstShape, IShape secondShape)
        {
            if (_isPressed && _isMoving)
            {
                return firstShape;
            }
            return null;
        }

    }
}
