﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingModel
{
    public class Model
    {
        public event ModelChangedEventHandler _modelChanged;
        public delegate void ModelChangedEventHandler();

        CommandManager _commandManager = new CommandManager();
        private FileProducer _fileProducer;//處理檔案
        private int _currentSelectedIndex = -1;
        private IShape _firstShape;
        private IShape _secondShape;
        private IShape _currentShape;
        private IState _state = new DrawingEllipseState();
        private readonly List<IShape> _shapes = new List<IShape>();

        const int DEFAULT_MODE = -1;
        private int _drawingMode = DEFAULT_MODE;//-1 = NO SHAPE, 0 = line, 1 = rectangle, 2 = ellipse

        //get for test ,set for function
        public int DrawingMode
        {
            get
            {
                return _drawingMode;
            }
            set
            {
                _state = StateFactory.CreateState(value);
                _drawingMode = value;
            }
        }

        public bool RedoStatus
        {
            get
            {
                return _commandManager.RedoStatus;
            }
        }

        public bool UndoStatus
        {
            get
            {
                return _commandManager.UndoStatus;
            }
        }

        //存取按下的資料
        public void PressPointer(double currentXCoordinate, double currentYCoordinate)
        {
            _firstShape = GetOnShape(currentXCoordinate, currentYCoordinate);
            _state.Press(currentXCoordinate, currentYCoordinate, _firstShape);
        }

        //移動時的資料(讓畫面可以跟著滑鼠畫圖的東西)
        public void MovePointer(double currentXCoordinate, double currentYCoordinate)
        {
            _currentShape = _state.Move(currentXCoordinate, currentYCoordinate, _firstShape);
            NotifyModelChanged();
        }

        //滑鼠按鍵放開後做的事情
        public void ReleasePointer(double currentXCoordinate, double currentYCoordinate)
        {
            _secondShape = GetOnShape(currentXCoordinate, currentYCoordinate);
            IShape shape = _state.Release(_firstShape, _secondShape);

            RunCommand(shape);

            _currentShape = null;//優化
            NotifyModelChanged();
        }

        //跑command
        private void RunCommand(IShape shape)
        {
            if (shape != null && _drawingMode != DEFAULT_MODE)
                _commandManager.Execute(new DrawCommand(this, shape));
            if (shape != null && _drawingMode == DEFAULT_MODE)
                _commandManager.Execute(new MoveCommand(this, shape, shape.GetMoveTuple()));
        }

        //清空
        public void Clear()
        {
            _commandManager.Clear();
            CleanMark();
            _shapes.Clear();
            NotifyModelChanged();
        }

        //畫圖
        public void Draw(IGraphics graphics)
        {
            graphics.ClearAll();
            foreach (IShape aShape in _shapes)
                if (aShape.GetType() == new Line().GetType())
                    aShape.Draw(graphics);
            foreach (IShape aShape in _shapes)
                if (aShape.GetType() != new Line().GetType())
                    aShape.Draw(graphics);
            foreach (IShape aShape in _shapes)
                aShape.Selected(graphics);
            if (_currentShape != null)
            {
                _currentShape.Draw(graphics);
            }
        }

        //標出選擇物件
        public void MarkShape(double xCoordinate, double yCoordinate)
        {
            if (_currentSelectedIndex != -1)
                CleanMark();
            for (int i = _shapes.Count - 1; i >= 0; i--)
            {
                if (PointInShape(xCoordinate, yCoordinate, i))
                {
                    _shapes[i].IsSelected = true;
                    _currentSelectedIndex = i;
                    break;
                }
            }
        }

        //清掉選取
        public void CleanMark()
        {
            foreach (IShape shape in _shapes)
            {
                if (shape.IsSelected)
                {
                    shape.IsSelected = false;
                }
                _currentSelectedIndex = DEFAULT_MODE;
            }
        }

        //observer
        public void NotifyModelChanged()
        {
            if (_modelChanged != null)
                _modelChanged();
        }

        //清空全部 目前沒實作，因為畫面自己有在做事
        public void ClearAll()
        {
            // OnPaint時會自動清除畫面，因此不需實作
        }

        //current for test 得到shapes
        public List<IShape> GetShapes()
        {
            return _shapes;
        }

        //增加新形狀
        public void AddNewShape(IShape shape)
        {
            _shapes.Add(shape);
        }

        //刪除最後畫的shape
        public void DeleteShape()
        {
            _shapes.RemoveAt(_shapes.Count - 1);
        }

        //儲存shape移動
        public void SaveShapeMove(IShape shape, Tuple<double, double, double, double> move)
        {
            shape.SaveMove(move);
            RefreshLine();
        }

        //移動取消
        public void DeleteShapeMove(IShape shape)
        {
            shape.MoveDisable();
            RefreshLine();
        }

        //上一步
        public void Undo()
        {
            _commandManager.Undo();
            NotifyModelChanged();
        }

        //下一步
        public void Redo()
        {
            _commandManager.Redo();
            NotifyModelChanged();
        }

        //得到點擊的shape
        public IShape GetOnShape(double currentXCoordinate, double currentYCoordinate)
        {
            for (int i = _shapes.Count - 1; i >= 0; i--)
            {
                if (PointInShape(currentXCoordinate, currentYCoordinate, i))
                {
                    return _shapes[i];
                }
            }
            return null;
        }

        //確定點擊位置在shape上面
        private bool PointInShape(double currentXCoordinate, double currentYCoordinate, int index)
        {
            IShape shape = _shapes[index];
            if (shape.IsInShape(currentXCoordinate, currentYCoordinate))
            {
                if (shape.GetType() != new Line().GetType())
                    return true;
            }
            return false;
        }

        //刷新移動圖時Line的狀態
        private void RefreshLine()
        {
            foreach (IShape shape in _shapes)
            {
                shape.Refresh();
            }
        }

        //save
        public void SaveAllShapes()
        {
            _fileProducer = new FileProducer(this);
            _fileProducer.UploadShapes();
        }

        //load
        public void LoadFileShapes()
        {
            _shapes.Clear();//先清掉
            _fileProducer = new FileProducer(this);
            _fileProducer.DownloadShapes();
            _commandManager.Clear();//不給redo undo
            CleanMark();//避免error
        }
    }
}