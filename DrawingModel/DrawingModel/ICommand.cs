﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingModel
{
    public interface ICommand
    {
        //執行動作
        void Execute();
        //去除前一個執行的動作
        void UnExecute();

    }
}
