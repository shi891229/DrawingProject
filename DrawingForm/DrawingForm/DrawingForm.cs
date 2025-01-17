﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingModel;
using System.IO;

namespace DrawingForm
{
    public partial class DrawingForm : Form
    {
        readonly Model _model;
        readonly PresentationModel.PresentationModel _presentationModel;
        readonly Panel _canvas = new DoubleBufferedPanel();

        const int LINE_MODE = 0;
        const int RECTANGLE_MODE = 1;
        const int ELLIPSE_MODE = 2;
        const int DEFAULT_MODE = -1;
        const String DEFAULT_LABEL = "No Shape Selected";

        public DrawingForm()
        {
            InitializeComponent();
            _canvas.Name = "_canvas";
            _canvas.Dock = DockStyle.Fill;
            _canvas.BackColor = System.Drawing.Color.LightYellow;
            _canvas.MouseDown += HandleCanvasPressed;
            _canvas.MouseUp += HandleCanvasReleased;
            _canvas.MouseMove += HandleCanvasMoved;
            _canvas.Paint += HandleCanvasPaint;
            Controls.Add(_canvas);
            _model = new Model();
            _presentationModel = new PresentationModel.PresentationModel(_model);
            ResetDefaultButtonAndMode();
            _model._modelChanged += HandleModelChanged;
            RefreshButtonStatus();
        }

        //clear按鈕按下的事件
        public void HandleClearButtonClick(object sender, System.EventArgs e)
        {
            _model.Clear();
            _label.Text = DEFAULT_LABEL;
            ResetDefaultButtonAndMode();
        }

        //滑鼠按下Canvas的事件
        public void HandleCanvasPressed(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _model.PressPointer(e.X, e.Y);
        }

        //滑鼠放開Canvas的事件
        public void HandleCanvasReleased(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _label.Text = _presentationModel.GetShapeData(e.X, e.Y);
            _model.ReleasePointer(e.X, e.Y);
            ResetDefaultButtonAndMode();
        }

        //滑鼠移動的事件
        public void HandleCanvasMoved(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _model.MovePointer(e.X, e.Y);
        }

        //畫圖事件
        public void HandleCanvasPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            _presentationModel.Draw(e.Graphics);
        }

        //當觀察者觸發時跑的事件
        public void HandleModelChanged()
        {
            _redo.Enabled = _model.RedoStatus;
            _undo.Enabled = _model.UndoStatus;
            Invalidate(true);
        }

        //刷新按鈕狀態
        private void RefreshButtonStatus()
        {
            _line.Enabled = _presentationModel.LineButtonStatus;
            _rectangle.Enabled = _presentationModel.RectangleButtonStatus;
            _ellipse.Enabled = _presentationModel.EllipseButtonStatus;
            _clear.Enabled = _presentationModel.ClearButtonStatus;
        }

        //Line按鈕按下的事件
        public void HandleLineButtonClick(object sender, System.EventArgs e)
        {
            _presentationModel.SetButtonStatus(false, true, true);
            _model.CleanMark();
            _label.Text = DEFAULT_LABEL;
            _model.NotifyModelChanged();

            _presentationModel.DrawingMode = LINE_MODE;

            RefreshButtonStatus();
        }

        //Rectangle按鈕按下的事件
        public void HandleRectangleButtonClick(object sender, System.EventArgs e)
        {
            _presentationModel.SetButtonStatus(true, false, true);
            _model.CleanMark();
            _label.Text = DEFAULT_LABEL;
            _model.NotifyModelChanged();

            _presentationModel.DrawingMode = RECTANGLE_MODE;

            RefreshButtonStatus();
        }

        //Ellipse按鈕按下的事件
        public void HandleEllipseButtonClick(object sender, System.EventArgs e)
        {
            _presentationModel.SetButtonStatus(true, true, false);
            _model.CleanMark();
            _label.Text = DEFAULT_LABEL;
            _model.NotifyModelChanged();

            _presentationModel.DrawingMode = ELLIPSE_MODE;

            RefreshButtonStatus();
        }

        //回到預設狀態
        private void ResetDefaultButtonAndMode()
        {
            _presentationModel.SetButtonStatus(true, true, true);

            _presentationModel.DrawingMode = DEFAULT_MODE;

            RefreshButtonStatus();
        }

        //undo
        private void UndoHandler(Object sender, EventArgs e)
        {
            _model.Undo();
            RefreshButtonStatus();
        }

        //redo
        private void RedoHandler(Object sender, EventArgs e)
        {
            _model.Redo();
            RefreshButtonStatus();
        }

        //save
        private async void ClickSaveButton(object sender, EventArgs e)
        {
            _save.Enabled = false;
            await Task.Factory.StartNew(() =>
            {
                _model.SaveAllShapes();
            });
            _label.Text = DEFAULT_LABEL;
            _save.Enabled = true;
        }

        //load
        private async void ClickLoadButton(object sender, EventArgs e)
        {
            _load.Enabled = false;
            await Task.Factory.StartNew(() =>
            {
                _model.LoadFileShapes();
            });
            _label.Text = DEFAULT_LABEL;
            _load.Enabled = true;
            _clear.Enabled = true;
            _model.NotifyModelChanged();
        }
    }
}
